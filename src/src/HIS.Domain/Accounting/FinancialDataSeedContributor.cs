using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace HIS.Accounting;

public class FinancialDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    public ILogger<FinancialDataSeedContributor> Logger { get; set; }

    public FinancialDataSeedContributor(
        IRepository<Account, Guid> accountRepository,
        IRepository<JournalEntry, Guid> journalEntryRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _accountRepository = accountRepository;
        _journalEntryRepository = journalEntryRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        Logger = NullLogger<FinancialDataSeedContributor>.Instance;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var allAccounts = await _accountRepository.GetListAsync();

        // Check if we need to patch existing accounts with NameAr
        if (allAccounts.Count > 0)
        {
            await PatchArabicNamesAsync();
            return;
        }

        await CreateStandardAccountsAsync();
    }

    private async Task PatchArabicNamesAsync()
    {
        Logger.LogInformation("Starting Financial Data Patch...");

        // Ensure Root accounts exist even in patch mode
        var assets = await EnsureAccountExistsAsync("1000", "Assets", "الأصول", AccountType.Asset, null);
        var liabilities = await EnsureAccountExistsAsync("2000", "Liabilities", "الخصوم", AccountType.Liability, null);
        var equity = await EnsureAccountExistsAsync("3000", "Equity", "حقوق الملكية", AccountType.Equity, null);
        var revenue = await EnsureAccountExistsAsync("4000", "Revenue", "الإيرادات", AccountType.Revenue, null);
        var expenses = await EnsureAccountExistsAsync("5000", "Expenses", "المصروفات", AccountType.Expense, null);

        // Core Charts - Force Arabic Names
        await UpdateNameArAsync("1100", "أصول متداولة", true);
        await UpdateNameArAsync("1110", "النقدية", true);
        await UpdateNameArAsync("1120", "المدينون", true);
        await UpdateNameArAsync("1130", "المخزون", true);
        await UpdateNameArAsync("1200", "أصول ثابتة", true);
        await UpdateNameArAsync("1210", "مباني", true);
        await UpdateNameArAsync("1220", "أجهزة طبية", true);

        var currentLiabs = await EnsureAccountExistsAsync("2100", "Current Liabilities", "خصوم متداولة", AccountType.Liability, liabilities.Id);
        await UpdateNameArAsync("2110", "الدائنون", true);
        await EnsureAccountExistsAsync("2200", "VAT Payable", "ضريبة القيمة المضافة المستحقة", AccountType.Liability, currentLiabs.Id);

        await UpdateNameArAsync("3100", "رأس المال", true);
        await UpdateNameArAsync("3200", "أرباح مبقاة", true);

        await UpdateNameArAsync("4100", "إيرادات خدمات طبية", true);
        await UpdateNameArAsync("4110", "إيرادات العمليات", true);
        await EnsureAccountExistsAsync("4120", "Laboratory Revenue", "إيرادات المختبر", AccountType.Revenue, revenue.Id);
        await UpdateNameArAsync("4120", "إيرادات المختبر", true);
        await EnsureAccountExistsAsync("4130", "Radiology Revenue", "إيرادات الأشعة", AccountType.Revenue, revenue.Id);
        await UpdateNameArAsync("4130", "إيرادات الأشعة", true);
        await UpdateNameArAsync("4200", "إيرادات صيدلية", true);

        await UpdateNameArAsync("5100", "مصروفات الرواتب", true);
        await UpdateNameArAsync("5200", "مصروفات مستلزمات", true);
        await UpdateNameArAsync("5300", "مصروفات مرافق", true);
        
        await EnsureAccountExistsAsync("5400", "Petty Cash & Sundry Expenses", "نثريات ومصاريف متنوعة", AccountType.Expense, expenses.Id);
        await UpdateNameArAsync("5400", "نثريات ومصاريف متنوعة", true);

        await EnsureAccountExistsAsync("5410", "Buffet Expenses", "مصاريف البوفيه", AccountType.Expense, expenses.Id);
        await UpdateNameArAsync("5410", "مصاريف البوفيه", true);
        
        Logger.LogInformation("Financial Data Patch Completed.");
    }

    private async Task<Account> EnsureAccountExistsAsync(string code, string name, string nameAr, AccountType type, Guid? parentId)
    {
        var account = await _accountRepository.FirstOrDefaultAsync(x => x.Code == code);
        if (account == null)
        {
            Logger.LogInformation($"Account {code} missing. Creating...");
            return await CreateAccountAsync(code, name, nameAr, type, parentId);
        }
        
        bool changed = false;
        // Update NameAr if it was missing
        if (string.IsNullOrEmpty(account.NameAr) && !string.IsNullOrEmpty(nameAr))
        {
            account.NameAr = nameAr;
            changed = true;
        }

        if (account.ParentId != parentId)
        {
            Logger.LogInformation($"Account {code} has wrong parent (Current: {account.ParentId}, Expected: {parentId}). Updating...");
            account.ParentId = parentId;
            changed = true;
        }

        if (account.Type != type)
        {
            Logger.LogInformation($"Account {code} has wrong type (Current: {account.Type}, Expected: {type}). Updating...");
            account.Type = type;
            changed = true;
        }

        if (changed)
        {
            await _accountRepository.UpdateAsync(account);
        }
        
        return account;
    }

    private async Task UpdateNameArAsync(string code, string nameAr, bool force = false)
    {
        var accounts = await _accountRepository.GetListAsync(a => a.Code == code || a.Code.Trim() == code);
        foreach (var account in accounts)
        {
            if (force || string.IsNullOrEmpty(account.NameAr) || account.NameAr == account.Name)
            {
                if (account.NameAr != nameAr)
                {
                    account.NameAr = nameAr;
                    await _accountRepository.UpdateAsync(account);
                    Logger.LogInformation($"Updated Account {code} NameAr to: {nameAr}");
                }
            }
        }
    }

    private async Task CreateStandardAccountsAsync()
    {
        // 1. Assets (الأصول)
        var assets = await CreateAccountAsync("1000", "Assets", "الأصول", AccountType.Asset, null);
        var currentAssets = await CreateAccountAsync("1100", "Current Assets", "أصول متداولة", AccountType.Asset, assets.Id);
        var cash = await CreateAccountAsync("1110", "Cash", "النقدية", AccountType.Asset, currentAssets.Id);
        await CreateAccountAsync("1120", "Accounts Receivable", "المدينون", AccountType.Asset, currentAssets.Id);
        await CreateAccountAsync("1130", "Inventory", "المخزون", AccountType.Asset, currentAssets.Id);

        var fixedAssets = await CreateAccountAsync("1200", "Fixed Assets", "أصول ثابتة", AccountType.Asset, assets.Id);
        await CreateAccountAsync("1210", "Buildings", "مباني", AccountType.Asset, fixedAssets.Id);
        await CreateAccountAsync("1220", "Medical Equipment", "أجهزة طبية", AccountType.Asset, fixedAssets.Id);

        // 2. Liabilities (الخصوم/المطلوبات)
        var liabilities = await CreateAccountAsync("2000", "Liabilities", "الخصوم", AccountType.Liability, null);
        var currentLiabilities = await CreateAccountAsync("2100", "Current Liabilities", "خصوم متداولة", AccountType.Liability, liabilities.Id);
        await CreateAccountAsync("2110", "Accounts Payable", "الدائنون", AccountType.Liability, currentLiabilities.Id);
        await CreateAccountAsync("2200", "VAT Payable", "ضريبة القيمة المضافة المستحقة", AccountType.Liability, currentLiabilities.Id);

        // 3. Equity (حقوق الملكية)
        var equity = await CreateAccountAsync("3000", "Equity", "حقوق الملكية", AccountType.Equity, null);
        var capital = await CreateAccountAsync("3100", "Capital", "رأس المال", AccountType.Equity, equity.Id);
        await CreateAccountAsync("3200", "Retained Earnings", "أرباح مبقاة", AccountType.Equity, equity.Id);

        // 4. Revenue (الإيرادات)
        var revenue = await CreateAccountAsync("4000", "Revenue", "الإيرادات", AccountType.Revenue, null);
        await CreateAccountAsync("4100", "Medical Services Revenue", "إيرادات خدمات طبية", AccountType.Revenue, revenue.Id);
        await CreateAccountAsync("4110", "Surgery Revenue", "إيرادات العمليات", AccountType.Revenue, revenue.Id);
        await CreateAccountAsync("4120", "Laboratory Revenue", "إيرادات المختبر", AccountType.Revenue, revenue.Id);
        await CreateAccountAsync("4130", "Radiology Revenue", "إيرادات الأشعة", AccountType.Revenue, revenue.Id);
        await CreateAccountAsync("4200", "Pharmacy Revenue", "إيرادات صيدلية", AccountType.Revenue, revenue.Id);

        // 5. Expenses (المصروفات)
        var expenses = await CreateAccountAsync("5000", "Expenses", "المصروفات", AccountType.Expense, null);
        await CreateAccountAsync("5100", "Salaries Expense", "مصروفات الرواتب", AccountType.Expense, expenses.Id);
        await CreateAccountAsync("5200", "Supplies Expense", "مصروفات مستلزمات", AccountType.Expense, expenses.Id);
        await CreateAccountAsync("5300", "Utilities Expense", "مصروفات مرافق", AccountType.Expense, expenses.Id);
        await CreateAccountAsync("5400", "Petty Cash & Sundry Expenses", "نثريات ومصاريف متنوعة", AccountType.Expense, expenses.Id);
        await CreateAccountAsync("5410", "Buffet Expenses", "مصاريف البوفيه", AccountType.Expense, expenses.Id);

        // 6. Sample Journal Entries (only on fresh seed)
        await CreateJournalEntryAsync(DateTime.Now.AddDays(-30), "Initial Capital Injection", 
            new[] { (cash.Id, 100000m, 0m), (capital.Id, 0m, 100000m) });
    }

    private async Task CreateJournalEntryAsync(DateTime date, string description, (Guid AccountId, decimal Debit, decimal Credit)[] lines)
    {
        var entry = new JournalEntry(_guidGenerator.Create(), date, "JE-" + date.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999), description);

        foreach (var line in lines)
        {
            entry.AddLine(_guidGenerator, line.AccountId, line.Debit, line.Credit);
        }

        await _journalEntryRepository.InsertAsync(entry);
    }

    private async Task<Account> CreateAccountAsync(string code, string name, string nameAr, AccountType type, Guid? parentId)
    {
        var account = new Account(_guidGenerator.Create(), code, name, nameAr, type, parentId);
        await _accountRepository.InsertAsync(account);
        return account;
    }
}
