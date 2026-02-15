using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
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
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SurgicalOperationAppService(
        IRepository<SurgicalOperation, Guid> repository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Doctor, Guid> doctorRepository,
        IInvoiceAppService invoiceAppService,
        AccountingManager accountingManager,
        IRepository<Account, Guid> accountRepository,
        IWebHostEnvironment webHostEnvironment) : base(repository)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _invoiceAppService = invoiceAppService;
        _accountingManager = accountingManager;
        _accountRepository = accountRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    public override async Task<SurgicalOperationDto> CreateAsync(CreateUpdateSurgicalOperationDto input)
    {
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
            Status = input.Status,
            AdmissionId = input.AdmissionId,
            Notes = input.Notes
        };

        // 2. Create Invoice (Auto-Invoice)
        if (input.TotalAmount > 0)
        {
            var invoiceInput = new CreateUpdateInvoiceDto
            {
                PatientId = input.PatientId,
                DueDate = input.OperationDate, // Due immediately or as per policy
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

            // Using CreateAsync of InvoiceService to handle logic
            var invoice = await _invoiceAppService.CreateAsync(invoiceInput);
            operation.InvoiceId = invoice.Id;

            // 3. Create Journal Entry (Accrual Basis)
             // Debit: Patient Receivables (or Cash if paid immediately, but here it's invoice first)
             // Credit: Surgery Revenue
             
             // Get Accounts (Hardcoded or Settings-based - specific implementation detail)
             // For MVP, we'll try to find by Name or use placeholders if not found.
             // Ideally this should be configuration driven.
             
             var receivablesAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Name == "Accounts Receivable" || x.Name == "المدينون");
             var revenueAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Name == "Surgery Revenue" || x.Name == "إيرادات العمليات");

             if (receivablesAccount != null && revenueAccount != null)
             {
                 var patient = await _patientRepository.FindAsync(input.PatientId);
                 var patientName = patient?.FullNameAr ?? "Patient";
                 
                 var je = await _accountingManager.CreateEntryAsync(
                     input.OperationDate, 
                     $"INV-{invoice.InvoiceNumber}", 
                     $"فاتورة جراحة: {input.OperationName} - {patientName}");

                 je.AddLine(GuidGenerator, receivablesAccount.Id, input.TotalAmount, 0);
                 je.AddLine(GuidGenerator, revenueAccount.Id, 0, input.TotalAmount);
                 
                 // Auto-post for now
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

        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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
        return new Volo.Abp.Content.RemoteStreamContent(ms, $"Surgery_Ticket_{operation.Id}.pdf", "application/pdf");
    }

    protected override async Task<IQueryable<SurgicalOperation>> CreateFilteredQueryAsync(GetSurgicalOperationsInput input)
    {
        var queryable = await base.CreateFilteredQueryAsync(input);

        return queryable
            .WhereIf(!string.IsNullOrWhiteSpace(input.SearchText),
                x => x.OperationName.Contains(input.SearchText!))
            .WhereIf(input.PatientId.HasValue, x => x.PatientId == input.PatientId!.Value)
            .WhereIf(input.DoctorId.HasValue, x => x.DoctorId == input.DoctorId!.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value)
            .WhereIf(input.FromDate.HasValue, x => x.OperationDate >= input.FromDate!.Value)
            .WhereIf(input.ToDate.HasValue, x => x.OperationDate <= input.ToDate!.Value);
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
            }
        }
    }
}
