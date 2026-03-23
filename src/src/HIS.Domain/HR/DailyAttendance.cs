using System;
using HIS.HR.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.HR;

/// <summary>
/// سجل الحضور والانصراف اليومي - Daily Attendance
/// </summary>
public class DailyAttendance : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    /// <summary>
    /// الموظف
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// التاريخ
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// وقت الحضور
    /// </summary>
    public DateTime? CheckInTime { get; set; }

    /// <summary>
    /// وقت الانصراف
    /// </summary>
    public DateTime? CheckOutTime { get; set; }

    /// <summary>
    /// الحالة (حاضر/غائب/متأخر/انصراف مبكر/إجازة)
    /// </summary>
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

    /// <summary>
    /// ساعات العمل (محسوبة تلقائياً)
    /// </summary>
    public decimal WorkedHours { get; set; }

    /// <summary>
    /// ساعات العمل الإضافية
    /// </summary>
    public decimal OvertimeHours { get; set; }

    /// <summary>
    /// ملاحظات
    /// </summary>
    public string? Notes { get; set; }

    protected DailyAttendance() { }

    public DailyAttendance(Guid id, Guid? tenantId, Guid employeeId, DateTime date)
        : base(id)
    {
        TenantId = tenantId;
        EmployeeId = employeeId;
        Date = date;
    }

    /// <summary>
    /// حساب ساعات العمل تلقائياً من وقت الحضور والانصراف
    /// </summary>
    public void CalculateWorkedHours()
    {
        if (CheckInTime.HasValue && CheckOutTime.HasValue)
        {
            var diff = CheckOutTime.Value - CheckInTime.Value;
            var totalHours = (decimal)diff.TotalHours;
            
            // Standard shift is 8 hours
            WorkedHours = Math.Min(8, Math.Round(totalHours, 2));
            OvertimeHours = totalHours > 8 ? Math.Round(totalHours - 8, 2) : 0;
        }
        else
        {
            WorkedHours = 0;
            OvertimeHours = 0;
        }
    }
}
