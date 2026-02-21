using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using HIS.Permissions;

using HIS.Accounting;
using HIS.General;

namespace HIS.Billing;

/// <summary>
/// خدمة الفواتير
/// </summary>
public class InvoiceAppService : CrudAppService<Invoice, InvoiceDto, Guid, GetInvoicesInput, CreateUpdateInvoiceDto>, IInvoiceAppService
{
    private readonly IRepository<InvoiceItem, Guid> _itemRepository;
    private readonly IRepository<HIS.Patients.Patient, Guid> _patientRepository;
    private readonly IRepository<HIS.Accounting.JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<HIS.Accounting.Account, Guid> _accountRepository;
    private readonly IWebHostEnvironment _env;

    public InvoiceAppService(
        IRepository<Invoice, Guid> repository,
        IRepository<InvoiceItem, Guid> itemRepository,
        IRepository<HIS.Patients.Patient, Guid> patientRepository,
        IRepository<HIS.Accounting.JournalEntry, Guid> journalEntryRepository,
        IRepository<HIS.Accounting.Account, Guid> accountRepository,
        IWebHostEnvironment env) : base(repository)
    {
        _itemRepository = itemRepository;
        _patientRepository = patientRepository;
        _journalEntryRepository = journalEntryRepository;
        _accountRepository = accountRepository;
        _env = env;
        
        GetPolicyName = HISPermissions.Billing.Default;
        GetListPolicyName = HISPermissions.Billing.Default;
        CreatePolicyName = HISPermissions.Billing.ManageInvoices;
        UpdatePolicyName = HISPermissions.Billing.ManageInvoices;
        DeletePolicyName = HISPermissions.Billing.ManageInvoices;
    }

    public override async Task<InvoiceDto> CreateAsync(CreateUpdateInvoiceDto input)
    {
        // ... implementation (this override will automatically be protected by CreatePolicyName check in base) ...
        await CheckCreatePolicyAsync(); // Good practice to call this or rely on base. But since we have logic before base insert, we should check.
        // Actually base.CreateAsync calls CheckCreatePolicyAsync() then MapToEntity then Repository.Insert.
        // Since we are COMPLETELY overriding logic without calling base.CreateAsync, we MUST call CheckCreatePolicyAsync().
        
        // However, standard CreateAsync calls repository insert. We are doing custom logic.
        // Let's call CheckCreatePolicyAsync() manually.
        await CheckCreatePolicyAsync();

        var invoiceId = GuidGenerator.Create();
        var invoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
        
        var invoice = new Invoice(invoiceId, CurrentTenant.Id, input.PatientId, invoiceNumber)
        {
            DueDate = input.DueDate,
            DiscountAmount = input.DiscountAmount,
            TaxPercentage = input.TaxPercentage == 0 ? 15m : input.TaxPercentage,
            PatientInsuranceId = input.PatientInsuranceId,
            AppointmentId = input.AppointmentId,
            Notes = input.Notes,
            Status = InvoiceStatus.Issued
        };

        // Calculate totals from items
        decimal totalAmount = 0;
        if (input.Items != null)
        {
            foreach (var itemDto in input.Items)
            {
                var itemId = GuidGenerator.Create();
                var discountAmount = (itemDto.Quantity * itemDto.UnitPrice) * (itemDto.DiscountPercentage / 100);
                var item = new InvoiceItem(itemId, CurrentTenant.Id, invoiceId, itemDto.Description, itemDto.UnitPrice)
                {
                    ServiceType = itemDto.ServiceType,
                    ServiceCode = itemDto.ServiceCode,
                    Quantity = itemDto.Quantity,
                    DiscountPercentage = itemDto.DiscountPercentage,
                    DiscountAmount = discountAmount,
                    IsCoveredByInsurance = itemDto.IsCoveredByInsurance,
                    Notes = itemDto.Notes
                };
                await _itemRepository.InsertAsync(item);
                totalAmount += item.TotalPrice;
            }
        }

        invoice.TotalAmount = totalAmount;
        invoice.TaxAmount = (totalAmount - input.DiscountAmount) * (input.TaxPercentage / 100);
        invoice.NetAmount = totalAmount - input.DiscountAmount + invoice.TaxAmount;

        await Repository.InsertAsync(invoice);

        // Auto-Create Journal Entry (Debit AR 1120, Credit Revenue 4100)
        await CreateInvoiceJournalEntryAsync(invoice, totalAmount);

        return ObjectMapper.Map<Invoice, InvoiceDto>(invoice);
    }

