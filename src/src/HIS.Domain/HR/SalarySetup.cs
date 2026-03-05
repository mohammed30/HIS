using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// ضبط المرتبات - Salary Setup (ربط بند ببموظف)
/// </summary>
public class SalarySetup : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// الموظف
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// البند
    /// </summary>
    public Guid CompensationItemId { get; set; }

    /// <summary>
    /// القيمة
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// تكرار بشكل دوري
    /// </summary>
    public bool IsRecurring { get; set; } = true;

    /// <summary>
    /// يبدأ من
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// الحالة (نشط / غير نشط)
    /// </summary>
    public bool IsActive { get; set; } = true;

    protected SalarySetup() { }

    public SalarySetup(Guid id, Guid? tenantId, Guid employeeId, Guid compensationItemId, decimal amount)
        : base(id)
    {
        TenantId = tenantId;
        EmployeeId = employeeId;
        CompensationItemId = compensationItemId;
        Amount = amount;
    }
}
