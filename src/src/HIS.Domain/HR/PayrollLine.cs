using System;
using HIS.HR.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// بند مرتب موظف في دورة التجهيز - Payroll Line
/// </summary>
public class PayrollLine : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// دورة تجهيز المرتبات
    /// </summary>
    public Guid PayrollRunId { get; set; }

    /// <summary>
    /// الموظف
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// البند
    /// </summary>
    public Guid CompensationItemId { get; set; }

    /// <summary>
    /// المبلغ
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// طبيعة البند (استحقاق / استقطاع)
    /// </summary>
    public CompensationNature Nature { get; set; }

    protected PayrollLine() { }

    public PayrollLine(Guid id, Guid payrollRunId, Guid employeeId, Guid compensationItemId, decimal amount, CompensationNature nature)
        : base(id)
    {
        PayrollRunId = payrollRunId;
        EmployeeId = employeeId;
        CompensationItemId = compensationItemId;
        Amount = amount;
        Nature = nature;
    }
}
