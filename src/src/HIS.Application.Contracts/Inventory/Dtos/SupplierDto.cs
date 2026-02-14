using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Inventory.Dtos;

public class SupplierDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string TaxId { get; set; }
}