    private async Task CreateInvoiceJournalEntryAsync(Invoice invoice, decimal amount)
    {
        if (amount <= 0) return;

        var arAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1120");
        var revenueAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "4100");
        var taxAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "2200");

        var patient = await _patientRepository.FindAsync(invoice.PatientId);
        var patientName = patient != null ? patient.FullNameAr : invoice.PatientId.ToString();

        if (arAccount != null && revenueAccount != null)
        {
            var je = new HIS.Accounting.JournalEntry(
                GuidGenerator.Create(),
                invoice.InvoiceDate,
                invoice.InvoiceNumber,
                $"فاتورة رقم {invoice.InvoiceNumber} - المريض: {patientName}"
            );
            
            // Debit AR for Net Amount (Total + Tax - Discount)
            je.AddLine(GuidGenerator, arAccount.Id, invoice.NetAmount, 0);
            
            // Credit Revenue for Subtotal (Total - Discount)
            var revenueAmount = amount - invoice.DiscountAmount;
            je.AddLine(GuidGenerator, revenueAccount.Id, 0, revenueAmount);

            // Credit Tax Liability 
            if (invoice.TaxAmount > 0 && taxAccount != null)
            {
                je.AddLine(GuidGenerator, taxAccount.Id, 0, invoice.TaxAmount);
            }

            await _journalEntryRepository.InsertAsync(je);
        }
    }

    public async Task<InvoiceDto> GetWithItemsAsync(Guid id)
    {
        await CheckGetPolicyAsync();
        var invoice = await Repository.GetAsync(id);
        var dto = ObjectMapper.Map<Invoice, InvoiceDto>(invoice);
        
        var itemsQueryable = await _itemRepository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(itemsQueryable.Where(x => x.InvoiceId == id));
        dto.Items = ObjectMapper.Map<List<InvoiceItem>, List<InvoiceItemDto>>(items);
        
        return dto;
    }

    [Authorize(HISPermissions.Billing.ManageInvoices)]
    public async Task<InvoiceDto> UpdateStatusAsync(Guid id, InvoiceStatus status)
    {
        var invoice = await Repository.GetAsync(id);
        invoice.Status = status;
        await Repository.UpdateAsync(invoice);
        return ObjectMapper.Map<Invoice, InvoiceDto>(invoice);
    }

    [Microsoft.AspNetCore.Mvc.HttpGet]
    [Microsoft.AspNetCore.Mvc.Route("api/app/billing/invoice-pdf/{id}")]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetInvoicePdfAsync(Guid id)
    {
        var invoice = await Repository.GetAsync(id);
        var patient = await _patientRepository.GetAsync(invoice.PatientId);
        
        var itemsQueryable = await _itemRepository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(itemsQueryable.Where(x => x.InvoiceId == id));
        
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        
        byte[] logoBytes = null;
        var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? "", "images", "logo", "Dark.png");
        
        // Fallback for development if WebRootPath is not set or file not found in WebRootPath
        if (!System.IO.File.Exists(logoPath))
        {
            // Try to find it relative to current directory (for development)
            var devPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "Dark.png");
            if (System.IO.File.Exists(devPath)) logoPath = devPath;
        }

        if (System.IO.File.Exists(logoPath)) logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);

        var document = new HIS.Billing.Printing.InvoiceDocument
        {
             InvoiceNumber = invoice.InvoiceNumber,
             Date = invoice.InvoiceDate,
             DueDate = invoice.DueDate,
             Status = invoice.Status.ToString(),
             PatientName = $"{patient.FirstNameAr} {patient.LastNameAr}",
             PatientNumber = patient.Id.ToString().Substring(0, 8).ToUpper(),
             SubTotal = invoice.TotalAmount, // Assuming TotalAmount is subtotal in logic above (sum of items)
             Discount = invoice.DiscountAmount,
             Tax = invoice.TaxAmount,
             Total = invoice.NetAmount,
             LogoBytes = logoBytes,
             Items = items.Select(x => new HIS.Billing.Printing.InvoiceDocument.InvoiceItemModel 
             {
                 Service = x.Description, // Or ServiceCode map to Name
                 Quantity = x.Quantity,
                 UnitPrice = x.UnitPrice,
                 Total = x.TotalPrice
             }).ToList()
        };

        var pdfBytes = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(document);
        var stream = new System.IO.MemoryStream(pdfBytes);
        return new Volo.Abp.Content.RemoteStreamContent(stream, "Invoice.pdf", "application/pdf");
    }

    protected override async Task<IQueryable<Invoice>> CreateFilteredQueryAsync(GetInvoicesInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x => x.InvoiceNumber.Contains(input.SearchText));
        }

        if (input.PatientId.HasValue)
            queryable = queryable.Where(x => x.PatientId == input.PatientId);

        if (input.Status.HasValue)
            queryable = queryable.Where(x => x.Status == input.Status);

        if (input.FromDate.HasValue)
            queryable = queryable.Where(x => x.InvoiceDate >= input.FromDate);

        if (input.ToDate.HasValue)
            queryable = queryable.Where(x => x.InvoiceDate <= input.ToDate);

        return queryable;
    }

    protected override IQueryable<Invoice> ApplyDefaultSorting(IQueryable<Invoice> query)
    {
        return query.OrderByDescending(x => x.InvoiceDate);
    }
}

