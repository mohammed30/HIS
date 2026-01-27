using System;
using Volo.Abp.Application.Dtos;
using HIS.Appointments;

namespace HIS.Appointments.Dtos;

public class AppointmentDto : AuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    
    public Guid ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public AppointmentType Type { get; set; }

    public string? Notes { get; set; }
}
