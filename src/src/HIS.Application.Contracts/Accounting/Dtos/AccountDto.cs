using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Accounting.Dtos;

public class AccountDto : AuditedEntityDto<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string NameAr { get; set; }
    public AccountType Type { get; set; }
    public Guid? ParentId { get; set; }
    public string ParentName { get; set; } // For display
    public bool IsActive { get; set; }
}
