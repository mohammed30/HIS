using System;
using Volo.Abp.Application.Dtos;
using HIS.Financials;

namespace HIS.Financials.Dtos;

public class AccountDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; }
    public string NameAr { get; set; }
    public string NameEn { get; set; }
    public Guid? ParentId { get; set; }
    public int Level { get; set; }
    public AccountType Type { get; set; }
    public bool IsLeaf { get; set; }
}
