using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Emergency;

public class EmergencyVisit : FullAuditedAggregateRoot<Guid>
{
    public Guid PatientId { get; private set; }
    public DateTime ArrivalTime { get; private set; }
    public EmergencySeverity Severity { get; set; }
    public EmergencyVisitStatus Status { get; set; }
    
    public string ChiefComplaint { get; set; }
    
    // Vitals
    public string BloodPressure { get; set; }
    public int HeartRate { get; set; }
    public decimal Temperature { get; set; }
    public int RespiratoryRate { get; set; }
    public int OxygenSaturation { get; set; }
    
    public string? Notes { get; set; }

    protected EmergencyVisit() { }

    public EmergencyVisit(Guid id, Guid patientId, EmergencySeverity severity, string chiefComplaint) : base(id)
    {
        PatientId = patientId;
        ArrivalTime = DateTime.Now;
        Severity = severity;
        ChiefComplaint = chiefComplaint;
        Status = EmergencyVisitStatus.Triaged;
    }
}
