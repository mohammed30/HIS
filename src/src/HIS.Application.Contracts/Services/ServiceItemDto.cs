using System;
using Volo.Abp.Application.Dtos;

using HIS.Services;

namespace HIS.Services;

public class ServiceItemDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public ServiceCategory Category { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool IsActive { get; set; }
    public decimal? Price { get; set; }
    // Lab-specific fields
    public string? Unit { get; set; }
    public string? ReferenceRange { get; set; }
    public string? Instructions { get; set; }
}

public class CreateUpdateServiceItemDto
{
    public string? Code { get; set; } // Optional, auto-generated if empty
    public string Name { get; set; }
    public ServiceCategory Category { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal? Price { get; set; }
    // Lab-specific fields
    public string? Unit { get; set; }
    public string? ReferenceRange { get; set; }
    public string? Instructions { get; set; }
}

public class RadiologyItemDto : ServiceItemDto
{
    public string Modality { get; set; }
    public string BodyPart { get; set; }
    public string Instructions { get; set; }
}

public class CreateUpdateRadiologyItemDto : CreateUpdateServiceItemDto
{
    public string Modality { get; set; }
    public string BodyPart { get; set; }
    public string Instructions { get; set; }
}
