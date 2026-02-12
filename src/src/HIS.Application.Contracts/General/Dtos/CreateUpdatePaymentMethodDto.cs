using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace HIS.General.Dtos;

public class CreateUpdatePaymentMethodDto
{
    [Required]
    [StringLength(128)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [StringLength(128)]
    public string NameEn { get; set; } = string.Empty;

    [StringLength(32)]
    public string? Code { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
}
