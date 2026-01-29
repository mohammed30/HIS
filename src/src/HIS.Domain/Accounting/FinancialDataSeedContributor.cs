using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace HIS.Accounting;

public class FinancialDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public FinancialDataSeedContributor(
        IRepository<Account, Guid> accountRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _accountRepository = accountRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // Check if we need to patch existing accounts with NameAr
        if (await _accountRepository.GetCountAsync() > 0)
        {
            await PatchArabicNamesAsync();
            return;
        }

        // ... (Standard creation logic remains, but we add the logic to create new if empty)
        await CreateStandardAccountsAsync();
    }

    private async Task PatchArabicNamesAsync()
    {
        await UpdateNameArAsync("1000", "الأصول");
        await UpdateNameArAsync("1100", "أصول متداولة");
        await UpdateNameArAsync("1110", "النقدية");
        await UpdateNameArAsync("1120", "المدينون");
        await UpdateNameArAsync("1130", "المخزون");
        
        await UpdateNameArAsync("1200", "أصول ثابتة");
        await UpdateNameArAsync("1210", "مباني");
        await UpdateNameArAsync("1220", "أجهزة طبية");

        await UpdateNameArAsync("2000", "الخصوم");
        await UpdateNameArAsync("2100", "خصوم متداولة");
        await UpdateNameArAsync("2110", "الدائنون");

        await UpdateNameArAsync("3000", "حقوق الملكية");
        await UpdateNameArAsync("3100", "رأس المال");
        await UpdateNameArAsync("3200", "أرباح مبقاة");

        await UpdateNameArAsync("4000", "الإيرادات");
        await UpdateNameArAsync("4100", "إيرادات خدمات طبية");
        await UpdateNameArAsync("4200", "إيرادات صيدلية");

        await UpdateNameArAsync("5000", "المصروفات");
        await UpdateNameArAsync("5100", "مصروفات الرواتب");
        await UpdateNameArAsync("5200", "مصروفات مستلزمات");
        await UpdateNameArAsync("5300", "مصروفات مرافق");
    }

    private async Task UpdateNameArAsync(string code, string nameAr)
    {
        var account = await _accountRepository.FirstOrDefaultAsync(a => a.Code == code);
        if (account != null)
        {
            account.NameAr = nameAr;
            await _accountRepository.UpdateAsync(account);
        }
    }

    private async Task CreateStandardAccountsAsync()
    {
        // 1. Assets (الأصول)
        var assets = await CreateAccountAsync("1000", "Assets", "الأصول", AccountType.Asset, null);
        var currentAssets = await CreateAccountAsync("1100", "Current Assets", "أصول متداولة", AccountType.Asset, assets.Id);
        await CreateAccountAsync("1110", "Cash", "النقدية", AccountType.Asset, currentAssets.Id);
        await CreateAccountAsync("1120", "Accounts Receivable", "المدينون", AccountType.Asset, currentAssets.Id);
        await CreateAccountAsync("1130", "Inventory", "المخزون", AccountType.Asset, currentAssets.Id);

        var fixedAssets = await CreateAccountAsync("1200", "Fixed Assets", "أصول ثابتة", AccountType.Asset, assets.Id);
        await CreateAccountAsync("1210", "Buildings", "مباني", AccountType.Asset, fixedAssets.Id);
        await CreateAccountAsync("1220", "Medical Equipment", "أجهزة طبية", AccountType.Asset, fixedAssets.Id);

        // 2. Liabilities (الخصوم/المطلوبات)
        var liabilities = await CreateAccountAsync("2000", "Liabilities", "الخصوم", AccountType.Liability, null);
        var currentLiabilities = await CreateAccountAsync("2100", "Current Liabilities", "خصوم متداولة", AccountType.Liability, liabilities.Id);
        await CreateAccountAsync("2110", "Accounts Payable", "الدائنون", AccountType.Liability, currentLiabilities.Id);

        // 3. Equity (حقوق الملكية)
        var equity = await CreateAccountAsync("3000", "Equity", "حقوق الملكية", AccountType.Equity, null);
        await CreateAccountAsync("3100", "Capital", "رأس المال", AccountType.Equity, equity.Id);
        await CreateAccountAsync("3200", "Retained Earnings", "أرباح مبقاة", AccountType.Equity, equity.Id);

        // 4. Revenue (الإيرادات)
        var revenue = await CreateAccountAsync("4000", "Revenue", "الإيرادات", AccountType.Revenue, null);
        await CreateAccountAsync("4100", "Medical Services Revenue", "إيرادات خدمات طبية", AccountType.Revenue, revenue.Id);
        await CreateAccountAsync("4200", "Pharmacy Revenue", "إيرادات صيدلية", AccountType.Revenue, revenue.Id);

        // 5. Expenses (المصروفات)
        var expenses = await CreateAccountAsync("5000", "Expenses", "المصروفات", AccountType.Expense, null);
        await CreateAccountAsync("5100", "Salaries Expense", "مصروفات الرواتب", AccountType.Expense, expenses.Id);
        await CreateAccountAsync("5200", "Supplies Expense", "مصروفات مستلزمات", AccountType.Expense, expenses.Id);
        await CreateAccountAsync("5300", "Utilities Expense", "مصروفات مرافق", AccountType.Expense, expenses.Id);
    }

    private async Task<Account> CreateAccountAsync(string code, string name, string nameAr, AccountType type, Guid? parentId)
    {
        var account = new Account(_guidGenerator.Create(), code, name, nameAr, type, parentId);
        await _accountRepository.InsertAsync(account);
        return account;
    }
}
