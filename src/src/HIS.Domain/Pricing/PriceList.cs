using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Pricing;

public class PriceList : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    protected PriceList() { }

    public PriceList(Guid id, string name, bool isDefault, DateTime effectiveFrom, DateTime? effectiveTo = null)
        : base(id)
    {
        Name = name;
        IsDefault = isDefault;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }
}
