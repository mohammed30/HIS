using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Insurance;

/// <summary>
/// شركة التأمين - Insurance Company Entity
/// </summary>
public class InsuranceCompany : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// كود الشركة
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// اسم الشركة بالعربية
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم الشركة بالإنجليزية
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// رقم الهاتف
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// البريد الإلكتروني
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// العنوان
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// جهة الاتصال
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// رقم جهة الاتصال
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// الموقع الإلكتروني
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// الترتيب
    /// </summary>
    public int SortOrder { get; set; }

    protected InsuranceCompany() { }

    public InsuranceCompany(Guid id, Guid? tenantId, string code, string nameAr)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        NameAr = nameAr;
    }
}
