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
    public List<FinancialReportLineDto> ExpenseLines { get; set; } = new();
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetIncome => TotalRevenue - TotalExpense;
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
    public decimal NetCashFlow { get; set; }
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
