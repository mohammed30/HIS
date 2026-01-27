using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Appointments;

public class DoctorSchedule : FullAuditedEntity<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public Guid DoctorId { get; set; }
    
    public DayOfWeek DayOfWeek { get; set; }
    
    // Storing time as TimeSpan from midnight
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    
    /// <summary>
    /// Duration of each slot in minutes
    /// </summary>
    public int SlotDuration { get; set; } = 15;

    public bool IsActive { get; set; } = true;

    protected DoctorSchedule()
    {
    }

    public DoctorSchedule(
        Guid id, 
        Guid? tenantId, 
        Guid doctorId, 
        DayOfWeek dayOfWeek, 
        TimeSpan startTime, 
        TimeSpan endTime, 
        int slotDuration)
        : base(id)
    {
        TenantId = tenantId;
        DoctorId = doctorId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        SlotDuration = slotDuration;
    }
}