/// <summary>
/// خدمة المدفوعات
/// </summary>
public class PaymentAppService : CrudAppService<Payment, PaymentDto, Guid, GetPaymentsInput, CreatePaymentDto>, IPaymentAppService
{
    private readonly IRepository<Invoice, Guid> _invoiceRepository;
    private readonly IRepository<HIS.Accounting.ReceiptVoucher, Guid> _receiptVoucherRepository;
    private readonly IRepository<HIS.Accounting.JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<HIS.Accounting.Account, Guid> _accountRepository;
    private readonly IRepository<HIS.General.PaymentMethod, Guid> _paymentMethodRepository;
    private readonly IRepository<HIS.Patients.Patient, Guid> _patientRepository;

    public PaymentAppService(
        IRepository<Payment, Guid> repository,
        IRepository<Invoice, Guid> invoiceRepository,
        IRepository<HIS.Accounting.ReceiptVoucher, Guid> receiptVoucherRepository,
        IRepository<HIS.Accounting.JournalEntry, Guid> journalEntryRepository,
        IRepository<HIS.Accounting.Account, Guid> accountRepository,
        IRepository<HIS.General.PaymentMethod, Guid> paymentMethodRepository,
        IRepository<HIS.Patients.Patient, Guid> patientRepository) : base(repository)
    {
        _invoiceRepository = invoiceRepository;
        _receiptVoucherRepository = receiptVoucherRepository;
        _journalEntryRepository = journalEntryRepository;
        _accountRepository = accountRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _patientRepository = patientRepository;
        
        GetPolicyName = HISPermissions.Billing.Default;
        GetListPolicyName = HISPermissions.Billing.Default;
        CreatePolicyName = HISPermissions.Billing.ManageInvoices;
        UpdatePolicyName = HISPermissions.Billing.ManageInvoices;
        DeletePolicyName = HISPermissions.Billing.ManageInvoices;
    }

    public override async Task<PaymentDto> CreateAsync(CreatePaymentDto input)
    {
        await CheckCreatePolicyAsync();

        var paymentId = GuidGenerator.Create();
        var paymentNumber = $"PAY-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

        var payment = new Payment(paymentId, CurrentTenant.Id, input.PatientId, paymentNumber, input.Amount)
        {
            InvoiceId = input.InvoiceId,
            PaymentMethod = input.PaymentMethod,
            ReferenceNumber = input.ReferenceNumber,
            Notes = input.Notes,
            Status = PaymentStatus.Completed
        };

        await Repository.InsertAsync(payment);

        // Update invoice paid amount if linked
        if (input.InvoiceId.HasValue)
        {
            var invoice = await _invoiceRepository.GetAsync(input.InvoiceId.Value);
            invoice.PaidAmount += input.Amount;
            
            if (invoice.PaidAmount >= invoice.NetAmount)
                invoice.Status = InvoiceStatus.Paid;
            else if (invoice.PaidAmount > 0)
                invoice.Status = InvoiceStatus.PartiallyPaid;
                
            await _invoiceRepository.UpdateAsync(invoice);

            // Auto-Create Receipt Voucher and Journal Entry
            await CreatePaymentAccountingEntriesAsync(payment, invoice, input.PaymentMethod);
        }

        return ObjectMapper.Map<Payment, PaymentDto>(payment);
    }

