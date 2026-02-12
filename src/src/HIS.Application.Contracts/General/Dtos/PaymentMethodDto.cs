using System;
using Volo.Abp.Application.Dtos;

namespace HIS.General.Dtos;

public class PaymentMethodDto : FullAuditedEntityDto<Guid>
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
}
