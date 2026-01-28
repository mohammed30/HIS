using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Inventory;

public class Warehouse : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    public string Location { get; set; }

    protected Warehouse() { }

    public Warehouse(Guid id, string name, string location) : base(id)
    {
        Name = name;
        Location = location;
    }
}
