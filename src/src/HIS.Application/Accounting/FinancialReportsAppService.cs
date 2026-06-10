using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Fluent;

namespace HIS.Accounting;

/// <summary>
/// خدمة التقارير المالية (قائمة الدخل و ربحية الأقسام)
/// </summary>
public class FinancialReportsAppService : ApplicationService, IFinancialReportsAppService
{
    private readonly IRepository<JournalEntryLine, Guid> _journalEntryLineRepository;
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<CostCenter, Guid> _costCenterRepository;

    public FinancialReportsAppService(
        IRepository<JournalEntryLine, Guid> journalEntryLineRepository,
        IRepository<JournalEntry, Guid> journalEntryRepository,
        IRepository<Account, Guid> accountRepository,
        IRepository<CostCenter, Guid> costCenterRepository)
    {
        _journalEntryLineRepository = journalEntryLineRepository;
        _journalEntryRepository = journalEntryRepository;
        _accountRepository = accountRepository;
        _costCenterRepository = costCenterRepository;
    }

    [Authorize] // Should have specific permission, but using default Authorize for now
    public async Task<byte[]> GetDepartmentProfitabilityReportAsync(DateTime startDate, DateTime endDate)
    {
        // Join Lines with Entries to filter by Date
        var entries = await _journalEntryRepository.GetQueryableAsync();
        var lines = await _journalEntryLineRepository.GetQueryableAsync();
        var accounts = await _accountRepository.GetQueryableAsync();
        var costCenters = await _costCenterRepository.GetQueryableAsync();

        var query = from l in lines
                    join e in entries on l.JournalEntryId equals e.Id
                    join a in accounts on l.AccountId equals a.Id
                    join c in costCenters on l.CostCenterId equals c.Id
                    where e.Date >= startDate && e.Date <= endDate && l.CostCenterId != null
                    // We consider Revenue accounts (Credit balance) and Expense accounts (Debit balance)
                    where a.Type == AccountType.Revenue || a.Type == AccountType.Expense
                    select new 
                    {
                        l.CostCenterId,
                        CostCenterName = c.NameAr,
                        AccountType = a.Type,
                        l.Debit,
                        l.Credit
                    };

        var rawData = await AsyncExecuter.ToListAsync(query);

        var groupedData = rawData.GroupBy(x => new { x.CostCenterId, x.CostCenterName })
            .Select(g => new HIS.Accounting.Reports.DepartmentProfitabilityDto
            {
                CostCenterId = g.Key.CostCenterId.Value,
                CostCenterName = g.Key.CostCenterName,
                // Revenue increases with Credit
                TotalRevenue = g.Where(x => x.AccountType == AccountType.Revenue).Sum(x => x.Credit - x.Debit),
                // Expenses increase with Debit
                TotalExpense = g.Where(x => x.AccountType == AccountType.Expense).Sum(x => x.Debit - x.Credit)
            }).ToList();

        var document = new HIS.Accounting.Reports.DepartmentProfitabilityReportDocument
        {
            StartDate = startDate,
            EndDate = endDate,
            UserName = CurrentUser.Name ?? CurrentUser.UserName ?? "مستخدم النظام",
            Items = groupedData
        };

        return document.GeneratePdf();
    }

