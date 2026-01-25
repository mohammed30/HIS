using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Settings;

/// <summary>
/// التخصص الطبي - Specialty Entity
/// </summary>
public class Specialty : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// كود التخصص
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// اسم التخصص بالعربية
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم التخصص بالإنجليزية
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// الوصف
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// الترتيب
    /// </summary>
    public int SortOrder { get; set; }

    protected Specialty()
    {
    }

    public Specialty(Guid id, Guid? tenantId, string code, string nameAr)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        NameAr = nameAr;
    }
}
