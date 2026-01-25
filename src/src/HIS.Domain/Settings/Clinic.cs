using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Settings;

/// <summary>
/// العيادة - Clinic Entity
/// </summary>
public class Clinic : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// كود العيادة
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// اسم العيادة بالعربية
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم العيادة بالإنجليزية
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// القسم
    /// </summary>
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// الموقع
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// رقم الغرفة
    /// </summary>
    public string? RoomNumber { get; set; }

    /// <summary>
    /// رقم الهاتف الداخلي
    /// </summary>
    public string? ExtensionNumber { get; set; }

    /// <summary>
    /// السعة (عدد المرضى بالساعة)
    /// </summary>
    public int Capacity { get; set; } = 4;

    /// <summary>
    /// مدة الموعد بالدقائق
    /// </summary>
    public int AppointmentDuration { get; set; } = 15;

    /// <summary>
    /// سعر الكشف
    /// </summary>
    public decimal ConsultationFee { get; set; }

    /// <summary>
    /// نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// الترتيب
    /// </summary>
    public int SortOrder { get; set; }

    protected Clinic()
    {
    }

    public Clinic(Guid id, Guid? tenantId, string code, string nameAr, Guid departmentId)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        NameAr = nameAr;
        DepartmentId = departmentId;
    }
}
