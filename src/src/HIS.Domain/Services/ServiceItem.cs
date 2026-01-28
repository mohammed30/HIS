using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Services;

public class ServiceItem : FullAuditedAggregateRoot<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public ServiceCategory Category { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool IsActive { get; set; }

    protected ServiceItem() { }

    public ServiceItem(Guid id, string code, string name, ServiceCategory category, Guid? departmentId = null)
        : base(id)
    {
        Code = code;
        Name = name;
        Category = category;
        DepartmentId = departmentId;
        IsActive = true;
    }
}
