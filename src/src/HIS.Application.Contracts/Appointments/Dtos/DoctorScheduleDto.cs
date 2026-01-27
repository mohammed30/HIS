using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Appointments;

public class DoctorScheduleDto : AuditedEntityDto<Guid>
{
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDuration { get; set; }
    public bool IsActive { get; set; }
}
