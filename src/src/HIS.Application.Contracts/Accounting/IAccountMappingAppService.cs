using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Application.Dtos;
using HIS.Accounting.Dtos;

namespace HIS.Accounting
{
    public interface IAccountMappingAppService : IApplicationService
    {
        Task<ListResultDto<AccountMappingDto>> GetListAsync();
        Task<AccountMappingDto> UpdateAsync(Guid id, UpdateAccountMappingDto input);
    }
}
