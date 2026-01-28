using System;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Accounting;

public interface IJournalEntryAppService : IApplicationService
{
    Task<PagedResultDto<JournalEntryDto>> GetListAsync(PagedAndSortedResultRequestDto input);
}
