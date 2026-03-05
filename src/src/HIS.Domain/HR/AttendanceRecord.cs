using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// الغيابات والأذونات - Attendance Record / Permit
/// </summary>
public class AttendanceRecord : FullAuditedAggregateRoot<Guid>, IMultiTenant
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
    /// نوع الإذن
    /// </summary>
    public string? PermitType { get; set; }

    /// <summary>
    /// التاريخ
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// المدة - ساعات
    /// </summary>
    public int? Hours { get; set; }

    /// <summary>
    /// المدة - دقائق
    /// </summary>
    public int? Minutes { get; set; }

    /// <summary>
    /// السبب
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    protected AttendanceRecord() { }

    public AttendanceRecord(Guid id, Guid? tenantId, Guid employeeId, DateTime date)
        : base(id)
    {
        TenantId = tenantId;
        EmployeeId = employeeId;
        Date = date;
    }
}
