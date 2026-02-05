using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Inventory.Dtos;

public class WarehouseDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Location { get; set; }
}

public class CreateUpdateWarehouseDto
{
    public string Name { get; set; }
    public string Location { get; set; }
}
