using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Nursing;

public class DueMedicationDto : EntityDto<Guid>
{
    public string DrugName { get; set; }
    public string Dosage { get; set; }
    public string Route { get; set; }
    public string Frequency { get; set; }
    public string Instructions { get; set; }
    public DateTime OrderDate { get; set; }
}