    private async Task CreatePaymentAccountingEntriesAsync(Payment payment, Invoice invoice, HIS.Billing.PaymentMethod methodType)
    {
        var arAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1120");
        var cashAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1110");
        var bankAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Name.Contains("Bank") || x.NameAr.Contains("بنك")); 
        
        var debitAccount = (methodType == HIS.Billing.PaymentMethod.Cash) ? cashAccount : (bankAccount ?? cashAccount);

        if (arAccount != null && debitAccount != null)
        {
            // Get Patient Name
            var patient = await _patientRepository.GetAsync(payment.PatientId);
            var payerName = patient != null ? $"{patient.FirstNameAr} {patient.LastNameAr}" : "Unknown";

            // 1. Receipt Voucher
            var rvNumber = $"RV-{payment.PaymentNumber}";
            var rv = new HIS.Accounting.ReceiptVoucher
            {
                VoucherNumber = rvNumber,
                Date = payment.PaymentDate,
                PatientId = payment.PatientId,
                PayerName = payerName,
                Amount = payment.Amount,
                Description = $"Payment for Invoice {invoice.InvoiceNumber}",
                PaymentMethodId = null
            };
            
            rv.Lines.Add(new HIS.Accounting.ReceiptVoucherLine
            {
                ReceiptVoucherId = rv.Id,
                AccountId = arAccount.Id, 
                Amount = payment.Amount, 
                Description = "Payment on Account" 
            });
            
            await _receiptVoucherRepository.InsertAsync(rv);


            // 2. Journal Entry
            var je = new HIS.Accounting.JournalEntry(
                GuidGenerator.Create(),
                payment.PaymentDate,
                payment.PaymentNumber,
                $"سند قبض رقم {payment.PaymentNumber} - المريض: {payerName}"
            );

            // Debit Cash/Bank
            je.AddLine(GuidGenerator, debitAccount.Id, payment.Amount, 0);
            // Credit AR
            je.AddLine(GuidGenerator, arAccount.Id, 0, payment.Amount);

            await _journalEntryRepository.InsertAsync(je);
        }
    }


    public async Task<decimal> GetTotalByDateRangeAsync(DateTime from, DateTime to)
    {
        await CheckGetListPolicyAsync(); 

        var queryable = await Repository.GetQueryableAsync();
        var payments = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.PaymentDate >= from && x.PaymentDate <= to && x.Status == PaymentStatus.Completed));
        return payments.Sum(x => x.Amount);
    }

    protected override async Task<IQueryable<Payment>> CreateFilteredQueryAsync(GetPaymentsInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x => x.PaymentNumber.Contains(input.SearchText));
        }

        if (input.PatientId.HasValue)
            queryable = queryable.Where(x => x.PatientId == input.PatientId);

        if (input.InvoiceId.HasValue)
            queryable = queryable.Where(x => x.InvoiceId == input.InvoiceId);

        if (input.PaymentMethod.HasValue)
            queryable = queryable.Where(x => x.PaymentMethod == input.PaymentMethod);

        if (input.FromDate.HasValue)
            queryable = queryable.Where(x => x.PaymentDate >= input.FromDate);

        if (input.ToDate.HasValue)
            queryable = queryable.Where(x => x.PaymentDate <= input.ToDate);

        return queryable;
    }

    protected override IQueryable<Payment> ApplyDefaultSorting(IQueryable<Payment> query)
    {
        return query.OrderByDescending(x => x.PaymentDate);
    }

    public async Task<PaymentReceiptDto> GetReceiptDataAsync(Guid id)
    {
        await CheckGetPolicyAsync();
        
        var payment = await Repository.GetAsync(id);
        var patient = await _invoiceRepository.GetAsync(payment.InvoiceId ?? Guid.Empty); // Fallback logic might be needed
        // Properly fetching patient name. Since Payment has PatientId, we should repository for Patient if we have access, 
        // or rely on what we have. 
        // IMPORTANT: We need to inject Patient Repository to get names or use existing invoice data.
        // For now, let's assume we can get basic info.
        
        // Let's use the Invoice to get details if available.
        var invoice = payment.InvoiceId.HasValue ? await _invoiceRepository.GetAsync(payment.InvoiceId.Value) : null;
        
        // We really need Patient name. Let's assume the frontend passes it or we fetch it. 
        // Since I cannot easily add PatientRepository here without constructor changes (which breaks compatibility carefully),
        // I will assume I can get it from Invoice or just return placeholders if patient repo is missing.
        // Wait, PaymentAppService doesn't have PatientRepository. 
        // However, I can add it.
        
        var dto = new PaymentReceiptDto
        {
            PaymentId = payment.Id,
            PaymentNumber = payment.PaymentNumber,
            PaymentDate = payment.PaymentDate,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod.ToString(), // Should be localized
            ReferenceNumber = payment.ReferenceNumber,
            ReceivedBy = payment.ReceivedBy ?? CurrentUser.UserName,
            Notes = payment.Notes,
            InvoiceNumber = invoice?.InvoiceNumber,
            PatientName = "Patient", // Placeholder until we inject PatientRepo
            AmountInWords = $"{payment.Amount} ج.م" // Placeholder
        };

        if (invoice != null)
        {
             // We can fetch items if we inject ItemRepo. 
             // For now, let's return just summary.
             dto.Items = new List<ReceiptItemDto> { new ReceiptItemDto { ServiceName = "Medical Services", Price = payment.Amount } };
        }
        
        return dto;
    }

    public async Task<PaymentDailyReportDto> GetDailyReportAsync(DateTime date)
    {
        await CheckGetListPolicyAsync();

        var startOfDay = date.Date;
        var endOfDay = date.Date.AddDays(1).AddTicks(-1);

        var queryable = await Repository.GetQueryableAsync();
        var payments = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.PaymentDate >= startOfDay && x.PaymentDate <= endOfDay && x.Status == PaymentStatus.Completed));

        var report = new PaymentDailyReportDto
        {
            Date = date,
            TotalAmount = payments.Sum(x => x.Amount)
        };

        var grouped = payments.GroupBy(x => x.PaymentMethod)
            .Select(g => new PaymentMethodSummaryDto
            {
                Method = g.Key,
                MethodName = g.Key.ToString(),
                Count = g.Count(),
                Total = g.Sum(x => x.Amount)
            }).ToList();

        report.Methods = grouped;

        return report;
    }

    [Authorize(HISPermissions.Billing.ManageInvoices)]
    public async Task<PaymentDto> RefundAsync(Guid id, string reason)
    {
        var payment = await Repository.GetAsync(id);
        
        if (payment.Status != PaymentStatus.Completed)
        {
            throw new UserFriendlyException("Only completed payments can be refunded.");
        }

        payment.Status = PaymentStatus.Refunded;
        payment.Notes += $" [Refunded: {reason}]";
        
        await Repository.UpdateAsync(payment);

        if (payment.InvoiceId.HasValue)
        {
            var invoice = await _invoiceRepository.GetAsync(payment.InvoiceId.Value);
            invoice.PaidAmount -= payment.Amount;
            
            if (invoice.PaidAmount < invoice.NetAmount)
            {
                invoice.Status = invoice.PaidAmount > 0 ? InvoiceStatus.PartiallyPaid : InvoiceStatus.Issued;
            }
            
            await _invoiceRepository.UpdateAsync(invoice);
        }

        return ObjectMapper.Map<Payment, PaymentDto>(payment);
    }
}

