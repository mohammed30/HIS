using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Accounting;

public class Account : FullAuditedAggregateRoot<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string NameAr { get; set; }
    public AccountType Type { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }

    protected Account() { }

    public Account(Guid id, string code, string name, string nameAr, AccountType type, Guid? parentId = null)
        : base(id)
    {
        Code = code;
        Name = name;
        NameAr = nameAr;
        Type = type;
        ParentId = parentId;
        IsActive = true;
    }
}
