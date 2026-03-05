using System;
using HIS.HR.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// العقوبات والإنذارات - Penalty
/// </summary>
public class Penalty : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// الموظف
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// نوع العقوبة
    /// </summary>
    public PenaltyType PenaltyType { get; set; }

    /// <summary>
    /// الوصف / السبب
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// المبلغ (في حالة الخصم من المرتب)
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// عدد أيام الإيقاف (في حالة الإيقاف)
    /// </summary>
    public int? SuspensionDays { get; set; }

    /// <summary>
    /// التاريخ
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    protected Penalty() { }

    public Penalty(Guid id, Guid? tenantId, Guid employeeId, PenaltyType penaltyType, DateTime date)
        : base(id)
    {
        TenantId = tenantId;
        EmployeeId = employeeId;
        PenaltyType = penaltyType;
        Date = date;
    }
}
