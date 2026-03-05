using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Settings;

public class JobTitle : FullAuditedAggregateRoot<Guid>
{
    public string NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? Description { get; set; }
    public Guid? DepartmentId { get; set; }
    public virtual Department Department { get; set; }

    protected JobTitle() { }

    public JobTitle(Guid id, string nameAr, string nameEn = null, string description = null, Guid? departmentId = null) 
        : base(id)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        Description = description;
        DepartmentId = departmentId;
    }
}
