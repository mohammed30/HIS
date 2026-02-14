using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Nursing;

public class CarePlanDto : AuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public string Diagnosis { get; set; }
    public string Goal { get; set; }
    public string? Interventions { get; set; }
    public string? Evaluation { get; set; }
    public CarePlanStatus Status { get; set; }
    public DateTime DateCreate { get; set; }
}
