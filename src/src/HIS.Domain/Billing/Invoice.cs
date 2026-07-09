using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Billing;

/// <summary>
/// الفاتورة - Invoice Entity
/// </summary>
public class Invoice : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// المريض
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// رقم الفاتورة
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ الفاتورة
    /// </summary>
    public DateTime InvoiceDate { get; set; }

    /// <summary>
    /// تاريخ الاستحقاق
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// إجمالي المبلغ
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// مبلغ الخصم
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// نسبة الضريبة
    /// </summary>
    public decimal TaxPercentage { get; set; } = 15;

    /// <summary>
    /// مبلغ الضريبة
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// المبلغ الصافي
    /// </summary>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// المبلغ المدفوع
    /// </summary>
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// المبلغ المتبقي
    /// </summary>
    public decimal DueAmount => NetAmount - PaidAmount;

    /// <summary>
    /// تغطية التأمين
    /// </summary>
    public decimal InsuranceCoverage { get; set; }

    /// <summary>
    /// مبلغ المشاركة (Co-payment)
    /// </summary>
    public decimal CoPaymentAmount { get; set; }

    /// <summary>
    /// الحالة
    /// </summary>
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    /// <summary>
    /// طريقة الدفع
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    /// <summary>
    /// نوع الفاتورة (بيع / مرتجع)
    /// </summary>
    public InvoiceType InvoiceType { get; set; } = InvoiceType.Sale;

    /// <summary>
    /// سبب الرفض (يُملأ عند رفض الفاتورة)
    /// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// معرف الفاتورة الأصلية (لفواتير الارتجاع)
    /// </summary>
    public Guid? OriginalInvoiceId { get; set; }

    /// <summary>
    /// رقم الفاتورة الأصلية (لفواتير الارتجاع)
    /// </summary>
    public string? OriginalInvoiceNumber { get; set; }

    /// <summary>
    /// تأمين المريض (إن وجد)
    /// </summary>
    public Guid? PatientInsuranceId { get; set; }

    /// <summary>
    /// الموعد المرتبط (إن وجد)
    /// </summary>
    public Guid? AppointmentId { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// بنود الفاتورة
    /// </summary>
    public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

    protected Invoice() { }

    public Invoice(Guid id, Guid? tenantId, Guid patientId, string invoiceNumber)
        : base(id)
    {
        TenantId = tenantId;
        PatientId = patientId;
        InvoiceNumber = invoiceNumber;
        InvoiceDate = DateTime.Now;
    }
}
