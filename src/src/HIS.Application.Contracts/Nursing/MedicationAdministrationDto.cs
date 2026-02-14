using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Nursing;

public class MedicationAdministrationDto : AuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public Guid MedicalOrderId { get; set; }
    public string? DrugName { get; set; }
    public DateTime AdministrationTime { get; set; }
    public AdministrationStatus Status { get; set; }
    public string? Dosage { get; set; }
    public string? Notes { get; set; }
}
