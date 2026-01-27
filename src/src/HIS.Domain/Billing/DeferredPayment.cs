using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Billing;

/// <summary>
/// المدفوعات المؤجلة - Deferred Payment Entity
/// </summary>
public class DeferredPayment : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// المريض
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// الفاتورة الأصلية
    /// </summary>
    public Guid? InvoiceId { get; set; }

    /// <summary>
    /// رقم المؤجل
    /// </summary>
    public string DeferredNumber { get; set; } = string.Empty;

    /// <summary>
    /// المبلغ الإجمالي
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// المبلغ المدفوع
    /// </summary>
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// المبلغ المتبقي
    /// </summary>
    public decimal RemainingAmount => TotalAmount - PaidAmount;

    /// <summary>
    /// تاريخ الإنشاء
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// تاريخ الاستحقاق
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// عدد الأقساط
    /// </summary>
    public int NumberOfInstallments { get; set; } = 1;

    /// <summary>
    /// قيمة القسط
    /// </summary>
    public decimal InstallmentAmount { get; set; }

    /// <summary>
    /// الحالة
    /// </summary>
    public DeferredPaymentStatus Status { get; set; } = DeferredPaymentStatus.Active;

    /// <summary>
    /// السبب
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// رقم جوال للتواصل
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    // Navigation
    public virtual Invoice? Invoice { get; set; }

    protected DeferredPayment() { }

    public DeferredPayment(Guid id, Guid? tenantId, Guid patientId, string deferredNumber, decimal totalAmount)
        : base(id)
    {
        TenantId = tenantId;
        PatientId = patientId;
        DeferredNumber = deferredNumber;
        TotalAmount = totalAmount;
        CreatedDate = DateTime.Now;
    }
}
