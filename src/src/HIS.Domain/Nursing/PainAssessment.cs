using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Nursing;

public class PainAssessment : FullAuditedEntity<Guid>
{
    public Guid PatientId { get; set; }
    public int PainScore { get; set; } // 0-10
    public PainLocation Location { get; set; }
    public string Characteristics { get; set; }
    public string Intervention { get; set; }
    public DateTime AssessmentTime { get; set; }

    public PainAssessment() { }
}
