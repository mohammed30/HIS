using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace HIS.Settings.Dtos;

public class JobTitleDto : AuditedEntityDto<Guid>
{
    public string NameAr { get; set; }
    public string NameEn { get; set; }
    public string Description { get; set; }
    public Guid? DepartmentId { get; set; }
    public string DepartmentName { get; set; } // For display
}

public class CreateUpdateJobTitleDto
{
    [Required]
    [StringLength(100)]
    public string NameAr { get; set; }

    [StringLength(100)]
    public string NameEn { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    public Guid? DepartmentId { get; set; }
}
