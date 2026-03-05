using System;
using System.Collections.Generic;
using HIS.HR.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// دورة تجهيز المرتبات - Payroll Run
/// </summary>
public class PayrollRun : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// من تاريخ
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// حتى تاريخ
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// الإدارة (اختياري - لتجهيز إدارة محددة)
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// الدرجة الوظيفية (اختياري - تصفية)
    /// </summary>
    public Guid? JobGradeId { get; set; }

    /// <summary>
    /// الحالة
    /// </summary>
    public PayrollRunStatus Status { get; set; } = PayrollRunStatus.Draft;

    /// <summary>
    /// القيد المحاسبي المرتبط
    /// </summary>
    public Guid? JournalEntryId { get; set; }

    /// <summary>
    /// إجمالي الاستحقاقات
    /// </summary>
    public decimal TotalEarnings { get; set; }

    /// <summary>
    /// إجمالي الاستقطاعات
    /// </summary>
    public decimal TotalDeductions { get; set; }

    /// <summary>
    /// صافي المرتبات
    /// </summary>
    public decimal NetSalary { get; set; }

    /// <summary>
    /// بنود تفصيلية
    /// </summary>
    public ICollection<PayrollLine> Lines { get; set; } = new List<PayrollLine>();

    protected PayrollRun() { }

    public PayrollRun(Guid id, Guid? tenantId, DateTime periodStart, DateTime periodEnd)
        : base(id)
    {
        TenantId = tenantId;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
    }
}
