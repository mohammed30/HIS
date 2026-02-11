using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using Microsoft.AspNetCore.Authorization;
using HIS.Permissions;
using HIS.Patients;
using HIS.Rooms;

namespace HIS.Inpatient;

/// <summary>
/// خدمة التنويم
/// </summary>
[Authorize(HISPermissions.Reception.Default)]
public class AdmissionAppService : CrudAppService<
    Admission,
    AdmissionDto,
    Guid,
    GetAdmissionsInput,
    CreateUpdateAdmissionDto>, IAdmissionAppService
{
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Room, Guid> _roomRepository;

    public AdmissionAppService(
        IRepository<Admission, Guid> repository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Room, Guid> roomRepository) : base(repository)
    {
        _patientRepository = patientRepository;
        _roomRepository = roomRepository;
    }

    public override async Task<AdmissionDto> CreateAsync(CreateUpdateAdmissionDto input)
    {
        // Validate room availability
        var room = await _roomRepository.GetAsync(input.RoomId);
        if (room.AvailableBeds <= 0)
        {
            throw new UserFriendlyException("لا توجد أسرة متاحة في هذه الغرفة");
        }

        var admission = new Admission(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            input.PatientId,
            input.RoomId
        )
        {
            InsuranceCeiling = input.InsuranceCeiling,
            CompanionName = input.CompanionName,
            CompanionPhone = input.CompanionPhone,
            CompanionAddress = input.CompanionAddress,
            Purpose = input.Purpose,
            PharmacyPercentage = input.PharmacyPercentage,
            IsServicesStopped = input.IsServicesStopped,
            Notes = input.Notes
        };

        await Repository.InsertAsync(admission);

        // Update room available beds
        room.AvailableBeds--;
        if (room.AvailableBeds == 0)
        {
            room.Status = RoomStatus.Occupied;
        }
        await _roomRepository.UpdateAsync(room);

        var dto = ObjectMapper.Map<Admission, AdmissionDto>(admission);
        await EnrichAdmissionDtoAsync(dto);
        return dto;
    }

    /// <summary>
    /// إخراج المريض (إذن خروج)
    /// </summary>
    public async Task<AdmissionDto> DischargeAsync(Guid id, DischargeAdmissionDto input)
    {
        var admission = await Repository.GetAsync(id);
        admission.DischargeDate = input.DischargeDate;
        admission.NumberOfDays = (int)(input.DischargeDate - admission.AdmissionDate).TotalDays;
        if (admission.NumberOfDays < 1) admission.NumberOfDays = 1;
        admission.Status = AdmissionStatus.Discharged;

        if (!string.IsNullOrWhiteSpace(input.Notes))
        {
            admission.Notes = input.Notes;
        }

        // Calculate total based on days and room rate
        var room = await _roomRepository.GetAsync(admission.RoomId);
        admission.TotalAmount = admission.NumberOfDays * room.DailyRate;

        await Repository.UpdateAsync(admission);

        // Free up the bed
        room.AvailableBeds++;
        if (room.Status == RoomStatus.Occupied)
        {
            room.Status = RoomStatus.Available;
        }
        await _roomRepository.UpdateAsync(room);

        var dto = ObjectMapper.Map<Admission, AdmissionDto>(admission);
        await EnrichAdmissionDtoAsync(dto);
        return dto;
    }

    /// <summary>
    /// تحديث عدد أيام الإقامة
    /// </summary>
    public async Task<AdmissionDto> UpdateDaysAsync(Guid id, int numberOfDays)
    {
        var admission = await Repository.GetAsync(id);
        admission.NumberOfDays = numberOfDays;

        var room = await _roomRepository.GetAsync(admission.RoomId);
        admission.TotalAmount = numberOfDays * room.DailyRate;

        await Repository.UpdateAsync(admission);

        var dto = ObjectMapper.Map<Admission, AdmissionDto>(admission);
        await EnrichAdmissionDtoAsync(dto);
        return dto;
    }

    protected override async Task<IQueryable<Admission>> CreateFilteredQueryAsync(GetAdmissionsInput input)
    {
        var queryable = await base.CreateFilteredQueryAsync(input);

        return queryable
            .WhereIf(input.PatientId.HasValue, x => x.PatientId == input.PatientId!.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value)
            .WhereIf(input.RoomId.HasValue, x => x.RoomId == input.RoomId!.Value)
            .WhereIf(input.FromDate.HasValue, x => x.AdmissionDate >= input.FromDate!.Value)
            .WhereIf(input.ToDate.HasValue, x => x.AdmissionDate <= input.ToDate!.Value);
    }

    protected override IQueryable<Admission> ApplyDefaultSorting(IQueryable<Admission> query)
    {
        return query.OrderByDescending(x => x.AdmissionDate);
    }

    public override async Task<AdmissionDto> GetAsync(Guid id)
    {
        var dto = await base.GetAsync(id);
        await EnrichAdmissionDtoAsync(dto);
        return dto;
    }

    private async Task EnrichAdmissionDtoAsync(AdmissionDto dto)
    {
        var patient = await _patientRepository.FindAsync(dto.PatientId);
        if (patient != null)
        {
            dto.PatientName = patient.FullNameAr;
            dto.PatientFileNumber = patient.MRN;
        }

        var room = await _roomRepository.FindAsync(dto.RoomId);
        if (room != null)
        {
            dto.RoomNumber = room.RoomNumber;
            dto.RoomTypeName = room.Type.ToString();
        }
    }
}
