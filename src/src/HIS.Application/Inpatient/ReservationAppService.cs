using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using HIS.Permissions;
using HIS.Patients;
using HIS.Rooms;

namespace HIS.Inpatient;

/// <summary>
/// خدمة الحجز
/// </summary>
[Authorize(HISPermissions.Reception.Default)]
public class ReservationAppService : CrudAppService<
    Reservation,
    ReservationDto,
    Guid,
    GetReservationsInput,
    CreateUpdateReservationDto>, IReservationAppService
{
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Room, Guid> _roomRepository;
    private readonly IRepository<Bed, Guid> _bedRepository;

    public ReservationAppService(
        IRepository<Reservation, Guid> repository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Room, Guid> roomRepository,
        IRepository<Bed, Guid> bedRepository) : base(repository)
    {
        _patientRepository = patientRepository;
        _roomRepository = roomRepository;
        _bedRepository = bedRepository;
    }

    public override async Task<ReservationDto> CreateAsync(CreateUpdateReservationDto input)
    {
        // 1. Validate Room and Bed
        var room = await _roomRepository.GetAsync(input.RoomId);
        Bed? bed = null;
        if (input.BedId.HasValue)
        {
            bed = await _bedRepository.GetAsync(input.BedId.Value);
            if (bed.RoomId != input.RoomId)
            {
                throw new UserFriendlyException("السرير المختار لا ينتمي للغرفة المحددة");
            }
        }

        // 2. Validate Dates
        if (input.StartDate >= input.EndDate)
        {
            throw new UserFriendlyException("تاريخ النهاية يجب أن يكون بعد تاريخ البداية");
        }
        if (input.StartDate.Date < DateTime.Now.Date)
        {
             // Optional: Allow retroactive reservations? Usually strict for future.
             // throw new UserFriendlyException("لا يمكن الحجز في الماضي");
        }

        // 3. Check Availability
        if (await CheckOverlapAsync(input.RoomId, input.BedId, input.StartDate, input.EndDate))
        {
            throw new UserFriendlyException("الغرفة/السرير محجوز بالفعل في هذه الفترة");
        }

        return await base.CreateAsync(input);
    }

    public override async Task<ReservationDto> UpdateAsync(Guid id, CreateUpdateReservationDto input)
    {
         // 1. Validate Room and Bed
        var room = await _roomRepository.GetAsync(input.RoomId);
        if (input.BedId.HasValue)
        {
            var bed = await _bedRepository.GetAsync(input.BedId.Value);
            if (bed.RoomId != input.RoomId)
            {
                throw new UserFriendlyException("السرير المختار لا ينتمي للغرفة المحددة");
            }
        }

        // 2. Check Availability (excluding current reservation)
        if (await CheckOverlapAsync(input.RoomId, input.BedId, input.StartDate, input.EndDate, id))
        {
            throw new UserFriendlyException("الغرفة/السرير محجوز بالفعل في هذه الفترة");
        }

        return await base.UpdateAsync(id, input);
    }

    private async Task<bool> CheckOverlapAsync(Guid roomId, Guid? bedId, DateTime start, DateTime end, Guid? excludeId = null)
    {
        var query = await Repository.GetQueryableAsync();
        
        query = query
            .Where(x => x.RoomId == roomId)
            .WhereIf(bedId.HasValue, x => x.BedId == bedId)
            .WhereIf(excludeId.HasValue, x => x.Id != excludeId)
            .Where(x => x.Status != ReservationStatus.Cancelled);
            
        return await AsyncExecuter.AnyAsync(query, x => x.StartDate < end && x.EndDate > start);
    }

    protected override async Task<IQueryable<Reservation>> CreateFilteredQueryAsync(GetReservationsInput input)
    {
        var queryable = await base.CreateFilteredQueryAsync(input);

        return queryable
            .WhereIf(input.PatientId.HasValue, x => x.PatientId == input.PatientId!.Value)
            .WhereIf(input.RoomId.HasValue, x => x.RoomId == input.RoomId!.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value)
            .WhereIf(input.FromDate.HasValue, x => x.StartDate >= input.FromDate!.Value)
            .WhereIf(input.ToDate.HasValue, x => x.EndDate <= input.ToDate!.Value);
    }

    public override async Task<ReservationDto> GetAsync(Guid id)
    {
        var dto = await base.GetAsync(id);
        await EnrichDtoAsync(dto);
        return dto;
    }

    public override async Task<PagedResultDto<ReservationDto>> GetListAsync(GetReservationsInput input)
    {
        var result = await base.GetListAsync(input);
        foreach (var dto in result.Items)
        {
            await EnrichDtoAsync(dto);
        }
        return result;
    }

    private async Task EnrichDtoAsync(ReservationDto dto)
    {
        var patient = await _patientRepository.FindAsync(dto.PatientId);
        if (patient != null) dto.PatientName = patient.FullNameAr;

        var room = await _roomRepository.FindAsync(dto.RoomId);
        if (room != null) dto.RoomNumber = room.RoomNumber;

        if (dto.BedId.HasValue)
        {
            var bed = await _bedRepository.FindAsync(dto.BedId.Value);
            if (bed != null) dto.BedNumber = bed.BedNumber;
        }
    }
}
