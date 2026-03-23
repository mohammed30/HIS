using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Inpatient;

/// <summary>
/// التنويم - Admission Entity
/// </summary>
public class Admission : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// المريض
    /// </summary>
    public Guid PatientId { get; set; }

    /// <summary>
    /// الغرفة
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// السرير
    /// </summary>
    public Guid BedId { get; set; }

    /// <summary>
    /// تاريخ الدخول
    /// </summary>
    public DateTime AdmissionDate { get; set; }

    /// <summary>
    /// تاريخ الخروج
    /// </summary>
    public DateTime? DischargeDate { get; set; }

    /// <summary>
    /// عدد الأيام
    /// </summary>
    public int NumberOfDays { get; set; }

    /// <summary>
    /// سقف التأمين
    /// </summary>
    public decimal InsuranceCeiling { get; set; }

    /// <summary>
    /// اسم المرافق
    /// </summary>
    public string? CompanionName { get; set; }

    /// <summary>
    /// هاتف المرافق
    /// </summary>
    public string? CompanionPhone { get; set; }

    /// <summary>
    /// عنوان المرافق
    /// </summary>
    public string? CompanionAddress { get; set; }

    /// <summary>
    /// الغرض من التنويم
    /// </summary>
    public string? Purpose { get; set; }

    /// <summary>
    /// نسبة الصيدلية
    /// </summary>
    public decimal PharmacyPercentage { get; set; }

    /// <summary>
    /// إيقاف الخدمات
    /// </summary>
    public bool IsServicesStopped { get; set; }

    /// <summary>
    /// حالة التنويم
    /// </summary>
    public AdmissionStatus Status { get; set; } = AdmissionStatus.Active;

    /// <summary>
    /// المبلغ الإجمالي
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// المبلغ المدفوع
    /// </summary>
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// مبلغ التأمين
    /// </summary>
    public decimal InsuranceAmount { get; set; }

    /// <summary>
    /// المبلغ المتبقي
    /// </summary>
    public decimal DueAmount => TotalAmount - PaidAmount - InsuranceAmount;

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// الفاتورة المرتبطة
    /// </summary>
    public Guid? InvoiceId { get; set; }
    public Guid? PatientInsuranceId { get; set; }

    /// <summary>
    /// إجمالي رسوم الغرف السابقة المتراكمة
    /// </summary>
    public decimal AccumulatedRoomCharges { get; set; }

    /// <summary>
    /// تاريخ آخر نقل للسرير أو الغرفة
    /// </summary>
    public DateTime LastTransferDate { get; set; }

    protected Admission() { }

    public Admission(Guid id, Guid? tenantId, Guid patientId, Guid roomId, Guid bedId)
        : base(id)
    {
        TenantId = tenantId;
        PatientId = patientId;
        RoomId = roomId;
        BedId = bedId;
        AdmissionDate = DateTime.Now;
        LastTransferDate = DateTime.Now;
        Status = AdmissionStatus.Active;
    }
}
