using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Nursing;

public class CarePlan : FullAuditedEntity<Guid>
{
    public Guid PatientId { get; set; }
    public string Diagnosis { get; set; } // Nursing Diagnosis
    public string Goal { get; set; }
    public string? Interventions { get; set; } // What nurse will do
    public string? Evaluation { get; set; } // Outcome
    public CarePlanStatus Status { get; set; }
    public DateTime DateCreate { get; set; }

    protected CarePlan() { }
    
    public CarePlan(Guid id, Guid patientId, string diagnosis, string goal) 
        : base(id)
    {
        PatientId = patientId;
        Diagnosis = diagnosis;
        Goal = goal;
        Status = CarePlanStatus.Active;
        DateCreate = DateTime.Now;
    }
}


