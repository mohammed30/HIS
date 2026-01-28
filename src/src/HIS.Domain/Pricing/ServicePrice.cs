using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Pricing;

public class ServicePrice : FullAuditedEntity<Guid>
{
    public Guid PriceListId { get; set; }
    public Guid ServiceItemId { get; set; }
    public decimal Amount { get; set; }
    public decimal CoPayAmount { get; set; }

    protected ServicePrice() { }

    public ServicePrice(Guid id, Guid priceListId, Guid serviceItemId, decimal amount, decimal coPayAmount = 0)
        : base(id)
    {
        PriceListId = priceListId;
        ServiceItemId = serviceItemId;
        Amount = amount;
        CoPayAmount = coPayAmount;
    }
}
