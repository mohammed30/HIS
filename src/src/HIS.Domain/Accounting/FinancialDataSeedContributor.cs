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
        if (await _accountRepository.GetCountAsync() > 0)
        {
            return;
        }

        // 1. Assets
        var assets = await CreateAccountAsync("1000", "Assets", AccountType.Asset, null);
        var currentAssets = await CreateAccountAsync("1100", "Current Assets", AccountType.Asset, assets.Id);
        await CreateAccountAsync("1110", "Cash", AccountType.Asset, currentAssets.Id);
        await CreateAccountAsync("1120", "Accounts Receivable", AccountType.Asset, currentAssets.Id);
        await CreateAccountAsync("1130", "Inventory", AccountType.Asset, currentAssets.Id);

        var fixedAssets = await CreateAccountAsync("1200", "Fixed Assets", AccountType.Asset, assets.Id);
        await CreateAccountAsync("1210", "Buildings", AccountType.Asset, fixedAssets.Id);
        await CreateAccountAsync("1220", "Medical Equipment", AccountType.Asset, fixedAssets.Id);

        // 2. Liabilities
        var liabilities = await CreateAccountAsync("2000", "Liabilities", AccountType.Liability, null);
        var currentLiabilities = await CreateAccountAsync("2100", "Current Liabilities", AccountType.Liability, liabilities.Id);
        await CreateAccountAsync("2110", "Accounts Payable", AccountType.Liability, currentLiabilities.Id);

        // 3. Equity
        var equity = await CreateAccountAsync("3000", "Equity", AccountType.Equity, null);
        await CreateAccountAsync("3100", "Capital", AccountType.Equity, equity.Id);
        await CreateAccountAsync("3200", "Retained Earnings", AccountType.Equity, equity.Id);

        // 4. Revenue
        var revenue = await CreateAccountAsync("4000", "Revenue", AccountType.Revenue, null);
        await CreateAccountAsync("4100", "Medical Services Revenue", AccountType.Revenue, revenue.Id);
        await CreateAccountAsync("4200", "Pharmacy Revenue", AccountType.Revenue, revenue.Id);

        // 5. Expenses
        var expenses = await CreateAccountAsync("5000", "Expenses", AccountType.Expense, null);
        await CreateAccountAsync("5100", "Salaries Expense", AccountType.Expense, expenses.Id);
        await CreateAccountAsync("5200", "Supplies Expense", AccountType.Expense, expenses.Id);
        await CreateAccountAsync("5300", "Utilities Expense", AccountType.Expense, expenses.Id);
    }

    private async Task<Account> CreateAccountAsync(string code, string name, AccountType type, Guid? parentId)
    {
        var account = new Account(_guidGenerator.Create(), code, name, type, parentId);
        await _accountRepository.InsertAsync(account);
        return account;
    }
}
