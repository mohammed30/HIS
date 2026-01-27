using System;
using System.ComponentModel.DataAnnotations;

namespace HIS.Appointments;

public class CreateUpdateDoctorScheduleDto
{
    [Required]
    public Guid DoctorId { get; set; }

    [Required]
    public DayOfWeek DayOfWeek { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [Range(5, 120)]
    public int SlotDuration { get; set; } = 15;

    public bool IsActive { get; set; } = true;
}
