using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Pricing;

public class PriceListDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public class CreateUpdatePriceListDto
{
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public class ServicePriceDto : AuditedEntityDto<Guid>
{
    public Guid PriceListId { get; set; }
    public Guid ServiceItemId { get; set; }
    public string ServiceItemName { get; set; }
    public decimal Amount { get; set; }
    public decimal CoPayAmount { get; set; }
}

public class CreateUpdateServicePriceDto
{
    public Guid PriceListId { get; set; }
    public Guid ServiceItemId { get; set; }
    public decimal Amount { get; set; }
    public decimal CoPayAmount { get; set; }
}
