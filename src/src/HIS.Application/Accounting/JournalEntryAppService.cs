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
        var query = await _journalEntryRepository.WithDetailsAsync(x => x.Lines);

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.OrderByDescending(x => x.Date);

        var items = await AsyncExecuter.ToListAsync(query.PageBy(input));

        var entryDtos = ObjectMapper.Map<List<JournalEntry>, List<JournalEntryDto>>(items);

        var accountIds = items.SelectMany(x => x.Lines).Select(x => x.AccountId).Distinct().ToList();
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

        // Validate leaf-only accounts
        var accountIds = entry.Lines.Select(x => x.AccountId).Distinct().ToList();
        var allAccounts = await _accountRepository.GetListAsync();
        var parentIds = allAccounts.Where(x => x.ParentId.HasValue).Select(x => x.ParentId.Value).Distinct().ToHashSet();

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
