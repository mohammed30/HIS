using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Laboratory;

public class LabTest : FullAuditedAggregateRoot<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; set; }
    public string? Instructions { get; set; }
    public string? ReferenceRange { get; set; }
    public string? Unit { get; set; }
    public bool IsActive { get; set; }

    protected LabTest() { }

    public LabTest(Guid id, string code, string name, decimal price) : base(id)
    {
        Code = code;
        Name = name;
        Price = price;
        IsActive = true;
    }

    public void UpdateInfo(string name, decimal price, string? instructions, string? range, string? unit)
    {
        Name = name;
        Price = price;
        Instructions = instructions;
        ReferenceRange = range;
        Unit = unit;
    }
}
