using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Accounting.Dtos;

namespace HIS.Accounting;

public class AccountAppService : CrudAppService<Account, AccountDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateAccountDto>, IAccountAppService
{
    public AccountAppService(IRepository<Account, Guid> repository) 
        : base(repository)
    {
    }
}
