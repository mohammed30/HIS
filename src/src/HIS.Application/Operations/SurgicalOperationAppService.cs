using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using HIS.Permissions;
using HIS.Patients;
using HIS.Settings;
using HIS.Billing; // For Invoice
using HIS.Accounting; // For AccountingManager
using Microsoft.AspNetCore.Hosting; // For Ticket Logo
using System.IO;
using HIS.Operations.Printing; // For TicketDocument
using QuestPDF.Fluent; // For GeneratePdf
using Microsoft.AspNetCore.Mvc;
using HIS.Services; // For ServiceItem
using HIS.Inpatient; // For Admission

namespace HIS.Operations;

/// <summary>
/// خدمة العمليات الجراحية
/// </summary>
[Authorize(HISPermissions.Operations.Default)]
public class SurgicalOperationAppService : CrudAppService<
    SurgicalOperation,
    SurgicalOperationDto,
    Guid,
    GetSurgicalOperationsInput,
    CreateUpdateSurgicalOperationDto>, ISurgicalOperationAppService
{
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IInvoiceAppService _invoiceAppService;
    private readonly AccountingManager _accountingManager;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<Specialty, Guid> _specialtyRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IRepository<Admission, Guid> _admissionRepository;

    public SurgicalOperationAppService(
        IRepository<SurgicalOperation, Guid> repository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IInvoiceAppService invoiceAppService,
        AccountingManager accountingManager,
        IRepository<Account, Guid> accountRepository,
        IRepository<Specialty, Guid> specialtyRepository,
        IRepository<ServiceItem, Guid> serviceItemRepository,
        IWebHostEnvironment webHostEnvironment,
        IRepository<Admission, Guid> admissionRepository) : base(repository)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _invoiceAppService = invoiceAppService;
        _accountingManager = accountingManager;
        _accountRepository = accountRepository;
        _specialtyRepository = specialtyRepository;
        _serviceItemRepository = serviceItemRepository;
        _webHostEnvironment = webHostEnvironment;
        _admissionRepository = admissionRepository;
    }

    public override async Task<SurgicalOperationDto> CreateAsync(CreateUpdateSurgicalOperationDto input)
    {
        // Calculate fees
        decimal surgeonFeeAmount = input.SurgeonFeePercentage > 0 
            ? input.TotalAmount * (input.SurgeonFeePercentage / 100m)
            : input.SurgeonFeeAmount;
        
        decimal anesthFeeAmount = input.AnesthesiologistFeePercentage > 0
            ? input.TotalAmount * (input.AnesthesiologistFeePercentage / 100m)
            : input.AnesthesiologistFeeAmount;
            
        decimal hospitalShare = input.TotalAmount - surgeonFeeAmount - anesthFeeAmount;
        if (hospitalShare < 0) hospitalShare = 0;

        // 1. Create Operation
        var operation = new SurgicalOperation(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            input.PatientId,
            input.OperationName,
            input.OperationDate
        )
        {
            DoctorId = input.DoctorId,
            OperationTypeId = input.OperationTypeId,
            Details = input.Details,
            TotalAmount = input.TotalAmount,
            CompanyShare = input.CompanyShare,
            PatientShare = input.PatientShare,
            InsuranceTotal = input.TotalAmount - input.PatientShare,
            SurgeonFeePercentage = input.SurgeonFeePercentage,
            SurgeonFeeAmount = surgeonFeeAmount,
            AnesthesiologistId = input.AnesthesiologistId,
            AnesthesiologistFeePercentage = input.AnesthesiologistFeePercentage,
            AnesthesiologistFeeAmount = anesthFeeAmount,
            HospitalShareAmount = hospitalShare,
            Status = input.Status,
            AdmissionId = input.AdmissionId,
            Notes = input.Notes
        };

        // 2. Create Invoice & Admissions update
        if (input.TotalAmount > 0)
        {
            var patient = await _patientRepository.FindAsync(input.PatientId);
            var patientName = patient?.FullNameAr ?? "Patient";
            
            if (input.AdmissionId.HasValue)
            {
                // Inpatient Operation - Add to Admission Total
                var admission = await _admissionRepository.GetAsync(input.AdmissionId.Value);
                admission.TotalAmount += input.TotalAmount;
                await _admissionRepository.UpdateAsync(admission);
            }
            else
            {
                // Outpatient Operation - Create Invoice
                var invoiceInput = new CreateUpdateInvoiceDto
                {
                    PatientId = input.PatientId,
                    DueDate = input.OperationDate,
                    Notes = $"Auto-generated invoice for Surgery: {input.OperationName}",
                    Items = new System.Collections.Generic.List<CreateUpdateInvoiceItemDto>
                    {
                        new CreateUpdateInvoiceItemDto
                        {
                             ServiceType = ServiceType.Surgery,
                             Description = input.OperationName,
                             UnitPrice = input.TotalAmount,
                             Quantity = 1,
                             IsCoveredByInsurance = input.CompanyShare > 0
                        }
                    }
                };

                var invoice = await _invoiceAppService.CreateAsync(invoiceInput);
                operation.InvoiceId = invoice.Id;
            }

            // 3. Create Journal Entries (Revenue & Doctor Entitlements)
            var receivablesAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Name == "Accounts Receivable" || x.Name == "المدينون");
            var revenueAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Name == "Surgery Revenue" || x.Name == "إيرادات العمليات");
            var doctorExpenseAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Name == "Doctor Expenses" || x.Name == "مصروفات الأطباء");
            var doctorPayableAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Name == "Doctor Payables" || x.Name == "ذمم الأطباء الدائنة");

            if (receivablesAccount != null && revenueAccount != null)
            {
                var je = await _accountingManager.CreateEntryAsync(
                    input.OperationDate, 
                    $"OPR-{operation.Id.ToString().Substring(0,8).ToUpper()}", 
                    $"رسوم جراحة: {input.OperationName} - {patientName}");

                // Patient Billing (Revenue)
                je.AddLine(GuidGenerator, receivablesAccount.Id, input.TotalAmount, 0);
                je.AddLine(GuidGenerator, revenueAccount.Id, 0, input.TotalAmount);
                
                // Doctor Entitlements
                decimal totalDoctorFees = surgeonFeeAmount + anesthFeeAmount;
                if (totalDoctorFees > 0 && doctorExpenseAccount != null && doctorPayableAccount != null)
                {
                    je.AddLine(GuidGenerator, doctorExpenseAccount.Id, totalDoctorFees, 0);
                    if (surgeonFeeAmount > 0) je.AddLine(GuidGenerator, doctorPayableAccount.Id, 0, surgeonFeeAmount);
                    if (anesthFeeAmount > 0) je.AddLine(GuidGenerator, doctorPayableAccount.Id, 0, anesthFeeAmount);
                }
                 
                await _accountingManager.PostEntryAsync(je);
            }
        }

        await Repository.InsertAsync(operation);

        var dto = ObjectMapper.Map<SurgicalOperation, SurgicalOperationDto>(operation);
        await EnrichOperationDtoAsync(dto);
        return dto;
    }

    /// <summary>
    /// تحديث حالة العملية
    /// </summary>
    public async Task<SurgicalOperationDto> UpdateStatusAsync(Guid id, OperationStatus status)
    {
        var operation = await Repository.GetAsync(id);
        
        // If status changes to Cancelled, trigger Invoice Cancellation if exists
        if (status == OperationStatus.Cancelled && operation.Status != OperationStatus.Cancelled && operation.InvoiceId.HasValue)
        {
            try
            {
                await _invoiceAppService.CancelAsync(operation.InvoiceId.Value);
            }
            catch (Exception ex)
            {
                // Log or handle if invoice is already cancelled or other issues
            }
        }

        operation.Status = status;
        await Repository.UpdateAsync(operation);

        var dto = ObjectMapper.Map<SurgicalOperation, SurgicalOperationDto>(operation);
        await EnrichOperationDtoAsync(dto);
        return dto;
    }
    
    [HttpGet]
    [Route("api/app/surgical-operation/ticket-pdf/{id}")]
    [Authorize(HISPermissions.Operations.PrintTicket)]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetOperationTicketPdfAsync(Guid id)
    {
        var operation = await Repository.GetAsync(id);
        var patient = await _patientRepository.GetAsync(operation.PatientId);
        var doctor = operation.DoctorId.HasValue ? await _doctorRepository.GetAsync(operation.DoctorId.Value) : null;
        
        // Logo
        byte[] logoBytes = null;
        var logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "logo", "Dark.png");
        if (File.Exists(logoPath)) logoBytes = await File.ReadAllBytesAsync(logoPath);

        var document = new OperationTicketDocument
        {
            TicketNumber = operation.Id.ToString().Substring(0, 8).ToUpper(), // Or use a sequence
            Date = operation.OperationDate,
            PatientName = patient.FullNameAr ?? patient.FullNameEn,
            PatientFileNumber = patient.MRN,
            OperationName = operation.OperationName,
            DoctorName = doctor != null ? (doctor.NameAr ?? doctor.NameEn) : "-",
            AnesthesiaType = "General (Standard)", // Placeholder or add to entity
            Amount = operation.TotalAmount,
            UserName = CurrentUser.Name ?? "admin",
            LogoBytes = logoBytes
        };
        
        var pdfBytes = document.GeneratePdf();
        var ms = new MemoryStream(pdfBytes);
        var printTime = Clock.Now;
        return new Volo.Abp.Content.RemoteStreamContent(ms, $"تذكرة_عملية_{printTime:yyyy-MM-dd_HH-mm-ss}.pdf", "application/pdf");
    }

    [HttpGet]
    [Route("api/app/surgical-operation/report-pdf")]
    [Authorize(HISPermissions.Operations.Report)]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetOperationsReportPdfAsync(GetSurgicalOperationsInput input)
    {
        input.MaxResultCount = 1000; // Limit for report
        var operations = await GetListAsync(input);
        
        // Logo
        byte[] logoBytes = null;
        var logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "logo", "Dark.png");
        if (File.Exists(logoPath)) logoBytes = await File.ReadAllBytesAsync(logoPath);

        var document = new OperationsReportDocument
        {
            FromDate = input.FromDate,
            ToDate = input.ToDate,
            Operations = operations.Items.ToList(),
            LogoBytes = logoBytes,
            UserName = CurrentUser.Name ?? "admin"
        };
        
        var pdfBytes = document.GeneratePdf();
        var ms = new MemoryStream(pdfBytes);
        var printTime = Clock.Now;
        return new Volo.Abp.Content.RemoteStreamContent(ms, $"تقرير_العمليات_{printTime:yyyy-MM-dd}.pdf", "application/pdf");
    }

    protected override async Task<IQueryable<SurgicalOperation>> CreateFilteredQueryAsync(GetSurgicalOperationsInput input)
    {
        var queryable = await base.CreateFilteredQueryAsync(input);

        if (input.SpecialtyId.HasValue)
        {
            var doctors = await _doctorRepository.GetQueryableAsync();
            queryable = from op in queryable
                        join dr in doctors on op.DoctorId equals dr.Id
                        where dr.SpecialtyId == input.SpecialtyId.Value
                        select op;
        }

        return queryable
            .WhereIf(!string.IsNullOrWhiteSpace(input.SearchText),
                x => x.OperationName.Contains(input.SearchText!))
            .WhereIf(input.PatientId.HasValue, x => x.PatientId == input.PatientId!.Value)
            .WhereIf(input.DoctorId.HasValue, x => x.DoctorId == input.DoctorId!.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value)
            .WhereIf(input.FromDate.HasValue, x => x.OperationDate >= input.FromDate!.Value)
            .WhereIf(input.ToDate.HasValue, x => x.OperationDate < input.ToDate!.Value.Date.AddDays(1));
    }

    protected override IQueryable<SurgicalOperation> ApplyDefaultSorting(IQueryable<SurgicalOperation> query)
    {
        return query.OrderByDescending(x => x.OperationDate);
    }

    public override async Task<SurgicalOperationDto> GetAsync(Guid id)
    {
        var dto = await base.GetAsync(id);
        await EnrichOperationDtoAsync(dto);
        return dto;
    }

    public override async Task<Volo.Abp.Application.Dtos.PagedResultDto<SurgicalOperationDto>> GetListAsync(GetSurgicalOperationsInput input)
    {
        var result = await base.GetListAsync(input);
        if (result.Items != null && result.Items.Any())
        {
            await EnrichOperationDtosAsync(result.Items);
        }
        return result;
    }

    private async Task EnrichOperationDtosAsync(IReadOnlyList<SurgicalOperationDto> dtos)
    {
        var patientIds = dtos.Select(x => x.PatientId).Distinct().ToList();
        var doctorIds = dtos.Where(x => x.DoctorId.HasValue).Select(x => x.DoctorId.Value).Distinct().ToList();
        var operationTypeIds = dtos.Where(x => x.OperationTypeId.HasValue).Select(x => x.OperationTypeId.Value).Distinct().ToList();

        var patients = (await _patientRepository.GetListAsync(x => patientIds.Contains(x.Id)))
            .ToDictionary(x => x.Id);

        var doctors = (await _doctorRepository.GetListAsync(x => doctorIds.Contains(x.Id)))
            .ToDictionary(x => x.Id);

        var serviceItems = (await _serviceItemRepository.GetListAsync(x => operationTypeIds.Contains(x.Id)))
            .ToDictionary(x => x.Id);

        var specialtyIds = doctors.Values.Select(x => x.SpecialtyId).Distinct().ToList();
        var specialties = (await _specialtyRepository.GetListAsync(x => specialtyIds.Contains(x.Id)))
            .ToDictionary(x => x.Id);

        foreach (var dto in dtos)
        {
            if (patients.TryGetValue(dto.PatientId, out var patient))
            {
                dto.PatientName = patient.FullNameAr;
            }

            if (dto.DoctorId.HasValue && doctors.TryGetValue(dto.DoctorId.Value, out var doctor))
            {
                dto.DoctorName = doctor.NameAr ?? doctor.NameEn;
                if (specialties.TryGetValue(doctor.SpecialtyId, out var specialty))
                {
                    dto.SpecialtyName = specialty.NameAr ?? specialty.NameEn;
                }
            }

            if (string.IsNullOrWhiteSpace(dto.OperationName) && dto.OperationTypeId.HasValue && serviceItems.TryGetValue(dto.OperationTypeId.Value, out var serviceItem))
            {
                dto.OperationName = serviceItem.Name;
            }
        }
    }

    private async Task EnrichOperationDtoAsync(SurgicalOperationDto dto)
    {
        var patient = await _patientRepository.FindAsync(dto.PatientId);
        if (patient != null)
        {
            dto.PatientName = patient.FullNameAr;
        }

        if (dto.DoctorId.HasValue)
        {
            var doctor = await _doctorRepository.FindAsync(dto.DoctorId.Value);
            if (doctor != null)
            {
                dto.DoctorName = doctor.NameAr ?? doctor.NameEn;
                var specialty = await _specialtyRepository.FindAsync(doctor.SpecialtyId);
                if (specialty != null)
                {
                    dto.SpecialtyName = specialty.NameAr ?? specialty.NameEn;
                }
            }
        }

        if (string.IsNullOrWhiteSpace(dto.OperationName) && dto.OperationTypeId.HasValue)
        {
            var serviceItem = await _serviceItemRepository.FindAsync(dto.OperationTypeId.Value);
            if (serviceItem != null)
            {
                dto.OperationName = serviceItem.Name;
            }
        }
    }
}
