using System;
using System.ComponentModel.DataAnnotations;
using HIS.Appointments;

namespace HIS.Appointments.Dtos;

public class CreateAppointmentDto
{
    [Required]
    public Guid DoctorId { get; set; }

    [Required]
    public Guid ClinicId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public AppointmentType Type { get; set; }

    public string? Notes { get; set; }
}
