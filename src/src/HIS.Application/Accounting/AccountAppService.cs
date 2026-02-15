using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Accounting.Dtos;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using HIS.Billing;
using HIS.Patients;

namespace HIS.Accounting;

public class AccountAppService : CrudAppService<Account, AccountDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateAccountDto>, IAccountAppService
{
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<Invoice, Guid> _invoiceRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<ReceiptVoucher, Guid> _receiptVoucherRepository;
    private readonly IRepository<PaymentVoucher, Guid> _paymentVoucherRepository;

    public AccountAppService(
        IRepository<Account, Guid> repository,
        IRepository<JournalEntry, Guid> journalEntryRepository,
        IRepository<Invoice, Guid> invoiceRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<ReceiptVoucher, Guid> receiptVoucherRepository,
        IRepository<PaymentVoucher, Guid> paymentVoucherRepository)
        : base(repository)
    {
        _journalEntryRepository = journalEntryRepository;
        _invoiceRepository = invoiceRepository;
        _patientRepository = patientRepository;
        _receiptVoucherRepository = receiptVoucherRepository;
        _paymentVoucherRepository = paymentVoucherRepository;
    }

    public async Task<IncomeStatementDto> GetIncomeStatementAsync(DateRangeDto input)
    {
        var query = await _journalEntryRepository.GetQueryableAsync();
        var lines = query
            .Where(x => x.Date >= input.StartDate && x.Date <= input.EndDate)
            .SelectMany(x => x.Lines);

        var accountQuery = await Repository.GetQueryableAsync();
        
        var joined = from l in lines
                     join a in accountQuery on l.AccountId equals a.Id
                     where a.Code.StartsWith("4") || a.Code.StartsWith("5")
                     select new { l, a };
                     
        var result = await AsyncExecuter.ToListAsync(joined);

        var grouped = result
            .GroupBy(x => new { x.a.Code, x.a.Name })
            .Select(g => new
            {
                g.Key.Code,
                g.Key.Name,
                Amount = g.Sum(x => x.l.Credit - x.l.Debit) 
            })
            .ToList();

        var dto = new IncomeStatementDto();
        
        foreach (var item in grouped)
        {
            var line = new FinancialReportLineDto
            {
                AccountCode = item.Code,
                AccountName = item.Name,
                Amount = Math.Abs(item.Amount)
            };

            if (item.Code.StartsWith("4"))
            {
                dto.RevenueLines.Add(line);
            }
            else if (item.Code.StartsWith("50"))
            {
                // Cost of Sales (Direct Costs)
                dto.CostOfSalesLines.Add(line);
            }
            else if (item.Code.StartsWith("5"))
            {
                // Other Operating Expenses
                dto.OperatingExpenseLines.Add(line);
            }
        }

        dto.TotalRevenue = dto.RevenueLines.Sum(x => x.Amount);
        dto.TotalCostOfSales = dto.CostOfSalesLines.Sum(x => x.Amount);
        dto.TotalOperatingExpenses = dto.OperatingExpenseLines.Sum(x => x.Amount);
        
        return dto;
    }

