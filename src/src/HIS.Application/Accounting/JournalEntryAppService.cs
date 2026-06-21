using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using System.Linq.Dynamic.Core;

namespace HIS.Accounting;

public class JournalEntryAppService : ApplicationService, IJournalEntryAppService
{
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly AccountingManager _accountingManager;
    private readonly IGuidGenerator _guidGenerator;

    public JournalEntryAppService(
        IRepository<JournalEntry, Guid> journalEntryRepository,
        IRepository<Account, Guid> accountRepository,
        AccountingManager accountingManager,
        IGuidGenerator guidGenerator)
    {
        _journalEntryRepository = journalEntryRepository;
        _accountRepository = accountRepository;
        _accountingManager = accountingManager;
        _guidGenerator = guidGenerator;
    }

    public async Task<PagedResultDto<JournalEntryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        // 1. Get queryable without details for Count
        var query = await _journalEntryRepository.GetQueryableAsync();
        
        // 2. Count
        var totalCount = await AsyncExecuter.CountAsync(query);

        // 3. Apply Sorting
        if (!input.Sorting.IsNullOrWhiteSpace())
        {
             query = query.OrderBy(input.Sorting);
        }
        else
        {
             query = query.OrderByDescending(x => x.Date);
        }

        // 4. Apply Paging
        query = query.PageBy(input);

        // 5. Fetch IDs first to keep main query light or just fetch entities with details?
        // Better pattern: fetch paged entities with details.
        
        // We need to re-construct query with details for the fetch? 
        // Or can we use implicit loading? 
        // With ABP, we can just use the repository's WithDetails on the *paged* result? No, WithDetails returns a new IQueryable.
        
        // Let's get a fresh query with details, apply same filter/sort/page.
        // Actually, since we don't have filters in this method (only PagedAndSorted), it's simple.
        
        var queryWithDetails = await _journalEntryRepository.WithDetailsAsync(x => x.Lines);
        
        if (!input.Sorting.IsNullOrWhiteSpace())
        {
             queryWithDetails = queryWithDetails.OrderBy(input.Sorting);
        }
        else
        {
             queryWithDetails = queryWithDetails.OrderByDescending(x => x.Date);
        }
        
        var items = await AsyncExecuter.ToListAsync(queryWithDetails.PageBy(input));

        var entryDtos = ObjectMapper.Map<List<JournalEntry>, List<JournalEntryDto>>(items);

        if (items.Any())
        {
            var accountIds = items.SelectMany(x => x.Lines).Select(x => x.AccountId).Distinct().ToList();
            if (accountIds.Any())
            {
                var accounts = await _accountRepository.GetListAsync(x => accountIds.Contains(x.Id));
                var accountDict = accounts.ToDictionary(x => x.Id);

                foreach (var entryDto in entryDtos)
                {
                    foreach (var lineDto in entryDto.Lines)
                    {
                        if (accountDict.TryGetValue(lineDto.AccountId, out var account))
                        {
                            lineDto.AccountName = account.Name;
                            lineDto.AccountCode = account.Code;
                            lineDto.AccountNameAr = account.NameAr;
                        }
                    }
                }
            }
        }

