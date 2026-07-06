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
    private readonly FinancialPeriodManager _financialPeriodManager;
    private readonly IRepository<AccountMapping, Guid> _accountMappingRepository;

    public AccountingManager(
        IRepository<JournalEntry, Guid> journalEntryRepository, 
        IGuidGenerator guidGenerator,
        FinancialPeriodManager financialPeriodManager,
        IRepository<AccountMapping, Guid> accountMappingRepository)
    {
        _journalEntryRepository = journalEntryRepository;
        _guidGenerator = guidGenerator;
        _financialPeriodManager = financialPeriodManager;
        _accountMappingRepository = accountMappingRepository;
    }

    public async Task<JournalEntry> CreateEntryAsync(DateTime date, string reference, string description, bool isAutomatic = false)
    {
        await _financialPeriodManager.CheckIfPeriodIsClosedAsync(date);

        var entry = new JournalEntry(_guidGenerator.Create(), date, reference, description, isAutomatic);
        return await _journalEntryRepository.InsertAsync(entry);
    }

    public async Task PostEntryAsync(JournalEntry entry)
    {
        await _financialPeriodManager.CheckIfPeriodIsClosedAsync(entry.Date);

        if (entry.IsAutomatic)
        {
            var missingMappings = await _accountMappingRepository.GetListAsync(x => x.IsMandatory && x.AccountId == null);
            if (missingMappings.Count > 0)
            {
                var listStr = string.Join(", ", missingMappings.ConvertAll(x => x.MappingType.ToString()));
                throw new Volo.Abp.UserFriendlyException(
                    $"لا يمكن ترحيل القيد التلقائي لعدم اكتمال توجيهات الحسابات الإلزامية التالية: {listStr}");
            }
        }

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
