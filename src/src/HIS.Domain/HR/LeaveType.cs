using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// نوع الإجازة - Leave Type
/// </summary>
public class LeaveType : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// اسم الإجازة
    /// </summary>
    public string NameAr { get; set; } = string.Empty;

    /// <summary>
    /// مدة الإجازة (بالأيام)
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// فئة الموظف المستفيدة
    /// </summary>
    public string? EmployeeClass { get; set; }

    /// <summary>
    /// يؤثر على المرتب
    /// </summary>
    public bool AffectsSalary { get; set; }

    /// <summary>
    /// رصيد (هل لها رصيد سنوي)
    /// </summary>
    public bool IsBalance { get; set; }

    /// <summary>
    /// إجازة عامة (رسمية)
    /// </summary>
    public bool IsPublicHoliday { get; set; }

    /// <summary>
    /// تبدأ من
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// تنتهي في
    /// </summary>
    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    protected LeaveType() { }

    public LeaveType(Guid id, Guid? tenantId, string nameAr)
        : base(id)
    {
        TenantId = tenantId;
        NameAr = nameAr;
    }
}
