using System;

namespace HIS.Nursing;

public class CreateCarePlanDto
{
    public Guid PatientId { get; set; }
    public string Diagnosis { get; set; }
    public string Goal { get; set; }
    public string? Interventions { get; set; }
    public CarePlanStatus Status { get; set; } = CarePlanStatus.Active;
}