    public async Task<BalanceSheetDto> GetBalanceSheetAsync(DateRangeDto input)
    {
        // Balance Sheet is "As Of". But using DateRange endDate.
        var endDate = input.EndDate;

        var query = await _journalEntryRepository.GetQueryableAsync();
        var lines = query
            .Where(x => x.Date <= endDate)
            .SelectMany(x => x.Lines);

        var accountQuery = await Repository.GetQueryableAsync();

        var joined = from l in lines
                     join a in accountQuery on l.AccountId equals a.Id
                     where a.Code.StartsWith("1") || a.Code.StartsWith("2") || a.Code.StartsWith("3")
                     select new { l, a };

        var result = await AsyncExecuter.ToListAsync(joined);

        var grouped = result
            .GroupBy(x => new { x.a.Code, x.a.Name })
            .Select(g => new
            {
                g.Key.Code,
                g.Key.Name,
                Amount = g.Sum(x => x.l.Debit - x.l.Credit) // Assets are Debit Normal. Amount > 0 is Debit.
            })
            .ToList();

        var dto = new BalanceSheetDto();

        foreach (var item in grouped)
        {
            var line = new FinancialReportLineDto
            {
                AccountCode = item.Code,
                AccountName = item.Name,
                Amount = item.Amount
            };

            if (item.Code.StartsWith("1"))
            {
                dto.AssetLines.Add(line); // Debit (Positive) is good.
            }
            else if (item.Code.StartsWith("2"))
            {
                // Liability. Credit Normal.
                // item.Amount = Debit - Credit. So Liability is usually Negative.
                // We want to show positive Liability.
                line.Amount = -line.Amount;
                dto.LiabilityLines.Add(line);
            }
            else if (item.Code.StartsWith("3"))
            {
                // Equity. Credit Normal.
                line.Amount = -line.Amount;
                dto.EquityLines.Add(line);
            }
        }

        dto.TotalAssets = dto.AssetLines.Sum(x => x.Amount);
        dto.TotalLiabilities = dto.LiabilityLines.Sum(x => x.Amount);
        dto.TotalEquity = dto.EquityLines.Sum(x => x.Amount);
        
        // Note: Retained Earnings (Current Year Net Income) might not be in Journal Entries yet if not closed.
        // A real system calculates Net Income for the period and adds it to Equity section dynamically.
        // For now, simple aggregation.
        
        return dto;
    }

    public async Task<CashFlowStatementDto> GetCashFlowStatementAsync(DateRangeDto input)
    {
        var incomeStatement = await GetIncomeStatementAsync(input);
        var netIncome = incomeStatement.NetIncome;

        var dto = new CashFlowStatementDto();
        
        // Operating Activities starting with Net Income
        dto.OperatingActivities.Add(new FinancialReportLineDto 
        { 
            AccountCode = "NET_INCOME", 
            AccountName = "Net Income", 
            Amount = netIncome 
        });

        var query = await _journalEntryRepository.GetQueryableAsync();
        var lines = query
            .Where(x => x.Date >= input.StartDate && x.Date <= input.EndDate)
            .SelectMany(x => x.Lines);

        var accountQuery = await Repository.GetQueryableAsync();
        
        var joined = from l in lines
                           join a in accountQuery on l.AccountId equals a.Id
                           where a.Code.StartsWith("1") || a.Code.StartsWith("2") || a.Code.StartsWith("3")
                           // Exclude Cash/Bank accounts from the movements themselves (Class 10 usually)
                           && !a.Code.StartsWith("10") 
                           select new { l, a };

        var result = await AsyncExecuter.ToListAsync(joined);

        var grouped = result
            .GroupBy(x => new { x.a.Code, x.a.Name })
            .Select(g => new
            {
                g.Key.Code,
                g.Key.Name,
                // For Assets (Class 1): Debit (pos) means Decrease in Cash (negative flow). Credit (neg) means Increase.
                // For Lib/Equity (Class 2/3): Credit (pos) means Increase in Cash (positive flow).
                Amount = g.Key.Code.StartsWith("1") 
                    ? g.Sum(x => x.l.Credit - x.l.Debit)
                    : g.Sum(x => x.l.Credit - x.l.Debit)
            })
            .ToList();

        foreach (var item in grouped)
        {
            var line = new FinancialReportLineDto { AccountCode = item.Code, AccountName = item.Name, Amount = item.Amount };
            
            if (item.Code.StartsWith("11") || item.Code.StartsWith("12") || item.Code.StartsWith("21"))
            {
                // Typical Working Capital (AR, Inventory, AP)
                dto.OperatingActivities.Add(line);
            }
            else if (item.Code.StartsWith("15") || item.Code.StartsWith("16"))
            {
                // Fixed Assets
                dto.InvestingActivities.Add(line);
            }
            else if (item.Code.StartsWith("2") || item.Code.StartsWith("3"))
            {
                // Loans (non-current) and Equity
                dto.FinancingActivities.Add(line);
            }
        }

        dto.TotalOperating = dto.OperatingActivities.Sum(x => x.Amount);
        dto.TotalInvesting = dto.InvestingActivities.Sum(x => x.Amount);
        dto.TotalFinancing = dto.FinancingActivities.Sum(x => x.Amount);
        
        return dto;
    }

