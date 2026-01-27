using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Billing;

/// <summary>
/// خدمة الفواتير
/// </summary>
public class InvoiceAppService : CrudAppService<Invoice, InvoiceDto, Guid, GetInvoicesInput, CreateUpdateInvoiceDto>, IInvoiceAppService
{
    private readonly IRepository<InvoiceItem, Guid> _itemRepository;

    public InvoiceAppService(
        IRepository<Invoice, Guid> repository,
        IRepository<InvoiceItem, Guid> itemRepository) : base(repository)
    {
        _itemRepository = itemRepository;
    }

    public override async Task<InvoiceDto> CreateAsync(CreateUpdateInvoiceDto input)
    {
        var invoiceId = GuidGenerator.Create();
        var invoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
        
        var invoice = new Invoice(invoiceId, CurrentTenant.Id, input.PatientId, invoiceNumber)
        {
            DueDate = input.DueDate,
            DiscountAmount = input.DiscountAmount,
            TaxPercentage = input.TaxPercentage,
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

        return ObjectMapper.Map<Invoice, InvoiceDto>(invoice);
    }

    public async Task<InvoiceDto> GetWithItemsAsync(Guid id)
    {
        var invoice = await Repository.GetAsync(id);
        var dto = ObjectMapper.Map<Invoice, InvoiceDto>(invoice);
        
        var itemsQueryable = await _itemRepository.GetQueryableAsync();
        var items = await AsyncExecuter.ToListAsync(itemsQueryable.Where(x => x.InvoiceId == id));
        dto.Items = ObjectMapper.Map<List<InvoiceItem>, List<InvoiceItemDto>>(items);
        
        return dto;
    }

    public async Task<InvoiceDto> UpdateStatusAsync(Guid id, InvoiceStatus status)
    {
        var invoice = await Repository.GetAsync(id);
        invoice.Status = status;
        await Repository.UpdateAsync(invoice);
        return ObjectMapper.Map<Invoice, InvoiceDto>(invoice);
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

    public PaymentAppService(
        IRepository<Payment, Guid> repository,
        IRepository<Invoice, Guid> invoiceRepository) : base(repository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public override async Task<PaymentDto> CreateAsync(CreatePaymentDto input)
    {
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
        }

        return ObjectMapper.Map<Payment, PaymentDto>(payment);
    }

    public async Task<decimal> GetTotalByDateRangeAsync(DateTime from, DateTime to)
    {
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
}

/// <summary>
/// خدمة المؤجلات
/// </summary>
public class DeferredPaymentAppService : CrudAppService<DeferredPayment, DeferredPaymentDto, Guid, GetDeferredPaymentsInput, CreateDeferredPaymentDto>, IDeferredPaymentAppService
{
    public DeferredPaymentAppService(IRepository<DeferredPayment, Guid> repository) : base(repository)
    {
    }

    public override async Task<DeferredPaymentDto> CreateAsync(CreateDeferredPaymentDto input)
    {
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
}

public interface IDeferredPaymentAppService : ICrudAppService<DeferredPaymentDto, Guid, GetDeferredPaymentsInput, CreateDeferredPaymentDto>
{
    Task<DeferredPaymentDto> RecordPaymentAsync(Guid id, decimal amount);
    Task<List<DeferredPaymentDto>> GetOverdueAsync();
}
#endregion
