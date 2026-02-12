using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.General;

/// <summary>
/// طرق الدفع - Payment Methods
/// </summary>
public class PaymentMethod : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;

    protected PaymentMethod() { }

    public PaymentMethod(Guid id, string nameAr, string nameEn, string? code = null, bool isDefault = false, Guid? tenantId = null) : base(id)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        Code = code;
        IsDefault = isDefault;
        TenantId = tenantId;
        IsActive = true;
    }
}
