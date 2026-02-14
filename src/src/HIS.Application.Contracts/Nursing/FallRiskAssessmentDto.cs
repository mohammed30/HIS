using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Nursing;

public class FallRiskAssessmentDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public int TotalScore { get; set; }
    public RiskLevel RiskLevel { get; set; }
    
    public bool HistoryOfFalls { get; set; }
    public bool SecondaryDiagnosis { get; set; }
    public bool AmbulatoryAid { get; set; }
    public bool IVTherapy { get; set; }
    public bool GaitProblem { get; set; }
    public bool MentalStatusIssue { get; set; }

    public DateTime AssessmentTime { get; set; }
}

public class CreateFallRiskAssessmentDto
{
    public Guid PatientId { get; set; }
    public bool HistoryOfFalls { get; set; }
    public bool SecondaryDiagnosis { get; set; }
    public bool AmbulatoryAid { get; set; }
    public bool IVTherapy { get; set; }
    public bool GaitProblem { get; set; }
    public bool MentalStatusIssue { get; set; }
    
    // Logic to calculate score will be in AppService
    public DateTime AssessmentTime { get; set; } = DateTime.Now;
}
