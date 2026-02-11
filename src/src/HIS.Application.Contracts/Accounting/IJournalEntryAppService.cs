using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Accounting;

public interface IJournalEntryAppService : IApplicationService
{
    Task<PagedResultDto<JournalEntryDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task<JournalEntryDto> GetAsync(Guid id);
    Task<JournalEntryDto> CreateAsync(CreateUpdateJournalEntryDto input);
    Task<JournalEntryDto> UpdateAsync(Guid id, CreateUpdateJournalEntryDto input);
    Task DeleteAsync(Guid id);
    Task<JournalEntryDto> PostAsync(Guid id);
    Task<List<AccountLookupDto>> GetAccountLookupAsync();
}
