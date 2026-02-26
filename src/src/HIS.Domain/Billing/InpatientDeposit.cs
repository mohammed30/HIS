using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Billing;

/// <summary>
/// الدفعة المقدمة للتنويم - Inpatient Deposit Entity
/// </summary>
public class InpatientDeposit : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// المريض
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// التنويم المرتبط
    /// </summary>
    public Guid AdmissionId { get; set; }

    /// <summary>
    /// رقم الإيصال
    /// </summary>
    public string ReceiptNumber { get; set; } = string.Empty;

    /// <summary>
    /// تاريخ الاستلام
    /// </summary>
    public DateTime DepositDate { get; set; }

    /// <summary>
    /// المبلغ المستلم
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// طريقة الدفع
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    /// <summary>
    /// رقم المرجع (في حال الدفع بالبطاقة أو التحويل)
    /// </summary>
    public string? ReferenceNumber { get; set; }

    /// <summary>
    /// رقم القيد المحاسبي المرتبط
    /// </summary>
    public Guid? JournalEntryId { get; set; }

    /// <summary>
    /// اسم الموظف المستلم
    /// </summary>
    public string? ReceivedBy { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// حالة الدفعة (جديدة، مستخدمة كلياً، مستردة جزئياً)
    /// </summary>
    public DepositStatus Status { get; set; } = DepositStatus.Active;

    protected InpatientDeposit() { }

    public InpatientDeposit(Guid id, Guid? tenantId, Guid patientId, Guid admissionId, string receiptNumber, decimal amount)
        : base(id)
    {
        TenantId = tenantId;
        PatientId = patientId;
        AdmissionId = admissionId;
        ReceiptNumber = receiptNumber;
        Amount = amount;
        DepositDate = DateTime.Now;
    }
}

