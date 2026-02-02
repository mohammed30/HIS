using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Billing;

#region Invoice DTOs
public class InvoiceDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public decimal InsuranceCoverage { get; set; }
    public decimal CoPaymentAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public Guid? PatientInsuranceId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string? Notes { get; set; }
    public List<InvoiceItemDto>? Items { get; set; }
}

public class CreateUpdateInvoiceDto
{
    public Guid PatientId { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxPercentage { get; set; } = 15;
    public Guid? PatientInsuranceId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string? Notes { get; set; }
    public List<CreateUpdateInvoiceItemDto>? Items { get; set; }
}

public class GetInvoicesInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public Guid? PatientId { get; set; }
    public InvoiceStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
#endregion

#region InvoiceItem DTOs
public class InvoiceItemDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public ServiceType ServiceType { get; set; }
    public string? ServiceCode { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsCoveredByInsurance { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdateInvoiceItemDto
{
    public ServiceType ServiceType { get; set; }
    public string? ServiceCode { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsCoveredByInsurance { get; set; } = true;
    public string? Notes { get; set; }
}
#endregion

#region Payment DTOs
public class PaymentDto : FullAuditedEntityDto<Guid>
{
    public Guid? InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ReferenceNumber { get; set; }
    public PaymentStatus Status { get; set; }
    public string? ReceivedBy { get; set; }
    public string? Notes { get; set; }
}

public class CreatePaymentDto
{
    public Guid? InvoiceId { get; set; }
    public Guid PatientId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}

public class GetPaymentsInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? InvoiceId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
#endregion

#region DeferredPayment DTOs
public class DeferredPaymentDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public Guid? InvoiceId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string DeferredNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime DueDate { get; set; }
    public int NumberOfInstallments { get; set; }
    public decimal InstallmentAmount { get; set; }
    public DeferredPaymentStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? ContactPhone { get; set; }
    public string? Notes { get; set; }
}

public class CreateDeferredPaymentDto
{
    public Guid PatientId { get; set; }
    public Guid? InvoiceId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime DueDate { get; set; }
    public int NumberOfInstallments { get; set; } = 1;
    public string? Reason { get; set; }
    public string? ContactPhone { get; set; }
    public string? Notes { get; set; }
}

public class GetDeferredPaymentsInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public Guid? PatientId { get; set; }
    public DeferredPaymentStatus? Status { get; set; }
}
#endregion

#region Reporting DTOs
public class PaymentReceiptDto
{
    public Guid PaymentId { get; set; }
    public string PaymentNumber { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PatientName { get; set; }
    public string PatientFileNumber { get; set; }
    public decimal Amount { get; set; }
    public string AmountInWords { get; set; }
    public string PaymentMethod { get; set; }
    public string ReferenceNumber { get; set; }
    public string ReceivedBy { get; set; }
    public string Notes { get; set; }
    public string InvoiceNumber { get; set; }
    public List<ReceiptItemDto> Items { get; set; }
    public string HospitalName { get; set; } = "Asia Hospital";
    public string HospitalLogoUrl { get; set; }
}

public class ReceiptItemDto
{
    public string ServiceName { get; set; }
    public decimal Price { get; set; }
}

public class PaymentDailyReportDto
{
    public DateTime Date { get; set; }
    public List<PaymentMethodSummaryDto> Methods { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

public class PaymentMethodSummaryDto
{
    public PaymentMethod Method { get; set; }
    public string MethodName { get; set; }
    public int Count { get; set; }
    public decimal Total { get; set; }
}
#endregion