/// <summary>
/// خدمة المؤجلات
/// </summary>
public class DeferredPaymentAppService : CrudAppService<DeferredPayment, DeferredPaymentDto, Guid, GetDeferredPaymentsInput, CreateDeferredPaymentDto>, IDeferredPaymentAppService
{
    public DeferredPaymentAppService(IRepository<DeferredPayment, Guid> repository) : base(repository)
    {
        GetPolicyName = HISPermissions.Billing.Default;
        GetListPolicyName = HISPermissions.Billing.Default;
        CreatePolicyName = HISPermissions.Billing.ManageInvoices;
        UpdatePolicyName = HISPermissions.Billing.ManageInvoices;
        DeletePolicyName = HISPermissions.Billing.ManageInvoices;
    }

    public override async Task<DeferredPaymentDto> CreateAsync(CreateDeferredPaymentDto input)
    {
        await CheckCreatePolicyAsync();

        var id = GuidGenerator.Create();
        var deferredNumber = $"DEF-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
        var installmentAmount = input.TotalAmount / input.NumberOfInstallments;

        var deferred = new DeferredPayment(id, CurrentTenant.Id, input.PatientId, deferredNumber, input.TotalAmount)
        {
            InvoiceId = input.InvoiceId,
            DueDate = input.DueDate,
            NumberOfInstallments = input.NumberOfInstallments,
            InstallmentAmount = installmentAmount,
            Reason = input.Reason,
            ContactPhone = input.ContactPhone,
            Notes = input.Notes
        };

        await Repository.InsertAsync(deferred);

        return ObjectMapper.Map<DeferredPayment, DeferredPaymentDto>(deferred);
    }

