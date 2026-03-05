using System;
using HIS.HR.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// سلفية الموظف - Employee Loan
/// </summary>
public class EmployeeLoan : FullAuditedAggregateRoot<Guid>, IMultiTenant
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
    /// البند المرتبط (بند الاستقطاع)
    /// </summary>
    public Guid? CompensationItemId { get; set; }

    /// <summary>
    /// قيمة السلفية
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// تقسيم على (عدد الأقساط)
    /// </summary>
    public int Installments { get; set; } = 1;

    /// <summary>
    /// تبدأ من
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// الحالة
    /// </summary>
    public LoanStatus Status { get; set; } = LoanStatus.Active;

    /// <summary>
    /// المبلغ المسدد
    /// </summary>
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    protected EmployeeLoan() { }

    public EmployeeLoan(Guid id, Guid? tenantId, Guid employeeId, decimal amount)
        : base(id)
    {
        TenantId = tenantId;
        EmployeeId = employeeId;
        Amount = amount;
    }
}
