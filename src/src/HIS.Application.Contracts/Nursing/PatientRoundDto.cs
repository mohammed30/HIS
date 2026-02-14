using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Nursing;

public class PatientRoundDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string Note { get; set; }
    public Guid? NurseId { get; set; }
}

public class CreatePatientRoundDto
{
    public Guid PatientId { get; set; }
    public string Note { get; set; }
}