    public async Task<FinancialDashboardSummaryDto> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate)
    {
        var incomeStatement = await GetIncomeStatementAsync(startDate, endDate);
        var balanceSheet = await GetBalanceSheetAsync(endDate);
        
        // Use existing logic for Department Profitability
        var entries = await _journalEntryRepository.GetQueryableAsync();
        var lines = await _journalEntryLineRepository.GetQueryableAsync();
        var accounts = await _accountRepository.GetQueryableAsync();
        var costCenters = await _costCenterRepository.GetQueryableAsync();

        var query = from l in lines
                    join e in entries on l.JournalEntryId equals e.Id
                    join a in accounts on l.AccountId equals a.Id
                    join c in costCenters on l.CostCenterId equals c.Id
                    where e.Date >= startDate && e.Date <= endDate && l.CostCenterId != null
                    where a.Type == AccountType.Revenue || a.Type == AccountType.Expense
                    select new 
                    {
                        l.CostCenterId,
                        CostCenterName = c.NameAr,
                        AccountType = a.Type,
                        l.Debit,
                        l.Credit
                    };

        var rawData = await AsyncExecuter.ToListAsync(query);

        var deptProfitability = rawData.GroupBy(x => new { x.CostCenterId, x.CostCenterName })
            .Select(g => new HIS.Accounting.DepartmentProfitabilityDto
            {
                CostCenterId = g.Key.CostCenterId.Value,
                CostCenterName = g.Key.CostCenterName,
                TotalRevenue = g.Where(x => x.AccountType == AccountType.Revenue).Sum(x => x.Credit - x.Debit),
                TotalExpense = g.Where(x => x.AccountType == AccountType.Expense).Sum(x => x.Debit - x.Credit)
            }).ToList();

        return new FinancialDashboardSummaryDto
        {
            TotalAssets = balanceSheet.TotalAssets,
            TotalLiabilities = balanceSheet.TotalLiabilities,
            TotalRevenue = incomeStatement.TotalRevenue,
            TotalExpenses = incomeStatement.TotalExpenses,
            NetIncome = incomeStatement.NetIncome,
            DepartmentProfitability = deptProfitability
        };
    }

    public async Task<DashboardIncomeStatementDto> GetIncomeStatementAsync(DateTime startDate, DateTime endDate)
    {
        var entries = await _journalEntryRepository.GetQueryableAsync();
        var lines = await _journalEntryLineRepository.GetQueryableAsync();
        var accounts = await _accountRepository.GetQueryableAsync();

        var query = from l in lines
                    join e in entries on l.JournalEntryId equals e.Id
                    join a in accounts on l.AccountId equals a.Id
                    where e.Date >= startDate && e.Date <= endDate
                    where a.Type == AccountType.Revenue || a.Type == AccountType.Expense
                    select new 
                    {
                        a.Code,
                        a.NameAr,
                        a.Type,
                        l.Debit,
                        l.Credit
                    };

        var rawData = await AsyncExecuter.ToListAsync(query);

        var grouped = rawData.GroupBy(x => new { x.Code, x.NameAr, x.Type })
            .Select(g => new 
            {
                g.Key.Code,
                g.Key.NameAr,
                g.Key.Type,
                Balance = g.Key.Type == AccountType.Revenue 
                    ? g.Sum(x => x.Credit - x.Debit) 
                    : g.Sum(x => x.Debit - x.Credit)
            }).ToList();

        var revenues = grouped.Where(x => x.Type == AccountType.Revenue)
            .Select(x => new DashboardAccountBalanceDto { AccountCode = x.Code, AccountName = x.NameAr, Balance = x.Balance }).ToList();
            
        var expenses = grouped.Where(x => x.Type == AccountType.Expense)
            .Select(x => new DashboardAccountBalanceDto { AccountCode = x.Code, AccountName = x.NameAr, Balance = x.Balance }).ToList();

        return new DashboardIncomeStatementDto
        {
            StartDate = startDate,
            EndDate = endDate,
            RevenueAccounts = revenues,
            ExpenseAccounts = expenses,
            TotalRevenue = revenues.Sum(x => x.Balance),
            TotalExpenses = expenses.Sum(x => x.Balance)
        };
    }

    public async Task<DashboardBalanceSheetDto> GetBalanceSheetAsync(DateTime asOfDate)
    {
        var entries = await _journalEntryRepository.GetQueryableAsync();
        var lines = await _journalEntryLineRepository.GetQueryableAsync();
        var accounts = await _accountRepository.GetQueryableAsync();

        // For Balance Sheet, we sum all transactions from the beginning of time up to asOfDate
        var query = from l in lines
                    join e in entries on l.JournalEntryId equals e.Id
                    join a in accounts on l.AccountId equals a.Id
                    where e.Date <= asOfDate
                    where a.Type == AccountType.Asset || a.Type == AccountType.Liability || a.Type == AccountType.Equity
                    select new 
                    {
                        a.Code,
                        a.NameAr,
                        a.Type,
                        l.Debit,
                        l.Credit
                    };

        var rawData = await AsyncExecuter.ToListAsync(query);

        var grouped = rawData.GroupBy(x => new { x.Code, x.NameAr, x.Type })
            .Select(g => new 
            {
                g.Key.Code,
                g.Key.NameAr,
                g.Key.Type,
                // Assets increase with Debit, Liabilities and Equity increase with Credit
                Balance = g.Key.Type == AccountType.Asset 
                    ? g.Sum(x => x.Debit - x.Credit) 
                    : g.Sum(x => x.Credit - x.Debit)
            }).ToList();

        var assets = grouped.Where(x => x.Type == AccountType.Asset)
            .Select(x => new DashboardAccountBalanceDto { AccountCode = x.Code, AccountName = x.NameAr, Balance = x.Balance }).ToList();
            
        var liabilities = grouped.Where(x => x.Type == AccountType.Liability)
            .Select(x => new DashboardAccountBalanceDto { AccountCode = x.Code, AccountName = x.NameAr, Balance = x.Balance }).ToList();
            
        var equity = grouped.Where(x => x.Type == AccountType.Equity)
            .Select(x => new DashboardAccountBalanceDto { AccountCode = x.Code, AccountName = x.NameAr, Balance = x.Balance }).ToList();

        return new DashboardBalanceSheetDto
        {
            AsOfDate = asOfDate,
            AssetAccounts = assets,
            LiabilityAccounts = liabilities,
            EquityAccounts = equity,
            TotalAssets = assets.Sum(x => x.Balance),
            TotalLiabilities = liabilities.Sum(x => x.Balance),
            TotalEquity = equity.Sum(x => x.Balance)
        };
    }
}


