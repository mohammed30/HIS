using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Nursing;

public class FluidBalanceDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public FluidType Type { get; set; }
    public FluidMetric Metric { get; set; }
    public double Amount { get; set; }
    public DateTime EntryTime { get; set; }
    public string Notes { get; set; }
}

public class CreateFluidBalanceDto
{
    public Guid PatientId { get; set; }
    public FluidType Type { get; set; }
    public FluidMetric Metric { get; set; }
    public double Amount { get; set; }
    public DateTime EntryTime { get; set; } = DateTime.Now;
    public string Notes { get; set; }
}
