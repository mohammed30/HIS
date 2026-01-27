using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIS.Financials.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Financials;

public interface IAccountAppService : IApplicationService
{
    Task<List<AccountDto>> GetListAsync();
    Task<AccountDto> GetAsync(Guid id);
    Task<AccountDto> CreateAsync(CreateUpdateAccountDto input);
    Task<AccountDto> UpdateAsync(Guid id, CreateUpdateAccountDto input);
    Task DeleteAsync(Guid id);
    Task<List<AccountDto>> GetTreeAsync();
}
