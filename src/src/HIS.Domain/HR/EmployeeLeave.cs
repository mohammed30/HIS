using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// إجازة الموظف - Employee Leave
/// </summary>
public class EmployeeLeave : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// الموظف
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// الإدارة
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// نوع الإجازة
    /// </summary>
    public Guid LeaveTypeId { get; set; }

    /// <summary>
    /// تبدأ من
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// تنتهي في
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// المدة (بالأيام)
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// المستحق
    /// </summary>
    public int Entitled { get; set; }

    /// <summary>
    /// المستخدم
    /// </summary>
    public int Used { get; set; }

    /// <summary>
    /// الرصيد المتبقي
    /// </summary>
    public int Balance { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    protected EmployeeLeave() { }

    public EmployeeLeave(Guid id, Guid? tenantId, Guid employeeId, Guid leaveTypeId)
        : base(id)
    {
        TenantId = tenantId;
        EmployeeId = employeeId;
        LeaveTypeId = leaveTypeId;
    }
}
