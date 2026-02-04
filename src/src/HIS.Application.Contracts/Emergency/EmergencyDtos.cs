using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Emergency.Dtos;

public class EmergencyVisitDto : AuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public DateTime ArrivalTime { get; set; }
    public EmergencySeverity Severity { get; set; }
    public EmergencyVisitStatus Status { get; set; }
    public string ChiefComplaint { get; set; }
    
    // Vitals
    public string BloodPressure { get; set; }
    public int HeartRate { get; set; }
    public decimal Temperature { get; set; }
    public int RespiratoryRate { get; set; }
    public int OxygenSaturation { get; set; }
    
    public string Notes { get; set; }
}

public class CreateEmergencyVisitDto
{
    public Guid PatientId { get; set; }
    public string ChiefComplaint { get; set; }
}

public class TriageDto
{
    public EmergencySeverity Severity { get; set; }
    public string BloodPressure { get; set; }
    public int HeartRate { get; set; }
    public decimal Temperature { get; set; }
    public int RespiratoryRate { get; set; }
    public int OxygenSaturation { get; set; }
    public string Notes { get; set; }
}

public class UpdateStatusDto
{
    public EmergencyVisitStatus Status { get; set; }
    public string Notes { get; set; }
}
