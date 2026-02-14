using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Nursing;

public class FluidBalance : FullAuditedEntity<Guid>
{
    public Guid PatientId { get; set; }
    public FluidType Type { get; set; }
    public FluidMetric Metric { get; set; }
    public double Amount { get; set; } // in ml
    public DateTime EntryTime { get; set; }
    public string Notes { get; set; }

    public FluidBalance() { }
}
