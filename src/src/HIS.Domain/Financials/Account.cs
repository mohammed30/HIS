using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace HIS.Financials;

public class Account : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public string Code { get; set; }
    public string NameAr { get; set; }
    public string NameEn { get; set; }
    public Guid? ParentId { get; set; }
    public int Level { get; set; }
    public AccountType Type { get; set; }
    public bool IsLeaf { get; set; } = true;

    protected Account()
    {
    }

    public Account(
        Guid id,
        Guid? tenantId,
        string code,
        string nameAr,
        string nameEn,
        AccountType type,
        Guid? parentId = null,
        int level = 0)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        NameAr = nameAr;
        NameEn = nameEn;
        Type = type;
        ParentId = parentId;
        Level = level;
    }
}