        return new PagedResultDto<JournalEntryDto>(totalCount, entryDtos);
    }

    public async Task<JournalEntryDto> GetAsync(Guid id)
    {
        var query = await _journalEntryRepository.WithDetailsAsync(x => x.Lines);
        var entry = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));

        if (entry == null)
        {
            throw new UserFriendlyException("Journal entry not found.");
        }

        var dto = ObjectMapper.Map<JournalEntry, JournalEntryDto>(entry);

        var accountIds = entry.Lines.Select(x => x.AccountId).Distinct().ToList();
        var accounts = await _accountRepository.GetListAsync(x => accountIds.Contains(x.Id));
        var accountDict = accounts.ToDictionary(x => x.Id);

        foreach (var lineDto in dto.Lines)
        {
            if (accountDict.TryGetValue(lineDto.AccountId, out var account))
            {
                lineDto.AccountName = account.Name;
                lineDto.AccountCode = account.Code;
                lineDto.AccountNameAr = account.NameAr;
            }
        }

        return dto;
    }

    public async Task<JournalEntryDto> CreateAsync(CreateUpdateJournalEntryDto input)
    {
        await ValidateLinesAsync(input.Lines);

        var referenceNumber = input.ReferenceNumber;
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            referenceNumber = await GenerateReferenceNumberAsync(input.Date);
        }

        var entry = new JournalEntry(
            _guidGenerator.Create(),
            input.Date,
            referenceNumber,
            input.Description
        );

        foreach (var line in input.Lines)
        {
            entry.AddLine(_guidGenerator, line.AccountId, line.Debit, line.Credit);
        }

        await _journalEntryRepository.InsertAsync(entry);

        return await GetAsync(entry.Id);
    }

    public async Task<JournalEntryDto> UpdateAsync(Guid id, CreateUpdateJournalEntryDto input)
    {
        var query = await _journalEntryRepository.WithDetailsAsync(x => x.Lines);
        var entry = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));

        if (entry == null)
        {
            throw new UserFriendlyException("Journal entry not found.");
        }

        if (entry.IsPosted)
        {
            throw new UserFriendlyException("Cannot edit a posted journal entry.");
        }

        await ValidateLinesAsync(input.Lines);

        entry.Date = input.Date;
        entry.Description = input.Description;

        if (!string.IsNullOrWhiteSpace(input.ReferenceNumber))
        {
            entry.ReferenceNumber = input.ReferenceNumber;
        }

        // Clear and re-add lines
        entry.Lines.Clear();
        foreach (var line in input.Lines)
        {
            entry.AddLine(_guidGenerator, line.AccountId, line.Debit, line.Credit);
        }

        await _journalEntryRepository.UpdateAsync(entry);

        return await GetAsync(entry.Id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entry = await _journalEntryRepository.GetAsync(id);

        if (entry.IsPosted)
        {
            throw new UserFriendlyException("Cannot delete a posted journal entry.");
        }

        await _journalEntryRepository.DeleteAsync(id);
    }

    public async Task<JournalEntryDto> PostAsync(Guid id)
    {
        var query = await _journalEntryRepository.WithDetailsAsync(x => x.Lines);
        var entry = await AsyncExecuter.FirstOrDefaultAsync(query.Where(x => x.Id == id));

        if (entry == null)
        {
            throw new UserFriendlyException("Journal entry not found.");
        }

        if (entry.IsPosted)
        {
            throw new UserFriendlyException("This journal entry is already posted.");
        }

        // Validate leaf-only accounts, automatically convert parent to leaf accounts for existing drafts
        var allAccounts = await _accountRepository.GetListAsync();
        var parentIds = allAccounts.Where(x => x.ParentId.HasValue && x.IsActive).Select(x => x.ParentId.Value).Distinct().ToHashSet();

        bool entryModified = false;
        foreach (var line in entry.Lines)
        {
            if (parentIds.Contains(line.AccountId))
            {
                var account = allAccounts.FirstOrDefault(x => x.Id == line.AccountId);
                if (account != null)
                {
                    var leafAccount = GetLeafAccount(account, allAccounts, parentIds);
                    if (leafAccount != null && leafAccount.Id != line.AccountId)
                    {
                        line.AccountId = leafAccount.Id;
                        entryModified = true;
                    }
                }
            }
        }

        if (entryModified)
        {
            await _journalEntryRepository.UpdateAsync(entry);
        }

        var accountIds = entry.Lines.Select(x => x.AccountId).Distinct().ToList();
        foreach (var accountId in accountIds)
        {
            if (parentIds.Contains(accountId))
            {
                var account = allAccounts.FirstOrDefault(x => x.Id == accountId);
                var accountName = account?.Name ?? accountId.ToString();
                throw new UserFriendlyException($"Cannot post to parent account '{accountName}'. Only leaf accounts are allowed.");
            }
        }

        await _accountingManager.PostEntryAsync(entry);

        return await GetAsync(entry.Id);
    }

    private Account GetLeafAccount(Account account, List<Account> allAccounts, HashSet<Guid> parentIds)
    {
        if (account == null) return null;

        if (!parentIds.Contains(account.Id))
        {
            return account;
        }

        var children = allAccounts.Where(x => x.ParentId == account.Id && x.IsActive).OrderBy(x => x.Code).ToList();
        foreach (var child in children)
        {
            var leaf = GetLeafAccount(child, allAccounts, parentIds);
            if (leaf != null)
            {
                return leaf;
            }
        }

        return account;
    }

    public async Task<List<AccountLookupDto>> GetAccountLookupAsync()
    {
        var allAccounts = await _accountRepository.GetListAsync(x => x.IsActive);
        var parentIds = allAccounts.Where(x => x.ParentId.HasValue).Select(x => x.ParentId.Value).Distinct().ToHashSet();

        return allAccounts.Select(a => new AccountLookupDto
        {
            Id = a.Id,
            Code = a.Code,
            Name = a.Name,
            NameAr = a.NameAr,
            Type = a.Type,
            ParentId = a.ParentId,
            HasChildren = parentIds.Contains(a.Id)
        })
        .OrderBy(x => x.Code)
        .ToList();
    }

    private async Task ValidateLinesAsync(List<CreateUpdateJournalEntryLineDto> lines)
    {
        if (lines == null || lines.Count < 2)
        {
            throw new UserFriendlyException("A journal entry must have at least 2 lines.");
        }

        // Validate balance
        var totalDebit = lines.Sum(x => x.Debit);
        var totalCredit = lines.Sum(x => x.Credit);

        if (totalDebit != totalCredit)
        {
            throw new UserFriendlyException($"Journal entry is unbalanced. Debits: {totalDebit:N2}, Credits: {totalCredit:N2}");
        }

        // Validate each line has either debit or credit (not both, not neither)
        foreach (var line in lines)
        {
            if (line.Debit > 0 && line.Credit > 0)
            {
                throw new UserFriendlyException("A line cannot have both debit and credit amounts.");
            }

            if (line.Debit == 0 && line.Credit == 0)
            {
                throw new UserFriendlyException("Each line must have either a debit or credit amount.");
            }

            if (line.Debit < 0 || line.Credit < 0)
            {
                throw new UserFriendlyException("Debit and credit amounts must be positive.");
            }
        }

        // Validate accounts exist
        var accountIds = lines.Select(x => x.AccountId).Distinct().ToList();
        var existingAccounts = await _accountRepository.GetListAsync(x => accountIds.Contains(x.Id));

        if (existingAccounts.Count != accountIds.Count)
        {
            throw new UserFriendlyException("One or more selected accounts do not exist.");
        }
    }

    private async Task<string> GenerateReferenceNumberAsync(DateTime date)
    {
        var prefix = $"JE-{date:yyyyMMdd}-";

        var query = await _journalEntryRepository.GetQueryableAsync();
        var count = await AsyncExecuter.CountAsync(
            query.Where(x => x.ReferenceNumber.StartsWith(prefix))
        );

        return $"{prefix}{(count + 1):D3}";
    }
}
