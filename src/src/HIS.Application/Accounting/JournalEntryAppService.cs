using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Accounting;

// TODO: Add Authorization
public class JournalEntryAppService : ApplicationService, IJournalEntryAppService
{
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<Account, Guid> _accountRepository;

    public JournalEntryAppService(
        IRepository<JournalEntry, Guid> journalEntryRepository,
        IRepository<Account, Guid> accountRepository)
    {
        _journalEntryRepository = journalEntryRepository;
        _accountRepository = accountRepository;
    }

    public async Task<PagedResultDto<JournalEntryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var query = await _journalEntryRepository.WithDetailsAsync(x => x.Lines);
        
        var totalCount = await AsyncExecuter.CountAsync(query);
        
        query = query.OrderByDescending(x => x.Date); // Default sort
        
        var items = await AsyncExecuter.ToListAsync(query.PageBy(input));

        // Manually map to include Account Names efficiently
        var entryDtos = ObjectMapper.Map<List<JournalEntry>, List<JournalEntryDto>>(items);
        
        var accountIds = items.SelectMany(x => x.Lines).Select(x => x.AccountId).Distinct().ToList();
        var accounts = await _accountRepository.GetListAsync(x => accountIds.Contains(x.Id));
        var accountDict = accounts.ToDictionary(x => x.Id, x => x.Name);

        foreach (var entryDto in entryDtos)
        {
            foreach (var lineDto in entryDto.Lines)
            {
                if (accountDict.ContainsKey(lineDto.AccountId))
                {
                    lineDto.AccountName = accountDict[lineDto.AccountId];
                }
            }
        }

        return new PagedResultDto<JournalEntryDto>(
            totalCount,
            entryDtos
        );
    }
}
