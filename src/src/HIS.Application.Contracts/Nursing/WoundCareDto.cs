using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Nursing;

public class WoundCareDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string Location { get; set; }
    public WoundStage Stage { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Depth { get; set; }
    public string Exudate { get; set; }
    public string Treatment { get; set; }
    public string Notes { get; set; }
    public DateTime AssessmentTime { get; set; }
}

public class CreateWoundCareDto
{
    public Guid PatientId { get; set; }
    public string Location { get; set; }
    public WoundStage Stage { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Depth { get; set; }
    public string Exudate { get; set; }
    public string Treatment { get; set; }
    public string Notes { get; set; }
    public DateTime AssessmentTime { get; set; } = DateTime.Now;
}
