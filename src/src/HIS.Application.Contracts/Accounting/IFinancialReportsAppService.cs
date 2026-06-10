using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HIS.Accounting;

public interface IFinancialReportsAppService : IApplicationService
{
    Task<byte[]> GetDepartmentProfitabilityReportAsync(DateTime startDate, DateTime endDate);
    
    Task<FinancialDashboardSummaryDto> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate);
    Task<DashboardIncomeStatementDto> GetIncomeStatementAsync(DateTime startDate, DateTime endDate);
    Task<DashboardBalanceSheetDto> GetBalanceSheetAsync(DateTime asOfDate);
}

public class FinancialDashboardSummaryDto
{
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetIncome { get; set; }
    public List<DepartmentProfitabilityDto> DepartmentProfitability { get; set; } = new();
}

public class DepartmentProfitabilityDto
{
    public Guid CostCenterId { get; set; }
    public string CostCenterName { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Profit => TotalRevenue - TotalExpense;
}

public class DashboardIncomeStatementDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetIncome => TotalRevenue - TotalExpenses;
    public List<DashboardAccountBalanceDto> RevenueAccounts { get; set; } = new();
    public List<DashboardAccountBalanceDto> ExpenseAccounts { get; set; } = new();
}

public class DashboardBalanceSheetDto
{
    public DateTime AsOfDate { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    
    public List<DashboardAccountBalanceDto> AssetAccounts { get; set; } = new();
    public List<DashboardAccountBalanceDto> LiabilityAccounts { get; set; } = new();
    public List<DashboardAccountBalanceDto> EquityAccounts { get; set; } = new();
}

public class DashboardAccountBalanceDto
{
    public string AccountCode { get; set; }
    public string AccountName { get; set; }
    public decimal Balance { get; set; }
}
