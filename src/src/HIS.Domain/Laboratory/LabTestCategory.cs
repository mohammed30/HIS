using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Laboratory;

public class LabTestCategory : FullAuditedAggregateRoot<Guid>
{
    public string Code { get; private set; }
    public string Name { get; private set; }
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }

    protected LabTestCategory() { }

    public LabTestCategory(Guid id, string code, string name, Guid? parentId = null, int sortOrder = 0) : base(id)
    {
        Code = code;
        Name = name;
        ParentId = parentId;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public void UpdateInfo(string name, int sortOrder)
    {
        Name = name;
        SortOrder = sortOrder;
    }
}
