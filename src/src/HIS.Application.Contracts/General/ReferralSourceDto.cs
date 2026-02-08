using System;
using Volo.Abp.Application.Dtos;

namespace HIS.General;

public class ReferralSourceDto : EntityDto<Guid>
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUpdateReferralSourceDto
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; } = true;
}
