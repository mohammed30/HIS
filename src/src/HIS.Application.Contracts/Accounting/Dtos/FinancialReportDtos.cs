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

public class DailyAccountsReportDto
{
    public List<ReportTransactionDto> Transactions { get; set; } = new();
    public decimal TotalReceipts => Transactions.Where(x => x.Type == "Receipt").Sum(x => x.Amount);
    public decimal TotalPayments => Transactions.Where(x => x.Type == "Payment").Sum(x => x.Amount);
}

public class ReportTransactionDto
{
    public DateTime Date { get; set; }
    public string ReferenceNumber { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } // Receipt, Payment, JournalEntry
    public string AccountName { get; set; }
}

public class CustomerDebtsReportDto
{
    public List<CustomerDebtDto> Debts { get; set; } = new();
    public decimal TotalOverallDebt => Debts.Sum(x => x.DueAmount);
}

public class CustomerDebtDto
{
    public Guid PatientId { get; set; }
    public string PatientName { get; set; }
    public string MRN { get; set; }
    public decimal TotalInvoiced { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal DueAmount { get; set; }
}

public class DiscountsReportDto
{
    public List<DiscountReportLineDto> Lines { get; set; } = new();
    public decimal TotalDiscounts => Lines.Sum(x => x.DiscountAmount);
}

public class DiscountReportLineDto
{
    public DateTime Date { get; set; }
    public string InvoiceNumber { get; set; }
    public string PatientName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
}
