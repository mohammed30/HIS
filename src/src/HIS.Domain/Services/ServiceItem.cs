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
    
    /// <summary>
    /// السعر
    /// </summary>
    public decimal Price { get; set; }
    
    // Lab-specific fields (used when Category = LabTest)
    /// <summary>
    /// الوحدة (للتحاليل)
    /// </summary>
    public string? Unit { get; set; }
    
    /// <summary>
    /// المرجع الطبيعي (للتحاليل)
    /// </summary>
    public string? ReferenceRange { get; set; }
    
    /// <summary>
    /// تعليمات (للتحاليل - صائم، إلخ)
    /// </summary>
    public string? Instructions { get; set; }

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
