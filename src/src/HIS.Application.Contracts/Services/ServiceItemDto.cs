using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Services;

public class ServiceItemDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public ServiceCategory Category { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUpdateServiceItemDto
{
    public string Code { get; set; }
    public string Name { get; set; }
    public ServiceCategory Category { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
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
