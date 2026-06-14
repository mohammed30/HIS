using System;
using System.Collections.Generic;
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
using HIS.Clinical;
using HIS.Operations;
using HIS.Insurance;

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
    private readonly IRepository<SurgicalOperation, Guid> _surgicalOperationRepository;
    private readonly IRepository<PatientInsurance, Guid> _patientInsuranceRepository;
    private readonly IRepository<InsurancePlan, Guid> _insurancePlanRepository;
    private readonly IRepository<InsuranceServicePrice, Guid> _insurancePriceRepository;
    private readonly IRepository<MedicalOrder, Guid> _medicalOrderRepository;
    private readonly HIS.Billing.IInvoiceAppService _invoiceAppService;
    private readonly IRepository<HIS.Billing.Invoice, Guid> _invoiceRepository;
    private readonly IRepository<HIS.Billing.InvoiceItem, Guid> _invoiceItemRepository;

    public AdmissionAppService(
        IRepository<Admission, Guid> repository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Room, Guid> roomRepository,
        IRepository<Bed, Guid> bedRepository,
        IRepository<HIS.Accounting.Account, Guid> accountRepository,
        IRepository<HIS.Accounting.JournalEntry, Guid> journalEntryRepository,
        IRepository<HIS.Billing.InpatientDeposit, Guid> inpatientDepositRepository,
        IRepository<PatientTransfer, Guid> patientTransferRepository,
        IRepository<SurgicalOperation, Guid> surgicalOperationRepository,
        IRepository<PatientInsurance, Guid> patientInsuranceRepository,
        IRepository<InsurancePlan, Guid> insurancePlanRepository,
        IRepository<InsuranceServicePrice, Guid> insurancePriceRepository,
        IRepository<MedicalOrder, Guid> medicalOrderRepository,
        HIS.Billing.IInvoiceAppService invoiceAppService,
        IRepository<HIS.Billing.Invoice, Guid> invoiceRepository,
        IRepository<HIS.Billing.InvoiceItem, Guid> invoiceItemRepository) : base(repository)
    {
        _patientRepository = patientRepository;
        _roomRepository = roomRepository;
        _bedRepository = bedRepository;
        _accountRepository = accountRepository;
        _journalEntryRepository = journalEntryRepository;
        _inpatientDepositRepository = inpatientDepositRepository;
        _patientTransferRepository = patientTransferRepository;
        _surgicalOperationRepository = surgicalOperationRepository;
        _patientInsuranceRepository = patientInsuranceRepository;
        _insurancePlanRepository = insurancePlanRepository;
        _insurancePriceRepository = insurancePriceRepository;
        _medicalOrderRepository = medicalOrderRepository;
        _invoiceAppService = invoiceAppService;
        _invoiceRepository = invoiceRepository;
        _invoiceItemRepository = invoiceItemRepository;
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
            Notes = input.Notes,
            PatientInsuranceId = input.PatientInsuranceId
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
        arAccount = await GetLeafAccountAsync(arAccount);
        var checkAmount = input.NumberOfDays > 0 ? (input.NumberOfDays * room.DailyRate) : (input.PaidAmount > 0 ? input.PaidAmount : 1000m); // Default fallback

        if (arAccount != null)
        {
            var revenueAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "4100");
            var cashAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1110");
            cashAccount = await GetLeafAccountAsync(cashAccount);

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
                revenueAccount = await GetLeafAccountAsync(revenueAccount);
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
        if (currentStayDays < 1 && admission.AccumulatedRoomCharges == 0) currentStayDays = 1; 
        
        decimal currentStayCharges = currentStayDays * room.DailyRate;
        admission.TotalAmount = admission.AccumulatedRoomCharges + currentStayCharges;
        
        // Handle Medical Orders
        var medicalOrderRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<MedicalOrder, Guid>>();
        var orders = await medicalOrderRepo.GetListAsync(x => x.AdmissionId == admission.Id && x.Status != OrderStatus.Cancelled);
        foreach (var order in orders)
        {
            admission.TotalAmount += (order.Price * order.Quantity);
        }

        // Handle Surgical Operations
        var operations = await _surgicalOperationRepository.GetListAsync(x => x.AdmissionId == admission.Id && x.Status != OperationStatus.Cancelled);
        foreach (var op in operations)
        {
            admission.TotalAmount += op.TotalAmount;
        }

        // --- Insurance Calculation Logic ---
        decimal totalInsuranceShare = 0;
        InsurancePlan? plan = null;
        if (admission.PatientInsuranceId.HasValue)
        {
            var patientInsurance = await _patientInsuranceRepository.FindAsync(admission.PatientInsuranceId.Value);
            if (patientInsurance != null && patientInsurance.Status == PatientInsuranceStatus.Active)
            {
                plan = await _insurancePlanRepository.FindAsync(patientInsurance.InsurancePlanId);
            }
        }
        
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
            string roomServiceCode = GetRoomServiceCode(room.Type);
            decimal dailyRate = await GetServicePriceAsync(admission.PatientInsuranceId, roomServiceCode, room.DailyRate);

            decimal itemAmount = (admission.NumberOfDays * dailyRate) + admission.AccumulatedRoomCharges;
            decimal insuranceShare = (plan != null && plan.IncludesInpatient) ? (itemAmount * (plan.CoveragePercentage / 100)) : 0;
            totalInsuranceShare += insuranceShare;

            invoiceInput.Items.Add(new HIS.Billing.CreateUpdateInvoiceItemDto
            {
                ServiceType = HIS.Billing.ServiceType.Inpatient, 
                Description = $"رسوم إقامة الغرف ({room.RoomNumber}) - {admission.NumberOfDays} يوم",
                UnitPrice = dailyRate,
                Quantity = admission.NumberOfDays,
                IsCoveredByInsurance = insuranceShare > 0,
                DiscountAmount = insuranceShare
            });
        }

        // Add Medical Orders
        foreach (var order in orders)
        {
            decimal itemAmount = order.Price * order.Quantity;
            decimal insuranceShare = 0;
            if (plan != null)
            {
                bool isCovered = (order.Type == OrderType.Lab && plan.IncludesLab) ||
                                (order.Type == OrderType.Radiology && plan.IncludesRadiology) ||
                                ((order.Type == OrderType.Medication || order.Type == OrderType.Consumable) && plan.IncludesMedications);
                insuranceShare = isCovered ? (itemAmount * (plan.CoveragePercentage / 100)) : 0;
            }
            totalInsuranceShare += insuranceShare;

            invoiceInput.Items.Add(new HIS.Billing.CreateUpdateInvoiceItemDto
            {
                ServiceType = order.Type == OrderType.Lab ? HIS.Billing.ServiceType.Laboratory :
                              order.Type == OrderType.Radiology ? HIS.Billing.ServiceType.Radiology :
                              order.Type == OrderType.Consumable ? HIS.Billing.ServiceType.Consumables : 
                              HIS.Billing.ServiceType.Other, 
                Description = order.ServiceName ?? "خدمة طبية",
                UnitPrice = order.Price,
                Quantity = order.Quantity,
                IsCoveredByInsurance = insuranceShare > 0,
                DiscountAmount = insuranceShare
            });
        }

        // Add Surgical Operations
        foreach (var op in operations)
        {
            decimal itemAmount = op.TotalAmount;
            decimal insuranceShare = (plan != null && plan.IncludesInpatient) ? (itemAmount * (plan.CoveragePercentage / 100)) : 0;
            totalInsuranceShare += insuranceShare;

            invoiceInput.Items.Add(new HIS.Billing.CreateUpdateInvoiceItemDto
            {
                ServiceType = HIS.Billing.ServiceType.Surgical,
                Description = op.OperationName ?? "عملية جراحية",
                UnitPrice = op.TotalAmount,
                Quantity = 1,
                IsCoveredByInsurance = insuranceShare > 0,
                DiscountAmount = insuranceShare
            });
        }

        // --- Add Existing Items from ALL Draft Invoices for this Patient (Current Stay) ---
        var draftInvoices = await _invoiceRepository.GetListAsync(x => 
            x.PatientId == admission.PatientId && 
            x.Status == HIS.Billing.InvoiceStatus.Draft &&
            x.InvoiceDate >= admission.AdmissionDate);
        foreach (var draftInvoice in draftInvoices)
        {
            var existingItems = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == draftInvoice.Id);
            foreach (var item in existingItems)
            {
                if (invoiceInput.Items.Any(x => x.Description == item.Description && x.Quantity == item.Quantity && x.UnitPrice == item.UnitPrice))
                {
                    continue;
                }

                invoiceInput.Items.Add(new HIS.Billing.CreateUpdateInvoiceItemDto
                {
                    ServiceType = item.ServiceType,
                    Description = item.Description,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    ServiceCode = item.ServiceCode,
                    Notes = item.Notes,
                    IsCoveredByInsurance = item.IsCoveredByInsurance,
                    DiscountAmount = item.DiscountAmount
                });
            }
        }

        admission.InsuranceAmount = totalInsuranceShare;

        // Cancel ALL draft invoices for this patient
        foreach (var oldInvoice in draftInvoices)
        {
            oldInvoice.Status = HIS.Billing.InvoiceStatus.Cancelled;
            oldInvoice.Notes += " - Consolidated into final discharge invoice";
            await _invoiceRepository.UpdateAsync(oldInvoice);
        }

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
                PaymentMethod = HIS.Billing.PaymentMethod.Cash,
                Notes = "Applied from Inpatient Deposits"
            });
        }

        await Repository.UpdateAsync(admission);

        room.AvailableBeds++; 
        if (room.Status == RoomStatus.Occupied) room.Status = RoomStatus.Available;
        await _roomRepository.UpdateAsync(room);

        var bed = await _bedRepository.GetAsync(admission.BedId);
        bed.Status = BedStatus.Cleaning;
        await _bedRepository.UpdateAsync(bed);

        var dto = ObjectMapper.Map<Admission, AdmissionDto>(admission);
        await EnrichAdmissionDtoAsync(dto);
        return dto;
    }

    /// <summary>
    /// عرض الفاتورة المبدئية
    /// </summary>
    public async Task<HIS.Billing.InvoiceDto> GetProvisionalInvoiceAsync(Guid id)
    {
        var admission = await Repository.GetAsync(id);
        var patient = await _patientRepository.GetAsync(admission.PatientId);
        var room = await _roomRepository.GetAsync(admission.RoomId);

        var invoiceDto = new HIS.Billing.InvoiceDto
        {
            PatientId = admission.PatientId,
            PatientName = !string.IsNullOrWhiteSpace(patient.FullNameAr) ? patient.FullNameAr : patient.MRN,
            InvoiceDate = DateTime.Now,
            Status = HIS.Billing.InvoiceStatus.Draft,
            Items = new System.Collections.Generic.List<HIS.Billing.InvoiceItemDto>()
        };

        // 1. Room Charges
        int currentStayDays = (int)(DateTime.Now - admission.LastTransferDate).TotalDays;
        if (currentStayDays < 1 && admission.AccumulatedRoomCharges == 0) currentStayDays = 1;
        
        string roomServiceCode = GetRoomServiceCode(room.Type);
        decimal dailyRate = await GetServicePriceAsync(admission.PatientInsuranceId, roomServiceCode, room.DailyRate);
        
        decimal currentStayCharges = currentStayDays * dailyRate;
        decimal totalRoomCharges = admission.AccumulatedRoomCharges + currentStayCharges;

        decimal totalInsuranceShare = 0;
        InsurancePlan? plan = null;
        if (admission.PatientInsuranceId.HasValue)
        {
            var patientInsurance = await _patientInsuranceRepository.FindAsync(admission.PatientInsuranceId.Value);
            if (patientInsurance != null && patientInsurance.Status == PatientInsuranceStatus.Active)
            {
                plan = await _insurancePlanRepository.FindAsync(patientInsurance.InsurancePlanId);
            }
        }

        if (totalRoomCharges > 0)
        {
            decimal insuranceShare = (plan != null && plan.IncludesInpatient) ? (totalRoomCharges * (plan.CoveragePercentage / 100)) : 0;
            totalInsuranceShare += insuranceShare;

            invoiceDto.Items.Add(new HIS.Billing.InvoiceItemDto
            {
                ServiceType = HIS.Billing.ServiceType.Inpatient, 
                Description = $"رسوم إقامة الغرف ({room.RoomNumber}) - حتى تاريخه",
                Quantity = currentStayDays > 0 ? currentStayDays : 1, 
                UnitPrice = dailyRate,
                TotalPrice = totalRoomCharges,
                IsCoveredByInsurance = insuranceShare > 0,
                DiscountAmount = insuranceShare
            });
            invoiceDto.TotalAmount += totalRoomCharges;
            invoiceDto.NetAmount += (totalRoomCharges - insuranceShare);
        }

        // 2. Medical Orders
        var orders = await _medicalOrderRepository.GetListAsync(x => x.AdmissionId == id && x.Status != OrderStatus.Cancelled);
        foreach (var order in orders)
        {
            decimal itemTotal = order.Price * order.Quantity;
            decimal insuranceShare = 0;
            if (plan != null)
            {
                bool isCovered = (order.Type == OrderType.Lab && plan.IncludesLab) ||
                                (order.Type == OrderType.Radiology && plan.IncludesRadiology) ||
                                ((order.Type == OrderType.Medication || order.Type == OrderType.Consumable) && plan.IncludesMedications);
                insuranceShare = isCovered ? (itemTotal * (plan.CoveragePercentage / 100)) : 0;
            }
            totalInsuranceShare += insuranceShare;

            invoiceDto.Items.Add(new HIS.Billing.InvoiceItemDto
            {
                ServiceType = order.Type == OrderType.Lab ? HIS.Billing.ServiceType.Laboratory :
                              order.Type == OrderType.Radiology ? HIS.Billing.ServiceType.Radiology :
                              order.Type == OrderType.Consumable ? HIS.Billing.ServiceType.Consumables : 
                              HIS.Billing.ServiceType.Other, 
                Description = order.ServiceName ?? "خدمة طبية",
                Quantity = order.Quantity,
                UnitPrice = order.Price,
                TotalPrice = itemTotal,
                IsCoveredByInsurance = insuranceShare > 0,
                DiscountAmount = insuranceShare
            });
            invoiceDto.TotalAmount += itemTotal;
            invoiceDto.NetAmount += (itemTotal - insuranceShare);
        }

        // 3. Surgical Operations
        var operations = await _surgicalOperationRepository.GetListAsync(x => x.AdmissionId == id && x.Status != OperationStatus.Cancelled);
        foreach (var op in operations)
        {
            decimal itemTotal = op.TotalAmount;
            decimal insuranceShare = (plan != null && plan.IncludesInpatient) ? (itemTotal * (plan.CoveragePercentage / 100)) : 0;
            totalInsuranceShare += insuranceShare;

            invoiceDto.Items.Add(new HIS.Billing.InvoiceItemDto
            {
                ServiceType = HIS.Billing.ServiceType.Surgical,
                Description = op.OperationName ?? "عملية جراحية",
                Quantity = 1,
                UnitPrice = itemTotal,
                TotalPrice = itemTotal,
                IsCoveredByInsurance = insuranceShare > 0,
                DiscountAmount = insuranceShare
            });
            invoiceDto.TotalAmount += itemTotal;
            invoiceDto.NetAmount += (itemTotal - insuranceShare);
        }

        // 4. ALL Draft Invoices for this Patient (Current Stay)
        var draftInvoices = await _invoiceRepository.GetListAsync(x => 
            x.PatientId == admission.PatientId && 
            x.Status == HIS.Billing.InvoiceStatus.Draft &&
            x.InvoiceDate >= admission.AdmissionDate);
        foreach (var draftInvoice in draftInvoices)
        {
            var existingItems = await _invoiceItemRepository.GetListAsync(x => x.InvoiceId == draftInvoice.Id);
            foreach (var item in existingItems)
            {
                if (invoiceDto.Items.Any(i => i.Description == item.Description && i.Quantity == item.Quantity && i.UnitPrice == item.UnitPrice)) continue;

                invoiceDto.Items.Add(new HIS.Billing.InvoiceItemDto
                {
                    ServiceType = item.ServiceType,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    IsCoveredByInsurance = item.IsCoveredByInsurance,
                    DiscountAmount = item.DiscountAmount
                });
                invoiceDto.TotalAmount += item.TotalPrice;
                invoiceDto.NetAmount += (item.TotalPrice - item.DiscountAmount);
            }
        }

        // 5. Deposits
        var activeDeposits = await _inpatientDepositRepository.GetListAsync(d => d.AdmissionId == id && d.Status == HIS.Billing.DepositStatus.Active);
        decimal totalDeposits = activeDeposits.Sum(d => d.Amount);
        
        invoiceDto.PaidAmount = admission.PaidAmount + totalDeposits;
        invoiceDto.DueAmount = invoiceDto.NetAmount - invoiceDto.PaidAmount;
        if (invoiceDto.DueAmount < 0) invoiceDto.DueAmount = 0;

        return invoiceDto;
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
        
        decimal chargesForOldRoom = daysInOldRoom * oldRoom.DailyRate;
        
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

        admission.AccumulatedRoomCharges += chargesForOldRoom;
        admission.LastTransferDate = transferDate;
        admission.RoomId = newRoom.Id;
        admission.BedId = newBed.Id;
        await Repository.UpdateAsync(admission);

        oldBed.Status = BedStatus.Cleaning;
        await _bedRepository.UpdateAsync(oldBed);
        
        oldRoom.AvailableBeds++;
        if (oldRoom.Status == RoomStatus.Occupied) oldRoom.Status = RoomStatus.Available;
        await _roomRepository.UpdateAsync(oldRoom);

        newBed.Status = BedStatus.Occupied;
        await _bedRepository.UpdateAsync(newBed);

        newRoom.AvailableBeds--;
        if (newRoom.AvailableBeds < 0) newRoom.AvailableBeds = 0;
        if (newRoom.AvailableBeds == 0) newRoom.Status = RoomStatus.Occupied;
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

    private string GetRoomServiceCode(RoomType type)
    {
        return type switch
        {
            RoomType.Standard => "ROOM-STD",
            RoomType.Private => "ROOM-PRV",
            RoomType.ICU => "ROOM-ICU",
            RoomType.Suite => "ROOM-SUI",
            RoomType.Isolation => "ROOM-ISO",
            _ => "ROOM-STD"
        };
    }

    private async Task<decimal> GetServicePriceAsync(Guid? patientInsuranceId, string serviceCode, decimal defaultPrice)
    {
        if (!patientInsuranceId.HasValue) return defaultPrice;

        var patientInsurance = await _patientInsuranceRepository.FindAsync(patientInsuranceId.Value);
        if (patientInsurance == null) return defaultPrice;

        var serviceItemRepo = LazyServiceProvider.LazyGetRequiredService<IRepository<HIS.Services.ServiceItem, Guid>>();
        var serviceItem = await serviceItemRepo.FirstOrDefaultAsync(s => s.Code == serviceCode);
        if (serviceItem == null) return defaultPrice;

        var customPrice = await _insurancePriceRepository.FirstOrDefaultAsync(x => 
            x.InsurancePlanId == patientInsurance.InsurancePlanId && 
            x.ServiceItemId == serviceItem.Id);

        return customPrice != null ? customPrice.CustomPrice : defaultPrice;
    }

    public async Task<List<AdmissionLookupDto>> GetActiveAdmissionsLookupAsync()
    {
        var admissions = await Repository.GetListAsync(x => x.Status == AdmissionStatus.Active);
        var patientIds = admissions.Select(x => x.PatientId).Distinct().ToList();
        var patients = await _patientRepository.GetListAsync(x => patientIds.Contains(x.Id));

        var lookup = new List<AdmissionLookupDto>();
        foreach (var admission in admissions)
        {
            var patient = patients.FirstOrDefault(x => x.Id == admission.PatientId);
            lookup.Add(new AdmissionLookupDto
            {
                Id = admission.Id,
                DisplayName = patient?.FullNameAr ?? patient?.MRN ?? "مريض مجهول"
            });
        }
        return lookup;
    }

    private async Task<HIS.Accounting.Account> GetLeafAccountAsync(HIS.Accounting.Account account)
    {
        if (account == null) return null;

        var hasChildren = await _accountRepository.AnyAsync(x => x.ParentId == account.Id);
        if (!hasChildren)
        {
            return account;
        }

        var children = await _accountRepository.GetListAsync(x => x.ParentId == account.Id);
        if (!children.Any())
        {
            return account;
        }

        foreach (var child in children.OrderBy(x => x.Code))
        {
            var leaf = await GetLeafAccountAsync(child);
            if (leaf != null)
            {
                return leaf;
            }
        }

        return account;
    }
}
