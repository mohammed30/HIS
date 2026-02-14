using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Inventory;

public class Supplier : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    public string ContactPerson { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }

    public string TaxId { get; set; }

    protected Supplier() { }

    public Supplier(Guid id, string name, string contactPerson = null, string phone = null, string email = null, string address = null, string taxId = null) 
        : base(id)
    {
        Name = name;
        ContactPerson = contactPerson;
        Phone = phone;
        Email = email;
        Address = address;
        TaxId = taxId;
    }
}
