using System;
using System.Collections.Generic;
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
    public Guid? CategoryId { get; set; }
    public bool IsActive { get; set; }
    public string? Machine { get; set; }
    public string? TurnaroundTime { get; set; }

    public virtual ICollection<LabTestNormalRange> NormalRanges { get; set; }

    protected LabTest() 
    { 
        NormalRanges = new List<LabTestNormalRange>();
    }

    public LabTest(Guid id, string code, string name, decimal price) : base(id)
    {
        Code = code;
        Name = name;
        Price = price;
        IsActive = true;
        NormalRanges = new List<LabTestNormalRange>();
    }

    public void UpdateInfo(string name, decimal price, string? instructions, string? range, string? unit, string? machine, string? turnaroundTime)
    {
        Name = name;
        Price = price;
        Instructions = instructions;
        ReferenceRange = range;
        Unit = unit;
        Machine = machine;
        TurnaroundTime = turnaroundTime;
    }
}
