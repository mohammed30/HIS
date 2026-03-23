using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Settings;

/// <summary>
/// الطبيب - Doctor Entity
/// </summary>
public class Doctor : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// كود الطبيب
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// الاسم بالعربية
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// الاسم بالإنجليزية
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// التخصص
    /// </summary>
    public Guid SpecialtyId { get; set; }

    /// <summary>
    /// القسم
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// العيادة المرتبطة
    /// </summary>
    public Guid? ClinicId { get; set; }

    /// <summary>
    /// رقم الترخيص الطبي
    /// </summary>
    public string? LicenseNumber { get; set; }

    /// <summary>
    /// تاريخ انتهاء الترخيص
    /// </summary>
    public DateTime? LicenseExpiryDate { get; set; }

    /// <summary>
    /// رقم الجوال
    /// </summary>
    public string? MobileNumber { get; set; }

    /// <summary>
    /// البريد الإلكتروني
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// الدرجة العلمية
    /// </summary>
    public string? Degree { get; set; }

    /// <summary>
    /// سعر الكشف
    /// </summary>
    public decimal ConsultationFee { get; set; }

    /// <summary>
    /// سعر الكشف صباحي
    /// </summary>
    public decimal MorningConsultationFee { get; set; }

    /// <summary>
    /// سعر الكشف مسائي
    /// </summary>
    public decimal EveningConsultationFee { get; set; }

    /// <summary>
    /// سعر المتابعة
    /// </summary>
    public decimal FollowUpFee { get; set; }

    /// <summary>
    /// مدة الموعد بالدقائق
    /// </summary>
    public int AppointmentDuration { get; set; } = 15;

    /// <summary>
    /// صورة الطبيب
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// السيرة الذاتية
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// معرف المستخدم المرتبط
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// الترتيب
    /// </summary>
    public int SortOrder { get; set; }

    protected Doctor()
    {
    }

    public Doctor(
        Guid id,
        Guid? tenantId,
        string code,
        string nameAr,
        Guid specialtyId,
        Guid departmentId)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        NameAr = nameAr;
        SpecialtyId = specialtyId;
        DepartmentId = departmentId;
    }
}
