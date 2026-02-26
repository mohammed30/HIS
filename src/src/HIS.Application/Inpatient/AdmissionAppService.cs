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
    private readonly IRepository<HIS.Billing.InpatientDeposit, Guid> _inpatientDepositRepository;
    private readonly IRepository<PatientTransfer, Guid> _patientTransferRepository;
    private readonly HIS.Billing.IInvoiceAppService _invoiceAppService;

    public AdmissionAppService(
        IRepository<Admission, Guid> repository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Room, Guid> roomRepository,
        IRepository<Bed, Guid> bedRepository,
        IRepository<HIS.Accounting.Account, Guid> accountRepository,
        IRepository<HIS.Accounting.JournalEntry, Guid> journalEntryRepository,
        IRepository<HIS.Billing.InpatientDeposit, Guid> inpatientDepositRepository,
        IRepository<PatientTransfer, Guid> patientTransferRepository,
        HIS.Billing.IInvoiceAppService invoiceAppService) : base(repository)
    {
        _patientRepository = patientRepository;
        _roomRepository = roomRepository;
        _bedRepository = bedRepository;
        _accountRepository = accountRepository;
        _journalEntryRepository = journalEntryRepository;
        _inpatientDepositRepository = inpatientDepositRepository;
        _patientTransferRepository = patientTransferRepository;
        _invoiceAppService = invoiceAppService;
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
        int currentStayDays = (int)(input.DischargeDate - admission.LastTransferDate).TotalDays;
        if (currentStayDays < 1 && admission.AccumulatedRoomCharges == 0) currentStayDays = 1; // Minimum 1 day total if no transfers
        
        decimal currentStayCharges = currentStayDays * room.DailyRate;
        admission.TotalAmount = admission.AccumulatedRoomCharges + currentStayCharges;
        
        // Handle Advance Payments (Deposits)
        var activeDeposits = await _inpatientDepositRepository.GetListAsync(d => d.AdmissionId == id && d.Status == HIS.Billing.DepositStatus.Active);
        if (activeDeposits.Any())
        {
            decimal totalDeposits = activeDeposits.Sum(d => d.Amount);
            admission.PaidAmount += totalDeposits;

            foreach (var deposit in activeDeposits)
            {
                deposit.Status = HIS.Billing.DepositStatus.Consumed;
                await _inpatientDepositRepository.UpdateAsync(deposit);
            }
        }

        // Generate Consolidated Invoice
        var invoiceInput = new HIS.Billing.CreateUpdateInvoiceDto
        {
            PatientId = admission.PatientId,
            DueDate = input.DischargeDate,
            Notes = $"Consolidated Inpatient Invoice - Admission: {admission.Id.ToString().Substring(0,8)}",
            Items = new System.Collections.Generic.List<HIS.Billing.CreateUpdateInvoiceItemDto>()
        };

        // Add Room Charges
        if (admission.TotalAmount > 0)
        {
            invoiceInput.Items.Add(new HIS.Billing.CreateUpdateInvoiceItemDto
            {
                ServiceType = HIS.Billing.ServiceType.Consultation, // Or better map to RoomCharge
                Description = $"رسوم إقامة الغرف - {admission.NumberOfDays} يوم",
                UnitPrice = admission.TotalAmount, // This includes surgeries and accumulated charges added to TotalAmount
                Quantity = 1,
                IsCoveredByInsurance = admission.InsuranceAmount > 0
            });
        }

        // Deduct Deposits as lines or apply immediately 
        // We handle this by setting PaidAmount on the invoice directly, but since CreateAsync doesn't accept PaidAmount directly, 
        // we'll fetch the created invoice and update it or create a payment record.
        var invoice = await _invoiceAppService.CreateAsync(invoiceInput);
        admission.InvoiceId = invoice.Id;
        
        if (admission.PaidAmount > 0)
        {
            var paymentAppService = LazyServiceProvider.LazyGetRequiredService<HIS.Billing.IPaymentAppService>();
            await paymentAppService.CreateAsync(new HIS.Billing.CreatePaymentDto 
            {
                InvoiceId = invoice.Id,
                PatientId = admission.PatientId,
                Amount = admission.PaidAmount,
                PaymentMethod = HIS.Billing.PaymentMethod.Cash, // Simplification for deposits
                Notes = "Applied from Inpatient Deposits"
            });
        }

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
        admission.TotalAmount = admission.AccumulatedRoomCharges + (numberOfDays * room.DailyRate);

        await Repository.UpdateAsync(admission);

        var dto = ObjectMapper.Map<Admission, AdmissionDto>(admission);
        await EnrichAdmissionDtoAsync(dto);
        return dto;
    }

    /// <summary>
    /// نقل المريض من غرفة/سرير إلى آخر
    /// </summary>
    public async Task<AdmissionDto> TransferPatientAsync(Guid id, CreatePatientTransferDto input)
    {
        var admission = await Repository.GetAsync(id);
        if (admission.Status != AdmissionStatus.Active)
        {
            throw new UserFriendlyException("يمكن فقط نقل المرضى المنومين حالياً");
        }

        var oldRoom = await _roomRepository.GetAsync(admission.RoomId);
        var oldBed = await _bedRepository.GetAsync(admission.BedId);

        var newRoom = await _roomRepository.GetAsync(input.ToRoomId);
        var newBedId = input.ToBedId ?? throw new UserFriendlyException("يجب اختيار السرير الجديد");
        var newBed = await _bedRepository.GetAsync(newBedId);

        if (newBed.RoomId != newRoom.Id)
        {
            throw new UserFriendlyException("السرير المختار لا ينتمي للغرفة المحددة");
        }
        if (newBed.Status != BedStatus.Available)
        {
            throw new UserFriendlyException("السرير المختار غير متاح حالياً");
        }

        var transferDate = DateTime.Now;
        int daysInOldRoom = (int)(transferDate - admission.LastTransferDate).TotalDays;
        // if transfer happens same day, we might charge 1 day or 0, let's say 0 to not overcharge if they move instantly
        // but if they stayed overnight, it's 1 day.
        
        decimal chargesForOldRoom = daysInOldRoom * oldRoom.DailyRate;
        
        // Create transfer log
        var transferLog = new PatientTransfer(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            admission.Id,
            oldRoom.Id,
            oldBed.Id,
            newRoom.Id,
            newBed.Id,
            transferDate,
            daysInOldRoom,
            oldRoom.DailyRate,
            chargesForOldRoom
        )
        {
            Reason = input.Reason
        };
        await _patientTransferRepository.InsertAsync(transferLog);

        // Update Admission
        admission.AccumulatedRoomCharges += chargesForOldRoom;
        admission.LastTransferDate = transferDate;
        admission.RoomId = newRoom.Id;
        admission.BedId = newBed.Id;
        await Repository.UpdateAsync(admission);

        // Free old bed
        oldBed.Status = BedStatus.Cleaning;
        await _bedRepository.UpdateAsync(oldBed);
        
        oldRoom.AvailableBeds++;
        if (oldRoom.Status == RoomStatus.Occupied)
        {
            oldRoom.Status = RoomStatus.Available;
        }
        await _roomRepository.UpdateAsync(oldRoom);

        // Occupy new bed
        newBed.Status = BedStatus.Occupied;
        await _bedRepository.UpdateAsync(newBed);

        newRoom.AvailableBeds--;
        if (newRoom.AvailableBeds < 0) newRoom.AvailableBeds = 0;
        if (newRoom.AvailableBeds == 0)
        {
            newRoom.Status = RoomStatus.Occupied;
        }
        await _roomRepository.UpdateAsync(newRoom);

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
