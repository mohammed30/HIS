using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Billing;

/// <summary>
/// الدفعة - Payment Entity
/// </summary>
public class Payment : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// الفاتورة
    /// </summary>
    public Guid? InvoiceId { get; set; }

    /// <summary>
    /// المريض
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// رقم الدفعة
    /// </summary>
    public string PaymentNumber { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ الدفع
    /// </summary>
    public DateTime PaymentDate { get; set; }

    /// <summary>
    /// المبلغ
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// طريقة الدفع
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    /// <summary>
    /// رقم المرجع (للبطاقات/التحويلات)
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// الحالة
    /// </summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Completed;

    /// <summary>
    /// اسم موظف الاستلام
    /// </summary>
    public string? ReceivedBy { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    // Navigation
    public virtual Invoice? Invoice { get; set; }

    protected Payment() { }

    public Payment(Guid id, Guid? tenantId, Guid patientId, string paymentNumber, decimal amount)
        : base(id)
    {
        TenantId = tenantId;
        PatientId = patientId;
        PaymentNumber = paymentNumber;
        Amount = amount;
        PaymentDate = DateTime.Now;
    }
}
