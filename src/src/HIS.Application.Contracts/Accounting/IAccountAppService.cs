using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using HIS.Accounting.Dtos;

namespace HIS.Accounting;

public interface IAccountAppService : ICrudAppService<AccountDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateAccountDto>
{
    // Add custom helper methods if needed later, like GetTreeAsync
}
