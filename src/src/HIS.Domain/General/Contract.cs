using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.General;

/// <summary>
/// التعاقدات - Contracts Master Data
/// </summary>
public class Contract : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; } = true;

    protected Contract() { }

    public Contract(Guid id, string nameAr, string nameEn, string? code = null, Guid? tenantId = null) : base(id)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        Code = code;
        TenantId = tenantId;
        IsActive = true;
    }
}
