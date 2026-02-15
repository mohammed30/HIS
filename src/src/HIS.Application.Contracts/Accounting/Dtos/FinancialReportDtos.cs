using System;
using System.Collections.Generic;

namespace HIS.Accounting.Dtos;

public class FinancialReportLineDto
{
    public string AccountCode { get; set; }
    public string AccountName { get; set; }
    public decimal Amount { get; set; }
}

public class IncomeStatementDto
{
    public List<FinancialReportLineDto> RevenueLines { get; set; } = new();
    public List<FinancialReportLineDto> CostOfSalesLines { get; set; } = new();
    public List<FinancialReportLineDto> OperatingExpenseLines { get; set; } = new();
    
    public decimal TotalRevenue { get; set; }
    public decimal TotalCostOfSales { get; set; }
    public decimal GrossProfit => TotalRevenue - TotalCostOfSales;
    
    public decimal TotalOperatingExpenses { get; set; }
    public decimal NetIncome => GrossProfit - TotalOperatingExpenses;
}

public class BalanceSheetDto
{
    public List<FinancialReportLineDto> AssetLines { get; set; } = new();
    public List<FinancialReportLineDto> LiabilityLines { get; set; } = new();
    public List<FinancialReportLineDto> EquityLines { get; set; } = new();
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
}

public class CashFlowStatementDto
{
    public List<FinancialReportLineDto> OperatingActivities { get; set; } = new();
    public List<FinancialReportLineDto> InvestingActivities { get; set; } = new();
    public List<FinancialReportLineDto> FinancingActivities { get; set; } = new();
    public decimal TotalOperating { get; set; }
    public decimal TotalInvesting { get; set; }
    public decimal TotalFinancing { get; set; }
    public decimal NetCashFlow => TotalOperating + TotalInvesting + TotalFinancing;
}

public class ChangesInEquityDto
{
    public decimal OpeningBalance { get; set; }
    public decimal NetIncome { get; set; }
    public decimal CapitalChanges { get; set; }
    public decimal ClosingBalance => OpeningBalance + NetIncome + CapitalChanges;
    public List<FinancialReportLineDto> DetailLines { get; set; } = new();
}

public class DateRangeDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
