using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Nursing;

public class PainAssessmentDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public int PainScore { get; set; }
    public PainLocation Location { get; set; }
    public string Characteristics { get; set; }
    public string Intervention { get; set; }
    public DateTime AssessmentTime { get; set; }
}

public class CreatePainAssessmentDto
{
    public Guid PatientId { get; set; }
    public int PainScore { get; set; }
    public PainLocation Location { get; set; }
    public string Characteristics { get; set; }
    public string Intervention { get; set; }
    public DateTime AssessmentTime { get; set; } = DateTime.Now;
}
