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
    private readonly IRepository<FinancialPeriod, Guid> _financialPeriodRepository;
    private readonly IRepository<CostCenter, Guid> _costCenterRepository;
    private readonly IRepository<HIS.Settings.Department, Guid> _departmentRepository;
    private readonly IRepository<AccountMapping, Guid> _accountMappingRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    public ILogger<FinancialDataSeedContributor> Logger { get; set; }

    public FinancialDataSeedContributor(
        IRepository<Account, Guid> accountRepository,
        IRepository<JournalEntry, Guid> journalEntryRepository,
        IRepository<FinancialPeriod, Guid> financialPeriodRepository,
        IRepository<CostCenter, Guid> costCenterRepository,
        IRepository<HIS.Settings.Department, Guid> departmentRepository,
        IRepository<AccountMapping, Guid> accountMappingRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _accountRepository = accountRepository;
        _journalEntryRepository = journalEntryRepository;
        _financialPeriodRepository = financialPeriodRepository;
        _costCenterRepository = costCenterRepository;
        _departmentRepository = departmentRepository;
        _accountMappingRepository = accountMappingRepository;
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
            await SeedAccountMappingsAsync();
            // await SeedSampleDashboardDataAsync();
            return;
        }

        await SeedFinancialPeriodsAsync();

        await CreateStandardAccountsAsync();

        await SeedAccountMappingsAsync();
        // await SeedSampleDashboardDataAsync();
    }

    private async Task SeedFinancialPeriodsAsync()
    {
        Logger.LogInformation("Seeding Financial Periods...");
        int startYear = 2015;
        int endYear = 2030;

        for (int year = startYear; year <= endYear; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var periodExists = await _financialPeriodRepository.FirstOrDefaultAsync(p => p.Year == year && p.Month == month);
                if (periodExists == null)
                {
                    var startDate = new DateTime(year, month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);
                    var period = new FinancialPeriod(_guidGenerator.Create(), year, month, startDate, endDate);
                    await _financialPeriodRepository.InsertAsync(period);
                }
            }
        }
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

    private async Task<CostCenter> GetOrCreateCostCenterAsync(string code, string nameAr, string nameEn)
    {
        var cc = await _costCenterRepository.FirstOrDefaultAsync(x => x.Code == code);
        if (cc == null)
        {
            cc = new CostCenter(_guidGenerator.Create(), code, nameAr, nameEn);
            await _costCenterRepository.InsertAsync(cc);
        }

        var dept = await _departmentRepository.FirstOrDefaultAsync(x => x.Code == code);
        if (dept != null && dept.CostCenterId == null)
        {
            dept.CostCenterId = cc.Id;
            await _departmentRepository.UpdateAsync(dept);
        }

        return cc;
    }

    private async Task SeedSampleDashboardDataAsync()
    {
        var today = DateTime.Today;

        // Delete any existing DASH-JE entries to re-seed with older dates
        var existingTestEntries = await _journalEntryRepository.GetListAsync(x => x.ReferenceNumber.StartsWith("DASH-JE-"));
        if (existingTestEntries.Any())
        {
            await _journalEntryRepository.DeleteManyAsync(existingTestEntries);
        }

        Logger.LogInformation("Seeding sample financial dashboard data...");

        // Fetch accounts
        var cash = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1110");
        var capital = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "3100");
        var labRevenue = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "4120");
        var radRevenue = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "4130");
        var medRevenue = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "4100");
        var salaries = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "5100");
        var supplies = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "5200");
        var utilities = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "5300");

        if (cash == null || labRevenue == null || salaries == null)
        {
            Logger.LogWarning("Required accounts for dashboard seeding are missing.");
            return;
        }

        // Fetch or create cost centers
        var ccLab = await GetOrCreateCostCenterAsync("DEP-LAB", "المختبر / المعمل", "Laboratory Department");
        var ccRad = await GetOrCreateCostCenterAsync("DEP-RAD", "قسم الأشعة والتصوير", "Radiology / Imaging Department");
        var ccOpd = await GetOrCreateCostCenterAsync("DEP-OPD", "العيادات الخارجية", "Outpatient Department (OPD)");
        var ccEr = await GetOrCreateCostCenterAsync("DEP-ER", "قسم الطوارئ والاستقبال", "Emergency Room (ER)");
        var ccPt = await GetOrCreateCostCenterAsync("DEP-PT", "قسم العلاج الطبيعي", "Physiotherapy Department");

        // 1. Initial Capital Entry (Balanced) - May 11
        var capitalEntry = new JournalEntry(_guidGenerator.Create(), today.AddDays(-30), "DASH-JE-001", "زيادة رأس المال نقداً") { IsPosted = true };
        capitalEntry.AddLine(_guidGenerator, cash.Id, 250000m, 0m);
        capitalEntry.AddLine(_guidGenerator, capital.Id, 0m, 250000m);
        await _journalEntryRepository.InsertAsync(capitalEntry);

        // 2. Outpatient Revenue Entry (Balanced) - May 16
        var opdRevEntry = new JournalEntry(_guidGenerator.Create(), today.AddDays(-25), "DASH-JE-002", "إيرادات عيادات خارجية نقدية") { IsPosted = true };
        opdRevEntry.AddLine(_guidGenerator, cash.Id, 45000m, 0m);
        opdRevEntry.AddLine(_guidGenerator, medRevenue.Id, 0m, 45000m, ccOpd.Id);
        await _journalEntryRepository.InsertAsync(opdRevEntry);

        // 3. Emergency Room Revenue Entry (Balanced) - May 19
        var erRevEntry = new JournalEntry(_guidGenerator.Create(), today.AddDays(-22), "DASH-JE-003", "إيرادات الطوارئ نقدية") { IsPosted = true };
        erRevEntry.AddLine(_guidGenerator, cash.Id, 30000m, 0m);
        erRevEntry.AddLine(_guidGenerator, medRevenue.Id, 0m, 30000m, ccEr.Id);
        await _journalEntryRepository.InsertAsync(erRevEntry);

        // 4. Lab Revenue Entry (Balanced) - May 21
        var labRevEntry = new JournalEntry(_guidGenerator.Create(), today.AddDays(-20), "DASH-JE-004", "إيرادات خدمات المختبر") { IsPosted = true };
        labRevEntry.AddLine(_guidGenerator, cash.Id, 25000m, 0m);
        labRevEntry.AddLine(_guidGenerator, labRevenue.Id, 0m, 25000m, ccLab.Id);
        await _journalEntryRepository.InsertAsync(labRevEntry);

        // 5. Radiology Revenue Entry (Balanced) - May 22
        var radRevEntry = new JournalEntry(_guidGenerator.Create(), today.AddDays(-19), "DASH-JE-005", "إيرادات خدمات الأشعة") { IsPosted = true };
        radRevEntry.AddLine(_guidGenerator, cash.Id, 35000m, 0m);
        radRevEntry.AddLine(_guidGenerator, radRevenue.Id, 0m, 35000m, ccRad.Id);
        await _journalEntryRepository.InsertAsync(radRevEntry);

        // 6. Salaries Expense Entry (Balanced) - May 23
        var salEntry = new JournalEntry(_guidGenerator.Create(), today.AddDays(-18), "DASH-JE-006", "رواتب موظفي الأقسام") { IsPosted = true };
        salEntry.AddLine(_guidGenerator, salaries.Id, 15000m, 0m, ccOpd.Id);
        salEntry.AddLine(_guidGenerator, salaries.Id, 12000m, 0m, ccEr.Id);
        salEntry.AddLine(_guidGenerator, salaries.Id, 8000m, 0m, ccLab.Id);
        salEntry.AddLine(_guidGenerator, salaries.Id, 10000m, 0m, ccRad.Id);
        salEntry.AddLine(_guidGenerator, salaries.Id, 11000m, 0m, ccPt.Id);
        salEntry.AddLine(_guidGenerator, cash.Id, 0m, 56000m);
        await _journalEntryRepository.InsertAsync(salEntry);

        // 7. Supplies Expense Entry (Balanced) - May 24
        var supEntry = new JournalEntry(_guidGenerator.Create(), today.AddDays(-17), "DASH-JE-007", "مستلزمات طبية مستهلكة") { IsPosted = true };
        supEntry.AddLine(_guidGenerator, supplies.Id, 5000m, 0m, ccLab.Id);
        supEntry.AddLine(_guidGenerator, supplies.Id, 7000m, 0m, ccRad.Id);
        supEntry.AddLine(_guidGenerator, supplies.Id, 4000m, 0m, ccPt.Id);
        supEntry.AddLine(_guidGenerator, cash.Id, 0m, 16000m);
        await _journalEntryRepository.InsertAsync(supEntry);

        // 8. Utilities Expense (Balanced, general - no Cost Center) - May 25
        var utilEntry = new JournalEntry(_guidGenerator.Create(), today.AddDays(-16), "DASH-JE-008", "مصروف كهرباء ومياه عمومي") { IsPosted = true };
        utilEntry.AddLine(_guidGenerator, utilities.Id, 8000m, 0m);
        utilEntry.AddLine(_guidGenerator, cash.Id, 0m, 8000m);
        await _journalEntryRepository.InsertAsync(utilEntry);

        // 9. Physiotherapy Revenue Entry (Balanced) - May 26
        var ptRevEntry = new JournalEntry(_guidGenerator.Create(), today.AddDays(-15), "DASH-JE-009", "إيرادات قسم العلاج الطبيعي") { IsPosted = true };
        ptRevEntry.AddLine(_guidGenerator, cash.Id, 25000m, 0m);
        ptRevEntry.AddLine(_guidGenerator, medRevenue.Id, 0m, 25000m, ccPt.Id);
        await _journalEntryRepository.InsertAsync(ptRevEntry);

        Logger.LogInformation("Sample financial dashboard data seeded successfully.");
    }

    private async Task SeedAccountMappingsAsync()
    {
        Logger.LogInformation("Seeding Account Mappings...");

        var mappings = new[]
        {
            new { Type = AccountMappingType.SalesRevenue, Code = "4200", IsMandatory = true },
            new { Type = AccountMappingType.CashAccount, Code = "1110", IsMandatory = true },
            new { Type = AccountMappingType.VATOutput, Code = "2200", IsMandatory = true },
            new { Type = AccountMappingType.VATInput, Code = "1120", IsMandatory = true },
            new { Type = AccountMappingType.Inventory, Code = "1130", IsMandatory = true },
            new { Type = AccountMappingType.COGS, Code = "5200", IsMandatory = true }
        };

        foreach (var m in mappings)
        {
            var existing = await _accountMappingRepository.FirstOrDefaultAsync(x => x.MappingType == m.Type);
            if (existing == null)
            {
                var account = await _accountRepository.FirstOrDefaultAsync(x => x.Code == m.Code);
                var mapping = new AccountMapping(_guidGenerator.Create(), m.Type, account?.Id, m.IsMandatory);
                await _accountMappingRepository.InsertAsync(mapping);
            }
        }
    }
}
