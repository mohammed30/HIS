using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Nursing;

public class FallRiskAssessment : FullAuditedEntity<Guid>
{
    public Guid PatientId { get; set; }
    public int TotalScore { get; set; }
    public RiskLevel RiskLevel { get; set; }
    
    // Factors
    public bool HistoryOfFalls { get; set; }
    public bool SecondaryDiagnosis { get; set; }
    public bool AmbulatoryAid { get; set; }
    public bool IVTherapy { get; set; }
    public bool GaitProblem { get; set; }
    public bool MentalStatusIssue { get; set; }

    public DateTime AssessmentTime { get; set; }

    public FallRiskAssessment() { }
}
