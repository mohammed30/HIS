using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Application.Dtos;
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
    private readonly IRepository<Bed, Guid> _bedRepository;
    private readonly IRepository<HIS.Accounting.Account, Guid> _accountRepository;
    private readonly IRepository<HIS.Accounting.JournalEntry, Guid> _journalEntryRepository;

    public AdmissionAppService(
        IRepository<Admission, Guid> repository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Room, Guid> roomRepository,
        IRepository<Bed, Guid> bedRepository,
        IRepository<HIS.Accounting.Account, Guid> accountRepository,
        IRepository<HIS.Accounting.JournalEntry, Guid> journalEntryRepository) : base(repository)
    {
        _patientRepository = patientRepository;
        _roomRepository = roomRepository;
        _bedRepository = bedRepository;
        _accountRepository = accountRepository;
        _journalEntryRepository = journalEntryRepository;
    }

    public override async Task<AdmissionDto> CreateAsync(CreateUpdateAdmissionDto input)
    {
        // 1. Validate Room
        var room = await _roomRepository.GetAsync(input.RoomId);
        
        // 2. Validate Bed
        var bed = await _bedRepository.GetAsync(input.BedId);
        if (bed.RoomId != input.RoomId)
        {
            throw new UserFriendlyException("السرير المختار لا ينتمي للغرفة المحددة");
        }
        if (bed.Status != BedStatus.Available)
        {
             throw new UserFriendlyException("السرير المختار غير متاح حالياً");
        }

        var admission = new Admission(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            input.PatientId,
            input.RoomId,
            input.BedId
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

        // Update room legacy counter
        room.AvailableBeds--;
        if (room.AvailableBeds < 0) room.AvailableBeds = 0;
        if (room.AvailableBeds == 0)
        {
            room.Status = RoomStatus.Occupied;
        }
        await _roomRepository.UpdateAsync(room);

        // Update Bed status
        bed.Status = BedStatus.Occupied;
        await _bedRepository.UpdateAsync(bed);

        // Accounting Journal Entry
        var patient = await _patientRepository.GetAsync(input.PatientId);
        var patientName = !string.IsNullOrWhiteSpace(patient.FullNameAr) ? patient.FullNameAr : patient.MRN;
        
        var arAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1120"); // Accounts Receivable
        var checkAmount = input.NumberOfDays > 0 ? (input.NumberOfDays * room.DailyRate) : (input.PaidAmount > 0 ? input.PaidAmount : 1000m); // Default fallback

        if (arAccount != null)
        {
            var revenueAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "4100");
            var cashAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1110");
            var jeNumber = $"ADM-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
            
            var je = new HIS.Accounting.JournalEntry(
                GuidGenerator.Create(),
                DateTime.Now,
                jeNumber,
                $"حجز تنويم - المريض: {patientName}"
            );

            if (input.PaidAmount > 0 && cashAccount != null)
            {
                // Advance Payment Booking: Debit Cash, Credit AR
                je.AddLine(GuidGenerator, cashAccount.Id, input.PaidAmount, 0);
                je.AddLine(GuidGenerator, arAccount.Id, 0, input.PaidAmount);
            }
            else if (revenueAccount != null)
            {
                // Standard Booking: Debit AR, Credit Revenue
                je.AddLine(GuidGenerator, arAccount.Id, checkAmount, 0);
                je.AddLine(GuidGenerator, revenueAccount.Id, 0, checkAmount);
            }

            await _journalEntryRepository.InsertAsync(je);
        }

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
        room.AvailableBeds++; // Keep legacy counter in sync
        if (room.Status == RoomStatus.Occupied)
        {
            room.Status = RoomStatus.Available;
        }
        await _roomRepository.UpdateAsync(room);

        // Update Bed status
        var bed = await _bedRepository.GetAsync(admission.BedId);
        bed.Status = BedStatus.Cleaning; // Mark as cleaning before available
        await _bedRepository.UpdateAsync(bed);

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

        if (input.RoomTypeId.HasValue)
        {
            var roomsQuery = await _roomRepository.GetQueryableAsync();
            queryable = from admission in queryable
                        join room in roomsQuery on admission.RoomId equals room.Id
                        where room.Type == (RoomType)input.RoomTypeId.Value
                        select admission;
        }

        if (!string.IsNullOrWhiteSpace(input.SearchText))
        {
            var patientsQuery = await _patientRepository.GetQueryableAsync();
            queryable = from admission in queryable
                        join patient in patientsQuery on admission.PatientId equals patient.Id
                        where patient.FirstNameAr.Contains(input.SearchText) ||
                              patient.LastNameAr.Contains(input.SearchText) ||
                              patient.MRN.Contains(input.SearchText)
                        select admission;
        }

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

    public override async Task<PagedResultDto<AdmissionDto>> GetListAsync(GetAdmissionsInput input)
    {
        var result = await base.GetListAsync(input);
        foreach (var dto in result.Items)
        {
            await EnrichAdmissionDtoAsync(dto);
        }
        return result;
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

        if (dto.BedId.HasValue)
        {
            var bed = await _bedRepository.FindAsync(dto.BedId.Value);
            if (bed != null)
            {
                dto.BedNumber = bed.BedNumber;
            }
        }
    }
}
