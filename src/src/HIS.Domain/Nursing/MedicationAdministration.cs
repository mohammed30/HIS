using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Nursing;

public class MedicationAdministration : FullAuditedEntity<Guid>
{
    public Guid PatientId { get; set; }
    public Guid MedicalOrderId { get; set; } // Link to the prescription/order
    public DateTime AdministrationTime { get; set; }
    public AdministrationStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? Dosage { get; set; } // Actual amount given
    
    // Snapshot of what was ordered vs given
    public string? DrugName { get; set; } 

    protected MedicationAdministration() { }

    public MedicationAdministration(Guid id, Guid patientId, Guid medicalOrderId, DateTime administrationTime, AdministrationStatus status) 
        : base(id)
    {
        PatientId = patientId;
        MedicalOrderId = medicalOrderId;
        AdministrationTime = administrationTime;
        Status = status;
    }
}


