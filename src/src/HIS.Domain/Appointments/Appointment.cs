using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Appointments;

public class Appointment : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid ClinicId { get; set; }

    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public AppointmentType Type { get; set; }
    
    public bool IsWalkIn { get; set; } = false;
    public Guid? ServiceItemId { get; set; }

    public string? Notes { get; set; }

    protected Appointment()
    {
    }

    public Appointment(
        Guid id,
        Guid? tenantId,
        Guid patientId,
        Guid doctorId,
        Guid clinicId,
        DateTime appointmentDate,
        AppointmentStatus status,
        AppointmentType type)
        : base(id)
    {
        TenantId = tenantId;
        PatientId = patientId;
        DoctorId = doctorId;
        ClinicId = clinicId;
        AppointmentDate = appointmentDate;
        Status = status;
        Type = type;
    }
}
