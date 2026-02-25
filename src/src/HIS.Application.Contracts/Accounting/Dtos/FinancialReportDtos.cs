using System;
using System.Linq;
using System.Collections.Generic;

namespace HIS.Accounting.Dtos;

public class FinancialReportLineDto
{
    public string AccountCode { get; set; }
    public string AccountName { get; set; }
    public decimal Amount { get; set; }
    public decimal PreviousAmount { get; set; }
}

public class IncomeStatementDto
{
    public List<FinancialReportLineDto> RevenueLines { get; set; } = new();
    public List<FinancialReportLineDto> CostOfSalesLines { get; set; } = new();
    
    public decimal TotalRevenue { get; set; }
    public decimal TotalCostOfSales { get; set; }
    public decimal GrossProfit => TotalRevenue - TotalCostOfSales;
    
    public List<FinancialReportLineDto> GeneralAndAdminExpenseLines { get; set; } = new();
    public decimal TotalGeneralAndAdminExpenses { get; set; }
    
    public decimal OperatingProfit => GrossProfit - TotalGeneralAndAdminExpenses;
    
    public List<FinancialReportLineDto> OtherRevenueLines { get; set; } = new();
    public decimal TotalOtherRevenues { get; set; }
    
    public List<FinancialReportLineDto> OtherExpenseLines { get; set; } = new();
    public decimal TotalOtherExpenses { get; set; }
    
    public decimal ProfitBeforeTax => OperatingProfit + TotalOtherRevenues - TotalOtherExpenses;
    
    // For now, NetIncome is ProfitBeforeTax unless taxes are introduced later
    public decimal NetIncome => ProfitBeforeTax;
}

public class BalanceSheetDto
{
    public List<FinancialReportLineDto> AssetLines { get; set; } = new();
    public List<FinancialReportLineDto> LiabilityLines { get; set; } = new();
    public List<FinancialReportLineDto> EquityLines { get; set; } = new();
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public decimal PreviousYearEquity { get; set; }
    public decimal TotalPreviousAssets { get; set; }
    public decimal TotalPreviousLiabilities { get; set; }
    public decimal TotalPreviousEquity { get; set; }
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
    public decimal CashAtBeginning { get; set; }
    public decimal CashAtEnd { get; set; }
}

public class ChangesInEquityDto
{
    public EquityItemDto Capital { get; set; } = new EquityItemDto { Name = "رأس المال" };
    public EquityItemDto RetainedEarnings { get; set; } = new EquityItemDto { Name = "الأرباح المحتجزة" };
    public EquityItemDto NetIncome { get; set; } = new EquityItemDto { Name = "أرباح العام" };
    public EquityItemDto Dividends { get; set; } = new EquityItemDto { Name = "توزيعات أرباح" };
      
    public decimal TotalPreviousYear => Capital.PreviousYear + RetainedEarnings.PreviousYear + NetIncome.PreviousYear + Dividends.PreviousYear;
    public decimal TotalChange => Capital.Change + RetainedEarnings.Change + NetIncome.Change + Dividends.Change;
    public decimal TotalCurrentYear => Capital.CurrentYear + RetainedEarnings.CurrentYear + NetIncome.CurrentYear + Dividends.CurrentYear;
}

public class EquityItemDto
{
    public string Name { get; set; }
    public decimal PreviousYear { get; set; }
    public decimal Change { get; set; }
    public decimal CurrentYear => PreviousYear + Change;
}

public class DateRangeDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class DailyAccountsReportDto
{
    public List<ReportTransactionDto> Transactions { get; set; } = new();
    public decimal TotalReceipts { get; set; }
    public decimal TotalPayments { get; set; }
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
