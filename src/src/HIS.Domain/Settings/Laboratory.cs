using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Settings;

/// <summary>
/// المعمل - Laboratory Entity
/// </summary>
public class Laboratory : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// كود المعمل
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// اسم المعمل بالعربية
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم المعمل بالإنجليزية
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// الوصف
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// الموقع
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// رقم الهاتف الداخلي
    /// </summary>
    public string? ExtensionNumber { get; set; }

    /// <summary>
    /// مدير المعمل
    /// </summary>
    public Guid? ManagerId { get; set; }

    /// <summary>
    /// ساعات العمل
    /// </summary>
    public string? WorkingHours { get; set; }

    /// <summary>
    /// يعمل على مدار الساعة
    /// </summary>
    public bool Is24Hours { get; set; }

    /// <summary>
    /// نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// الترتيب
    /// </summary>
    public int SortOrder { get; set; }

    protected Laboratory()
    {
    }

    public Laboratory(Guid id, Guid? tenantId, string code, string nameAr)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        NameAr = nameAr;
    }
}