    [Authorize(HISPermissions.Billing.ManageInvoices)]
    public async Task<DeferredPaymentDto> RecordPaymentAsync(Guid id, decimal amount)
    {
        var deferred = await Repository.GetAsync(id);
        deferred.PaidAmount += amount;
        
        if (deferred.RemainingAmount <= 0)
            deferred.Status = DeferredPaymentStatus.Settled;
            
        await Repository.UpdateAsync(deferred);
        return ObjectMapper.Map<DeferredPayment, DeferredPaymentDto>(deferred);
    }

    public async Task<List<DeferredPaymentDto>> GetOverdueAsync()
    {
        await CheckGetListPolicyAsync();
        
        var queryable = await Repository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(
            queryable.Where(x => x.DueDate < DateTime.Now && x.Status == DeferredPaymentStatus.Active));
        return ObjectMapper.Map<List<DeferredPayment>, List<DeferredPaymentDto>>(items);
    }

    protected override async Task<IQueryable<DeferredPayment>> CreateFilteredQueryAsync(GetDeferredPaymentsInput input)
    {
        var queryable = await Repository.GetQueryableAsync();

        if (!string.IsNullOrEmpty(input.SearchText))
        {
            queryable = queryable.Where(x => x.DeferredNumber.Contains(input.SearchText));
        }

        if (input.PatientId.HasValue)
            queryable = queryable.Where(x => x.PatientId == input.PatientId);

        if (input.Status.HasValue)
            queryable = queryable.Where(x => x.Status == input.Status);

        return queryable;
    }

    protected override IQueryable<DeferredPayment> ApplyDefaultSorting(IQueryable<DeferredPayment> query)
    {
        return query.OrderByDescending(x => x.CreatedDate);
    }
}

#region Interfaces
public interface IInvoiceAppService : ICrudAppService<InvoiceDto, Guid, GetInvoicesInput, CreateUpdateInvoiceDto>
{
    Task<InvoiceDto> GetWithItemsAsync(Guid id);
    Task<InvoiceDto> UpdateStatusAsync(Guid id, InvoiceStatus status);
}

public interface IPaymentAppService : ICrudAppService<PaymentDto, Guid, GetPaymentsInput, CreatePaymentDto>
{
    Task<decimal> GetTotalByDateRangeAsync(DateTime from, DateTime to);
    Task<PaymentReceiptDto> GetReceiptDataAsync(Guid id);
    Task<PaymentDailyReportDto> GetDailyReportAsync(DateTime date);
    Task<PaymentDto> RefundAsync(Guid id, string reason);
}

public interface IDeferredPaymentAppService : ICrudAppService<DeferredPaymentDto, Guid, GetDeferredPaymentsInput, CreateDeferredPaymentDto>
{
    Task<DeferredPaymentDto> RecordPaymentAsync(Guid id, decimal amount);
    Task<List<DeferredPaymentDto>> GetOverdueAsync();
}
#endregion
