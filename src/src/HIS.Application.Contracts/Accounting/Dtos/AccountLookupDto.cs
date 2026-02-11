using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Accounting.Dtos;

public class AccountLookupDto : EntityDto<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string NameAr { get; set; }
    public AccountType Type { get; set; }
    public Guid? ParentId { get; set; }
    public bool HasChildren { get; set; }
}