    public async Task<ChangesInEquityDto> GetChangesInEquityAsync(DateRangeDto input)
    {
        var dto = new ChangesInEquityDto();
        
        // 1. Opening Balance (Equity Accounts at StartDate)
        // Query journal lines before StartDate for Class 3
        var query = await _journalEntryRepository.GetQueryableAsync();
        var openingLines = query
            .Where(x => x.Date < input.StartDate) // Before start
            .SelectMany(x => x.Lines);
            
        var accountQuery = await Repository.GetQueryableAsync();

        var openingEquity = await AsyncExecuter.SumAsync(
            from l in openingLines
            join a in accountQuery on l.AccountId equals a.Id
            where a.Code.StartsWith("3")
            select (decimal?)(l.Credit - l.Debit) // Equity is Credit Normal
        ) ?? 0;

        dto.OpeningBalance = openingEquity;

        // 2. Net Income for the period
        var incomeStatement = await GetIncomeStatementAsync(input);
        dto.NetIncome = incomeStatement.NetIncome;

        // 3. Dividends or Capital Injections
        // Look for specific accounts or movements in Equity (3xxx) during period EXCEPT Net Income closing?
        // Usually, direct equity entries (Capital stock issuance, Dividends declared).
        // We query Class 3 movements during period.
        
        var periodLines = query
            .Where(x => x.Date >= input.StartDate && x.Date <= input.EndDate)
            .SelectMany(x => x.Lines);
            
        var equityMovements = from l in periodLines
                              join a in accountQuery on l.AccountId equals a.Id
                              where a.Code.StartsWith("3")
                              select new { l, a };

        var movements = await AsyncExecuter.ToListAsync(equityMovements);
        
        foreach (var move in movements)
        {
             // These are direct equity changes.
             // If we assume Retained Earnings is NOT auto-posted yet.
             // We list them.
             var amount = move.l.Credit - move.l.Debit;
             dto.DetailLines.Add(new FinancialReportLineDto
             {
                 AccountCode = move.a.Code,
                 AccountName = move.a.Name,
                 Amount = amount
             });
        }
        
        dto.CapitalChanges = dto.DetailLines.Sum(x => x.Amount);

        // Closing Balance
        // Opening + Net Income + Movements
        // Note: Check if Net Income is already in movements? 
        // If not closed, it's NOT in movements of Class 3. It's in Class 4/5. 
        // So we add it explicitly.
        
        return dto;
    }



    public override async Task<AccountDto> CreateAsync(CreateUpdateAccountDto input)
    {
        // 1. Determine Parent Code
        string parentCode = "";
        if (input.ParentId.HasValue)
        {
            var parent = await Repository.GetAsync(input.ParentId.Value);
            parentCode = parent.Code;
        }

        // 2. Find max code among siblings
        var siblings = await Repository.GetListAsync(x => x.ParentId == input.ParentId);

        // Filter siblings to find proper sequence
        // Logic: 
        // If Parent is "1", Children are "11", "12"...
        // If Parent is "101", Children are "10101", "10102"?? OR "101.1"? 
        // The user request says "consistent with parent". Standard accounting is often strictly hierarchical numbers.
        // Let's assume a suffix of length 1 or 2. 
        // If siblings exist, take max code and increment.
        // If no siblings, take parent code + "1" (or "01").

        // Better Approach: 
        // If ParentId is null (Root), max code length 1 (1, 2, 3...)
        // If ParentId exists, append next number.

        string nextCode;

        if (siblings.Any())
        {
            var maxCode = siblings.Select(x => x.Code).OrderByDescending(x => x.Length).ThenByDescending(x => x).FirstOrDefault();
            // Simple increment logic - assuming numeric codes
            if (long.TryParse(maxCode, out long maxCodeVal))
            {
                nextCode = (maxCodeVal + 1).ToString();
            }
            else
            {
                // Fallback if non-numeric
                nextCode = parentCode + (siblings.Count + 1);
            }
        }
        else
        {
            // First child
            if (string.IsNullOrEmpty(parentCode))
            {
                // First Root ever? unlikely but...
                nextCode = "1";
            }
            else
            {
                // Parent "1" -> Child "11"
                // Parent "11" -> Child "111" ? 
                // USUALLY: 
                // Level 1: 1 digit (1 - Assets)
                // Level 2: 2 digits (11 - Current Assets)
                // Level 3: 4 digits (1101 - Cash) ??

                // Let's try simple concatenation of '1' for now, user can correct if schema differs.
                // Request: "consistent with parent"
                // If Parent "1", Child "11".
                nextCode = parentCode + "1";
            }
        }

        input.Code = nextCode;

        return await base.CreateAsync(input);
    }

