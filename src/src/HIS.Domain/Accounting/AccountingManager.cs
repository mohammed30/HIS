using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace HIS.Accounting;

public class AccountingManager : DomainService
{
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IGuidGenerator _guidGenerator;

    public AccountingManager(IRepository<JournalEntry, Guid> journalEntryRepository, IGuidGenerator guidGenerator)
    {
        _journalEntryRepository = journalEntryRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task<JournalEntry> CreateEntryAsync(DateTime date, string reference, string description)
    {
        var entry = new JournalEntry(_guidGenerator.Create(), date, reference, description);
        return await _journalEntryRepository.InsertAsync(entry);
    }

    public async Task PostEntryAsync(JournalEntry entry)
    {
        // Validate Debits = Credits
        decimal totalDebit = 0;
        decimal totalCredit = 0;

        foreach (var line in entry.Lines)
        {
            totalDebit += line.Debit;
            totalCredit += line.Credit;
        }

        if (totalDebit != totalCredit)
        {
            throw new Volo.Abp.UserFriendlyException($"Journal Entry is unbalanced. Debits: {totalDebit}, Credits: {totalCredit}");
        }

        entry.IsPosted = true;
        await _journalEntryRepository.UpdateAsync(entry);
    }
}
