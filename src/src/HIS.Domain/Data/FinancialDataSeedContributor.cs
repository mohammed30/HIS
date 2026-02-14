using System;
using System.Threading.Tasks;
using HIS.Accounting;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace HIS.Data
{
    public class FinancialDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Account, Guid> _accountRepository;
        private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
        private readonly IGuidGenerator _guidGenerator;

        public FinancialDataSeedContributor(
            IRepository<Account, Guid> accountRepository,
            IRepository<JournalEntry, Guid> journalEntryRepository,
            IGuidGenerator guidGenerator)
        {
            _accountRepository = accountRepository;
            _journalEntryRepository = journalEntryRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _accountRepository.GetCountAsync() > 0)
            {
                return;
            }

            // Seed Accounts
            var assets = await CreateAccountAsync("1000", "Assets", "الأصول", AccountType.Asset, null);
            var currentAssets = await CreateAccountAsync("1100", "Current Assets", "الأصول المتداولة", AccountType.Asset, assets.Id);
            var cash = await CreateAccountAsync("1110", "Cash", "نقدية", AccountType.Asset, currentAssets.Id);
            var bank = await CreateAccountAsync("1120", "Bank", "البنك", AccountType.Asset, currentAssets.Id);
            var accountsReceivable = await CreateAccountAsync("1200", "Accounts Receivable", "العملاء", AccountType.Asset, currentAssets.Id);

            var liabilities = await CreateAccountAsync("2000", "Liabilities", "الخصوم", AccountType.Liability, null);
            var accountsPayable = await CreateAccountAsync("2100", "Accounts Payable", "الموردين", AccountType.Liability, liabilities.Id);

            var equity = await CreateAccountAsync("3000", "Equity", "حقوق الملكية", AccountType.Equity, null);
            var capital = await CreateAccountAsync("3100", "Capital", "رأس المال", AccountType.Equity, equity.Id);
            var retainedEarnings = await CreateAccountAsync("3200", "Retained Earnings", "الأرباح المبقاة", AccountType.Equity, equity.Id);

            var revenue = await CreateAccountAsync("4000", "Revenue", "الإيرادات", AccountType.Revenue, null);
            var medicalRevenue = await CreateAccountAsync("4100", "Medical Revenue", "إيرادات طبية", AccountType.Revenue, revenue.Id);
            var consultationRevenue = await CreateAccountAsync("4110", "Consultation Income", "دخل الاستشارات", AccountType.Revenue, medicalRevenue.Id);

            var expenses = await CreateAccountAsync("5000", "Expenses", "المصروفات", AccountType.Expense, null);
            var salaryExpense = await CreateAccountAsync("5100", "Salaries Expense", "رواتب", AccountType.Expense, expenses.Id);
            var rentExpense = await CreateAccountAsync("5200", "Rent Expense", "إيجار", AccountType.Expense, expenses.Id);
            var supplyExpense = await CreateAccountAsync("5300", "Medical Supplies Expense", "مستلزمات طبية", AccountType.Expense, expenses.Id);

            // Seed Journal Entries
            await CreateJournalEntryAsync(DateTime.Now.AddDays(-30), "Initial Capital Injection", 
                new[] { (cash.Id, 100000m, 0m), (capital.Id, 0m, 100000m) });

            await CreateJournalEntryAsync(DateTime.Now.AddDays(-25), "Rent Payment", 
                new[] { (rentExpense.Id, 5000m, 0m), (cash.Id, 0m, 5000m) });

            await CreateJournalEntryAsync(DateTime.Now.AddDays(-20), "Purchase Medical Supplies", 
                new[] { (supplyExpense.Id, 2000m, 0m), (bank.Id, 0m, 2000m) });

            await CreateJournalEntryAsync(DateTime.Now.AddDays(-15), "Consultation Income (Cash)", 
                new[] { (cash.Id, 500m, 0m), (consultationRevenue.Id, 0m, 500m) });

            await CreateJournalEntryAsync(DateTime.Now.AddDays(-10), "Consultation Income (Credit)", 
                new[] { (accountsReceivable.Id, 1500m, 0m), (consultationRevenue.Id, 0m, 1500m) });

            await CreateJournalEntryAsync(DateTime.Now.AddDays(-5), "Salary Payment", 
                new[] { (salaryExpense.Id, 10000m, 0m), (bank.Id, 0m, 10000m) });
        }

        private async Task<Account> CreateAccountAsync(string code, string name, string nameAr, AccountType type, Guid? parentId)
        {
            var account = new Account(_guidGenerator.Create(), code, name, nameAr, type, parentId);
            
            // If parent exists, set parent as not leaf - BUT Account entity doesn't show IsLeaf property in the viewed file.
            // Checking file 1792: Account.cs DOES have Type, ParentId, etc. but NO IsLeaf property is visible in lines 1-28.
            // Wait, looking at file 1792, lines 1-28 don't show IsLeaf. Let me check if it inherits it or if I missed lines.
            // Using view_file I saw total lines 28. It inherits from FullAuditedAggregateRoot.
            // I will remove IsLeaf setting since it doesn't seem to exist on the entity based on the file view.
            
            return await _accountRepository.InsertAsync(account);
        }

        private async Task CreateJournalEntryAsync(DateTime date, string description, (Guid AccountId, decimal Debit, decimal Credit)[] lines)
        {
            var entry = new JournalEntry(_guidGenerator.Create(), date, "JE-" + date.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999), description);

            foreach (var line in lines)
            {
               // Using the method defined in JournalEntry class: public void AddLine(IGuidGenerator guidGenerator, Guid accountId, decimal debit, decimal credit)
               entry.AddLine(_guidGenerator, line.AccountId, line.Debit, line.Credit);
            }

            await _journalEntryRepository.InsertAsync(entry);
        }
    }
}