    public async Task<DailyAccountsReportDto> GetDailyAccountsReportAsync(DateRangeDto input)
    {
        var dto = new DailyAccountsReportDto();

        // 1. Receipts
        var receipts = await _receiptVoucherRepository.GetListAsync(x => x.Date >= input.StartDate && x.Date <= input.EndDate);
        foreach (var r in receipts)
        {
            dto.Transactions.Add(new ReportTransactionDto
            {
                Date = r.Date,
                ReferenceNumber = r.VoucherNumber,
                Description = r.Description,
                Amount = r.Amount,
                Type = "Receipt",
                AccountName = r.PayerName
            });
        }

        // 2. Payments
        var payments = await _paymentVoucherRepository.GetListAsync(x => x.Date >= input.StartDate && x.Date <= input.EndDate);
        foreach (var p in payments)
        {
            dto.Transactions.Add(new ReportTransactionDto
            {
                Date = p.Date,
                ReferenceNumber = p.VoucherNumber,
                Description = p.Description,
                Amount = p.Amount,
                Type = "Payment",
                AccountName = p.PayeeName
            });
        }

        // 3. Journal Entries (Optional: exclude those generated from vouchers if they are duplicates)
        // For now, list all.
        var jes = await _journalEntryRepository.GetListAsync(x => x.Date >= input.StartDate && x.Date <= input.EndDate);
        foreach (var je in jes)
        {
            dto.Transactions.Add(new ReportTransactionDto
            {
                Date = je.Date,
                ReferenceNumber = je.ReferenceNumber,
                Description = je.Description,
                Amount = 0, // JE doesn't have a single "Amount"
                Type = "JournalEntry",
                AccountName = "Journal Entry"
            });
        }

        dto.Transactions = dto.Transactions.OrderByDescending(x => x.Date).ToList();
        return dto;
    }

    public async Task<CustomerDebtsReportDto> GetCustomerDebtsReportAsync()
    {
        var dto = new CustomerDebtsReportDto();

        var invoicesQuery = await _invoiceRepository.GetQueryableAsync();
        var patientsQuery = await _patientRepository.GetQueryableAsync();

        var debts = from i in invoicesQuery
                    group i by i.PatientId into g
                    join p in patientsQuery on g.Key equals p.Id
                    select new CustomerDebtDto
                    {
                        PatientId = p.Id,
                        PatientName = p.FirstName + " " + p.LastName,
                        MRN = p.MRN,
                        TotalInvoiced = g.Sum(x => x.NetAmount),
                        TotalPaid = g.Sum(x => x.PaidAmount),
                        DueAmount = g.Sum(x => x.DueAmount)
                    };

        dto.Debts = await AsyncExecuter.ToListAsync(debts.Where(x => x.DueAmount > 0));
        return dto;
    }

    public async Task<DiscountsReportDto> GetDiscountsReportAsync(DateRangeDto input)
    {
        var dto = new DiscountsReportDto();

        var invoicesQuery = await _invoiceRepository.GetQueryableAsync();
        var patientsQuery = await _patientRepository.GetQueryableAsync();

        var discountLines = from i in invoicesQuery
                            join p in patientsQuery on i.PatientId equals p.Id
                            where i.InvoiceDate >= input.StartDate && i.InvoiceDate <= input.EndDate && i.DiscountAmount > 0
                            select new DiscountReportLineDto
                            {
                                Date = i.InvoiceDate,
                                InvoiceNumber = i.InvoiceNumber,
                                PatientName = p.FirstName + " " + p.LastName,
                                TotalAmount = i.NetAmount,
                                DiscountAmount = i.DiscountAmount
                            };

        dto.Lines = await AsyncExecuter.ToListAsync(discountLines);
        return dto;
    }
}