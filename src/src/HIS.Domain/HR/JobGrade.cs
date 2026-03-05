using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// الدرجة الوظيفية - Job Grade Entity
/// </summary>
public class JobGrade : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// كود الدرجة
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// اسم الدرجة بالعربية
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// اسم الدرجة بالإنجليزية
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// الراتب الأساسي للدرجة
    /// </summary>
    public decimal BaseSalary { get; set; }

    /// <summary>
    /// نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    protected JobGrade() { }

    public JobGrade(Guid id, Guid? tenantId, string code, string nameAr)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        NameAr = nameAr;
    }
}
