using System;

namespace HIS.Nursing;

public class CreateMedicationAdministrationDto
{
    public Guid PatientId { get; set; }
    public Guid MedicalOrderId { get; set; }
    public DateTime AdministrationTime { get; set; } = DateTime.Now;
    public AdministrationStatus Status { get; set; }
    public string? Dosage { get; set; }
    public string? Notes { get; set; }
}
