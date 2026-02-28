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
using Microsoft.AspNetCore.Hosting;
using HIS.Accounting.Printing;

namespace HIS.Accounting;

public class AccountAppService : CrudAppService<Account, AccountDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateAccountDto>, IAccountAppService
{
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<Invoice, Guid> _invoiceRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<ReceiptVoucher, Guid> _receiptVoucherRepository;
    private readonly IRepository<PaymentVoucher, Guid> _paymentVoucherRepository;
    private readonly IRepository<JournalEntryLine, Guid> _journalEntryLineRepository;
    private readonly IWebHostEnvironment _env;

    public AccountAppService(
        IRepository<Account, Guid> repository,
        IRepository<JournalEntry, Guid> journalEntryRepository,
        IRepository<Invoice, Guid> invoiceRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<ReceiptVoucher, Guid> receiptVoucherRepository,
        IRepository<PaymentVoucher, Guid> paymentVoucherRepository,
        IRepository<JournalEntryLine, Guid> journalEntryLineRepository,
        IWebHostEnvironment env)
        : base(repository)
    {
        _journalEntryRepository = journalEntryRepository;
        _invoiceRepository = invoiceRepository;
        _patientRepository = patientRepository;
        _receiptVoucherRepository = receiptVoucherRepository;
        _paymentVoucherRepository = paymentVoucherRepository;
        _journalEntryLineRepository = journalEntryLineRepository;
        _env = env;
    }

    public async Task<IncomeStatementDto> GetIncomeStatementAsync(DateRangeDto input)
    {
        var (startDate, endDate) = GetNormalizedDateRange(input);
        
        var query = await _journalEntryRepository.GetQueryableAsync();
        var lines = query
            .Where(x => x.Date >= startDate && x.Date <= endDate)
            .SelectMany(x => x.Lines);

        var accountQuery = await Repository.GetQueryableAsync();
        
        var joined = from l in lines
                     join a in accountQuery on l.AccountId equals a.Id
                     where a.Code.StartsWith("4") || a.Code.StartsWith("5")
                     select new { l, a };
                     
        var result = await AsyncExecuter.ToListAsync(joined);

        var grouped = result
            .GroupBy(x => new { x.a.Code, x.a.Name, x.a.NameAr })
            .Select(g => new
            {
                g.Key.Code,
                Name = !string.IsNullOrEmpty(g.Key.NameAr) ? g.Key.NameAr : g.Key.Name,
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
                if (item.Code.StartsWith("48") || item.Code.StartsWith("49"))
                    dto.OtherRevenueLines.Add(line);
                else
                    dto.RevenueLines.Add(line);
            }
            else if (item.Code.StartsWith("50"))
            {
                // Cost of Sales (Direct Costs)
                dto.CostOfSalesLines.Add(line);
            }
            else if (item.Code.StartsWith("5"))
            {
                if (item.Code.StartsWith("58") || item.Code.StartsWith("59"))
                    dto.OtherExpenseLines.Add(line);
                else
                    // General and Admin Expenses
                    dto.GeneralAndAdminExpenseLines.Add(line);
            }
        }

        dto.TotalRevenue = dto.RevenueLines.Sum(x => x.Amount);
        dto.TotalCostOfSales = dto.CostOfSalesLines.Sum(x => x.Amount);
        dto.TotalGeneralAndAdminExpenses = dto.GeneralAndAdminExpenseLines.Sum(x => x.Amount);
        dto.TotalOtherRevenues = dto.OtherRevenueLines.Sum(x => x.Amount);
        dto.TotalOtherExpenses = dto.OtherExpenseLines.Sum(x => x.Amount);
        
        return dto;
    }

    [HttpGet]
    [Route("api/app/account/income-statement-pdf")]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetIncomeStatementPdfAsync(DateTime startDate, DateTime endDate)
    {
        var input = new DateRangeDto { StartDate = startDate, EndDate = endDate };
        var data = await GetIncomeStatementAsync(input);

        byte[] logoBytes = null;
        var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? "", "images", "logo", "Dark.png");
        if (!System.IO.File.Exists(logoPath))
        {
            var devPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "Dark.png");
            if (System.IO.File.Exists(devPath)) logoPath = devPath;
        }
        if (System.IO.File.Exists(logoPath)) logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);

        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

        var doc = new IncomeStatementDocument
        {
            StartDate = startDate,
            EndDate = endDate,
            PrintedBy = CurrentUser.UserName ?? "System",
            PrintedAt = Clock.Now,
            LogoBytes = logoBytes,
            TotalRevenue = data.TotalRevenue,
            TotalCostOfSales = data.TotalCostOfSales,
            TotalGaExpenses = data.TotalGeneralAndAdminExpenses,
            TotalOtherRevenues = data.TotalOtherRevenues,
            TotalOtherExpenses = data.TotalOtherExpenses,
            RevenueLines = data.RevenueLines.Select(l => new IncomeStatementDocument.ReportLine
            {
                AccountCode = l.AccountCode,
                AccountName = l.AccountName,
                Amount = l.Amount
            }).ToList(),
            CostOfSalesLines = data.CostOfSalesLines.Select(l => new IncomeStatementDocument.ReportLine
            {
                AccountCode = l.AccountCode,
                AccountName = l.AccountName,
                Amount = l.Amount
            }).ToList(),
            GaExpenseLines = data.GeneralAndAdminExpenseLines.Select(l => new IncomeStatementDocument.ReportLine
            {
                AccountCode = l.AccountCode,
                AccountName = l.AccountName,
                Amount = l.Amount
            }).ToList(),
            OtherRevenueLines = data.OtherRevenueLines.Select(l => new IncomeStatementDocument.ReportLine
            {
                AccountCode = l.AccountCode,
                AccountName = l.AccountName,
                Amount = l.Amount
            }).ToList(),
            OtherExpenseLines = data.OtherExpenseLines.Select(l => new IncomeStatementDocument.ReportLine
            {
                AccountCode = l.AccountCode,
                AccountName = l.AccountName,
                Amount = l.Amount
            }).ToList()
        };

        var pdf = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(doc);
        var stream = new System.IO.MemoryStream(pdf);
        var printTime = Clock.Now;
        var fileName = $"قائمة_الدخل_{printTime:yyyy-MM-dd_HH-mm-ss}.pdf";
        return new Volo.Abp.Content.RemoteStreamContent(stream, fileName, "application/pdf");
    }

    public async Task<BalanceSheetDto> GetBalanceSheetAsync(DateRangeDto input)
    {
        // Balance Sheet is "As Of". But using DateRange endDate.
        var startDate = input.StartDate;
        var endDate = input.EndDate;

        var query = await _journalEntryRepository.GetQueryableAsync();
        
        var lines = query
            .Where(x => x.Date <= endDate)
            .SelectMany(x => x.Lines);
            
        var prevLines = query
            .Where(x => x.Date < startDate)
            .SelectMany(x => x.Lines);

        var accountQuery = await Repository.GetQueryableAsync();

        var joined = from l in lines
                     join a in accountQuery on l.AccountId equals a.Id
                     where a.Code.StartsWith("1") || a.Code.StartsWith("2") || a.Code.StartsWith("3")
                     select new { l, a };
                     
        var prevJoined = from l in prevLines
                         join a in accountQuery on l.AccountId equals a.Id
                         where a.Code.StartsWith("1") || a.Code.StartsWith("2") || a.Code.StartsWith("3")
                         select new { l, a };

        var result = await AsyncExecuter.ToListAsync(joined);
        var prevResult = await AsyncExecuter.ToListAsync(prevJoined);

        var grouped = result
            .GroupBy(x => new { x.a.Code, x.a.Name, x.a.NameAr })
            .Select(g => new
            {
                g.Key.Code,
                Name = !string.IsNullOrEmpty(g.Key.NameAr) ? g.Key.NameAr : g.Key.Name,
                Amount = g.Sum(x => x.l.Debit - x.l.Credit) // Assets are Debit Normal. Amount > 0 is Debit.
            })
            .ToList();
            
        var prevGrouped = prevResult
            .GroupBy(x => new { x.a.Code, x.a.Name, x.a.NameAr })
            .Select(g => new
            {
                g.Key.Code,
                Name = !string.IsNullOrEmpty(g.Key.NameAr) ? g.Key.NameAr : g.Key.Name,
                Amount = g.Sum(x => x.l.Debit - x.l.Credit) // Assets are Debit Normal. Amount > 0 is Debit.
            })
            .ToList();

        var dto = new BalanceSheetDto();
        
        var accountKeys = grouped.Select(x => x.Code).Union(prevGrouped.Select(x => x.Code)).Distinct().OrderBy(x => x).ToList();

        foreach (var code in accountKeys)
        {
            var currItem = grouped.FirstOrDefault(x => x.Code == code);
            var prevItem = prevGrouped.FirstOrDefault(x => x.Code == code);
            
            var name = currItem?.Name ?? prevItem?.Name;
            var currAmount = currItem?.Amount ?? 0;
            var prevAmount = prevItem?.Amount ?? 0;

            var line = new FinancialReportLineDto
            {
                AccountCode = code,
                AccountName = name,
                Amount = currAmount,
                PreviousAmount = prevAmount
            };

            if (code.StartsWith("1"))
            {
                dto.AssetLines.Add(line); // Debit (Positive) is good.
            }
            else if (code.StartsWith("2"))
            {
                // Liability. Credit Normal.
                line.Amount = -line.Amount;
                line.PreviousAmount = -line.PreviousAmount;
                dto.LiabilityLines.Add(line);
            }
            else if (code.StartsWith("3"))
            {
                // Equity. Credit Normal.
                line.Amount = -line.Amount;
                line.PreviousAmount = -line.PreviousAmount;
                dto.EquityLines.Add(line);
            }
        }

        dto.TotalAssets = dto.AssetLines.Sum(x => x.Amount);
        dto.TotalLiabilities = dto.LiabilityLines.Sum(x => x.Amount);
        dto.TotalEquity = dto.EquityLines.Sum(x => x.Amount);
        
        dto.TotalPreviousAssets = dto.AssetLines.Sum(x => x.PreviousAmount);
        dto.TotalPreviousLiabilities = dto.LiabilityLines.Sum(x => x.PreviousAmount);
        dto.TotalPreviousEquity = dto.EquityLines.Sum(x => x.PreviousAmount);

        // PreviousYearEquity is retained for consistency if needed by other components, but we already have TotalPreviousEquity now.
        var prevEquity = prevGrouped.Where(x => x.Code.StartsWith("3")).Sum(x => -x.Amount); // Credit normal
        dto.PreviousYearEquity = prevEquity;
        
        return dto;
    }

    [HttpGet]
    [Route("api/app/account/balance-sheet-pdf")]
    public async Task<Volo.Abp.Content.IRemoteStreamContent> GetBalanceSheetPdfAsync(DateTime startDate, DateTime endDate)
    {
        var input = new DateRangeDto { StartDate = startDate, EndDate = endDate };
        var data = await GetBalanceSheetAsync(input);

        // Logo
        byte[] logoBytes = null;
        var logoPath = System.IO.Path.Combine(_env.WebRootPath ?? "", "images", "logo", "Dark.png");
        if (!System.IO.File.Exists(logoPath))
        {
            var devPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "images", "logo", "Dark.png");
            if (System.IO.File.Exists(devPath)) logoPath = devPath;
        }
        if (System.IO.File.Exists(logoPath))
            logoBytes = await System.IO.File.ReadAllBytesAsync(logoPath);

        

        var doc = new HIS.Accounting.Printing.BalanceSheetDocument
        {
            AsOfDate = endDate,
            PrintedBy = CurrentUser.UserName ?? CurrentUser.Name,
            PrintedAt = Clock.Now,
            LogoBytes = logoBytes,
            AssetLines = data.AssetLines.Select(l => new HIS.Accounting.Printing.BalanceSheetDocument.ReportLine
            {
                AccountCode = l.AccountCode,
                AccountName = l.AccountName,
                Amount = l.Amount,
                PreviousAmount = l.PreviousAmount
            }).ToList(),
            LiabilityLines = data.LiabilityLines.Select(l => new HIS.Accounting.Printing.BalanceSheetDocument.ReportLine
            {
                AccountCode = l.AccountCode,
                AccountName = l.AccountName,
                Amount = l.Amount,
                PreviousAmount = l.PreviousAmount
            }).ToList(),
            EquityLines = data.EquityLines.Select(l => new HIS.Accounting.Printing.BalanceSheetDocument.ReportLine
            {
                AccountCode = l.AccountCode,
                AccountName = l.AccountName,
                Amount = l.Amount,
                PreviousAmount = l.PreviousAmount
            }).ToList(),
            TotalAssets = data.TotalAssets,
            TotalLiabilities = data.TotalLiabilities,
            TotalEquity = data.TotalEquity,
            TotalPreviousAssets = data.TotalPreviousAssets,
            TotalPreviousLiabilities = data.TotalPreviousLiabilities,
            TotalPreviousEquity = data.TotalPreviousEquity
        };

        var pdf = QuestPDF.Fluent.GenerateExtensions.GeneratePdf(doc);
        var stream = new System.IO.MemoryStream(pdf);
        var printTime = Clock.Now;
        var fileName = $"قائمة_عمومية_{endDate:yyyy-MM-dd}_{printTime:HH-mm-ss}.pdf";
        return new Volo.Abp.Content.RemoteStreamContent(stream, fileName, "application/pdf");
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
            AccountName = "صافي الربح", 
            Amount = netIncome 
        });

        var (startDate, endDate) = GetNormalizedDateRange(input);
        var query = await _journalEntryRepository.GetQueryableAsync();
        var lines = query
            .Where(x => x.Date >= startDate && x.Date <= endDate)
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
            .GroupBy(x => new { x.a.Code, x.a.Name, x.a.NameAr })
            .Select(g => new
            {
                g.Key.Code,
                Name = !string.IsNullOrEmpty(g.Key.NameAr) ? g.Key.NameAr : g.Key.Name,
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
        
        // Calculate Cash at Beginning
        var prevLines = query.Where(x => x.Date < startDate).SelectMany(x => x.Lines);
        var cashAtBeginning = await AsyncExecuter.SumAsync(
            from l in prevLines
            join a in accountQuery on l.AccountId equals a.Id
            where a.Code.StartsWith("10")
            select (decimal?)(l.Debit - l.Credit) // Cash is Debit normal
        ) ?? 0;
        
        dto.CashAtBeginning = cashAtBeginning;
        dto.CashAtEnd = dto.CashAtBeginning + dto.NetCashFlow;
        
        return dto;
    }

    public async Task<ChangesInEquityDto> GetChangesInEquityAsync(DateRangeDto input)
    {
        var dto = new ChangesInEquityDto();
        var (startDate, endDate) = GetNormalizedDateRange(input);
        
        var query = await _journalEntryRepository.GetQueryableAsync();
        var accountQuery = await Repository.GetQueryableAsync();

        // Previous Periods Calculations (Before StartDate)
        var prevLines = query.Where(x => x.Date < startDate).SelectMany(x => x.Lines);
        var prevEquityData = await AsyncExecuter.ToListAsync(
            from l in prevLines
            join a in accountQuery on l.AccountId equals a.Id
            where a.Code.StartsWith("3")
            select new { Code = a.Code, Amount = l.Credit - l.Debit } // Credit normal
        );
        
        var prevIncomeData = await AsyncExecuter.ToListAsync(
            from l in prevLines
            join a in accountQuery on l.AccountId equals a.Id
            where a.Code.StartsWith("4") || a.Code.StartsWith("5")
            select new { Code = a.Code, Amount = l.Credit - l.Debit } // Revenue(Cr) - Expense(Dr)
        );

        // Previous Capital (31)
        dto.Capital.PreviousYear = prevEquityData.Where(x => x.Code.StartsWith("31")).Sum(x => x.Amount);
        
        // Previous Retained Earnings (32 + all previous Net Income)
        var previousRetainedEarningsDirect = prevEquityData.Where(x => x.Code.StartsWith("32")).Sum(x => x.Amount);
        var previousNetIncomeAccumulated = prevIncomeData.Sum(x => x.Amount);
        dto.RetainedEarnings.PreviousYear = previousRetainedEarningsDirect + previousNetIncomeAccumulated;
        
        // Previous Dividends (33)
        dto.Dividends.PreviousYear = prevEquityData.Where(x => x.Code.StartsWith("33")).Sum(x => x.Amount);
        
        // Net Income Previous Year is typically 0 in this matrix (it gets rolled into RE)
        dto.NetIncome.PreviousYear = 0;

        // Current Period Changes
        var periodLines = query.Where(x => x.Date >= startDate && x.Date <= endDate).SelectMany(x => x.Lines);
        var periodEquityData = await AsyncExecuter.ToListAsync(
            from l in periodLines
            join a in accountQuery on l.AccountId equals a.Id
            where a.Code.StartsWith("3")
            select new { Code = a.Code, Amount = l.Credit - l.Debit }
        );

        // Changes
        dto.Capital.Change = periodEquityData.Where(x => x.Code.StartsWith("31")).Sum(x => x.Amount);
        dto.RetainedEarnings.Change = periodEquityData.Where(x => x.Code.StartsWith("32")).Sum(x => x.Amount);
        dto.Dividends.Change = periodEquityData.Where(x => x.Code.StartsWith("33")).Sum(x => x.Amount);
        
        // Current Period Net Income
        var incomeStatement = await GetIncomeStatementAsync(input);
        dto.NetIncome.Change = incomeStatement.NetIncome;

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
        var (startDate, endDate) = GetNormalizedDateRange(input);

        // 1. Receipts
        var receipts = await _receiptVoucherRepository.GetListAsync(x => x.Date >= startDate && x.Date <= endDate);
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
        var payments = await _paymentVoucherRepository.GetListAsync(x => x.Date >= startDate && x.Date <= endDate);
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
        var jes = await _journalEntryRepository.GetListAsync(
            x => x.Date >= startDate && x.Date <= endDate
        );
        
        var jeIds = jes.Select(x => x.Id).ToList();
        var allJeLines = await _journalEntryLineRepository.GetListAsync(x => jeIds.Contains(x.JournalEntryId));

        foreach (var je in jes)
        {
            var lines = allJeLines.Where(x => x.JournalEntryId == je.Id).ToList();
            var amount = lines.Sum(l => l.Debit); // Sum of debits for the entry volume
            dto.Transactions.Add(new ReportTransactionDto
            {
                Date = je.Date,
                ReferenceNumber = je.ReferenceNumber,
                Description = je.Description,
                Amount = amount,
                Type = "JournalEntry",
                AccountName = "قيد يومية"
            });
        }

        dto.Transactions = dto.Transactions.OrderByDescending(x => x.Date).ToList();
        
        // Explicitly calculate totals since we changed them to auto-properties for serialization
        dto.TotalReceipts = dto.Transactions
            .Where(x => x.Type == "Receipt" || (x.Type == "JournalEntry" && x.Amount > 0))
            .Sum(x => x.Amount);
            
        dto.TotalPayments = dto.Transactions
            .Where(x => x.Type == "Payment" || (x.Type == "JournalEntry" && x.Amount < 0))
            .Sum(x => Math.Abs(x.Amount)); // Ensure payments are positive for display if stored as negative

        return dto;
    }

    public async Task<CustomerDebtsReportDto> GetCustomerDebtsReportAsync()
    {
        var dto = new CustomerDebtsReportDto();

        var invoicesQuery = await _invoiceRepository.GetQueryableAsync();
        var patientsQuery = await _patientRepository.GetQueryableAsync();

        // Aggregate invoices first
        var invoiceTotals = from i in invoicesQuery
                            group i by i.PatientId into g
                            select new
                            {
                                PatientId = g.Key,
                                TotalInvoiced = g.Sum(x => x.NetAmount),
                                TotalPaid = g.Sum(x => x.PaidAmount)
                            };

        // Join with patients and calculate DueAmount
        var query = from t in invoiceTotals
                    join p in patientsQuery on t.PatientId equals p.Id
                    select new CustomerDebtDto
                    {
                        PatientId = p.Id,
                        PatientName = p.FirstNameAr + " " + p.LastNameAr,
                        MRN = p.MRN,
                        TotalInvoiced = t.TotalInvoiced,
                        TotalPaid = t.TotalPaid,
                        DueAmount = t.TotalInvoiced - t.TotalPaid
                    };

        dto.Debts = await AsyncExecuter.ToListAsync(query.Where(x => x.DueAmount > 0));
        return dto;
    }

    public async Task<DiscountsReportDto> GetDiscountsReportAsync(DateRangeDto input)
    {
        var dto = new DiscountsReportDto();

        var (startDate, endDate) = GetNormalizedDateRange(input);
        
        var invoicesQuery = await _invoiceRepository.GetQueryableAsync();
        var patientsQuery = await _patientRepository.GetQueryableAsync();

        var discountLines = from i in invoicesQuery
                            join p in patientsQuery on i.PatientId equals p.Id
                            where i.InvoiceDate >= startDate && i.InvoiceDate <= endDate && i.DiscountAmount > 0
                            select new DiscountReportLineDto
                            {
                                Date = i.InvoiceDate,
                                InvoiceNumber = i.InvoiceNumber,
                                PatientName = p.FirstNameAr + " " + p.LastNameAr,
                                TotalAmount = i.NetAmount,
                                DiscountAmount = i.DiscountAmount
                            };

        dto.Lines = await AsyncExecuter.ToListAsync(discountLines);
        return dto;
    }

    private (DateTime startDate, DateTime endDate) GetNormalizedDateRange(DateRangeDto input)
    {
        // Use the Clock property from ApplicationService
        var start = Clock.Normalize(input.StartDate).Date;
        var end = Clock.Normalize(input.EndDate).Date.AddDays(1).AddTicks(-1);
        return (start, end);
    }

    public async Task<AccountStatementDto> GetAccountStatementAsync(AccountStatementInputDto input)
    {
        var (startDate, endDate) = GetNormalizedDateRange(new DateRangeDto
        {
            StartDate = input.StartDate,
            EndDate = input.EndDate
        });

        if (!input.AccountId.HasValue)
        {
            return new AccountStatementDto();
        }

        var account = await Repository.GetAsync(input.AccountId.Value);

        // Get all descendant leaf account IDs (if this is a parent account)
        var allAccounts = await Repository.GetListAsync();
        var targetAccountIds = GetDescendantIds(account.Id, allAccounts);
        targetAccountIds.Add(account.Id);

        // Opening balance: sum of all posted entries before startDate
        var allEntries = await _journalEntryRepository.GetListAsync();
        var postedEntryIds = allEntries.Where(e => e.IsPosted).Select(e => e.Id).ToHashSet();

        var allLines = await _journalEntryLineRepository.GetListAsync();

        // Filter lines for target accounts and posted entries
        var relevantLines = allLines
            .Where(l => targetAccountIds.Contains(l.AccountId) && postedEntryIds.Contains(l.JournalEntryId))
            .ToList();

        // Opening balance
        var openingLines = relevantLines
            .Where(l => allEntries.First(e => e.Id == l.JournalEntryId).Date < startDate);

        decimal openingBalance = openingLines.Sum(l => l.Debit) - openingLines.Sum(l => l.Credit);

        // Period lines
        var periodLines = relevantLines
            .Where(l =>
            {
                var entry = allEntries.First(e => e.Id == l.JournalEntryId);
                return entry.Date >= startDate && entry.Date <= endDate;
            })
            .Select(l =>
            {
                var entry = allEntries.First(e => e.Id == l.JournalEntryId);
                return new { Line = l, Entry = entry };
            })
            .OrderBy(x => x.Entry.Date)
            .ThenBy(x => x.Entry.ReferenceNumber)
            .ToList();

        var dto = new AccountStatementDto
        {
            AccountCode = account.Code,
            AccountName = account.NameAr ?? account.Name,
            OpeningBalance = openingBalance,
        };

        decimal runningBalance = openingBalance;
        foreach (var item in periodLines)
        {
            runningBalance += item.Line.Debit - item.Line.Credit;
            dto.Lines.Add(new AccountStatementLineDto
            {
                Date = item.Entry.Date,
                ReferenceNumber = item.Entry.ReferenceNumber,
                Description = item.Entry.Description,
                Debit = item.Line.Debit,
                Credit = item.Line.Credit,
                RunningBalance = runningBalance
            });
        }

        dto.TotalDebit = periodLines.Sum(x => x.Line.Debit);
        dto.TotalCredit = periodLines.Sum(x => x.Line.Credit);
        dto.ClosingBalance = runningBalance;

        return dto;
    }

    public async Task<List<AccountSummaryDto>> GetAccountSummaryAsync(DateRangeDto input)
    {
        var (startDate, endDate) = GetNormalizedDateRange(input);

        var allAccounts = await Repository.GetListAsync();
        var allEntries = await _journalEntryRepository.GetListAsync();
        var postedEntryIds = allEntries
            .Where(e => e.IsPosted && e.Date >= startDate && e.Date <= endDate)
            .Select(e => e.Id)
            .ToHashSet();

        var allLines = await _journalEntryLineRepository.GetListAsync();
        var periodLines = allLines.Where(l => postedEntryIds.Contains(l.JournalEntryId)).ToList();

        // Calculate balances per leaf account
        var leafBalances = periodLines
            .GroupBy(l => l.AccountId)
            .ToDictionary(
                g => g.Key,
                g => (Debit: g.Sum(x => x.Debit), Credit: g.Sum(x => x.Credit))
            );

        // Build hierarchical summary: only top-level (parentId == null) accounts
        var topLevelAccounts = allAccounts.Where(a => a.ParentId == null).OrderBy(a => a.Code).ToList();

        var result = new List<AccountSummaryDto>();
        foreach (var parent in topLevelAccounts)
        {
            var summary = BuildAccountSummary(parent, allAccounts, leafBalances);
            if (summary.TotalDebit != 0 || summary.TotalCredit != 0 || summary.Children.Any(c => c.TotalDebit != 0 || c.TotalCredit != 0))
            {
                result.Add(summary);
            }
        }

        return result;
    }

    private AccountSummaryDto BuildAccountSummary(
        Account account,
        List<Account> allAccounts,
        Dictionary<Guid, (decimal Debit, decimal Credit)> leafBalances)
    {
        var children = allAccounts.Where(a => a.ParentId == account.Id).OrderBy(a => a.Code).ToList();

        var dto = new AccountSummaryDto
        {
            AccountId = account.Id,
            AccountCode = account.Code,
            AccountName = account.NameAr ?? account.Name,
            AccountType = account.Type,
            IsParent = children.Any()
        };

        if (!children.Any())
        {
            // Leaf account: get balance directly
            if (leafBalances.TryGetValue(account.Id, out var balance))
            {
                dto.TotalDebit = balance.Debit;
                dto.TotalCredit = balance.Credit;
            }
        }
        else
        {
            // Parent: aggregate children recursively
            foreach (var child in children)
            {
                var childDto = BuildAccountSummary(child, allAccounts, leafBalances);
                dto.Children.Add(childDto);
                dto.TotalDebit += childDto.TotalDebit;
                dto.TotalCredit += childDto.TotalCredit;
            }
        }

        dto.Balance = dto.TotalDebit - dto.TotalCredit;
        return dto;
    }

    private List<Guid> GetDescendantIds(Guid parentId, List<Account> allAccounts)
    {
        var result = new List<Guid>();
        var directChildren = allAccounts.Where(a => a.ParentId == parentId).ToList();
        foreach (var child in directChildren)
        {
            result.Add(child.Id);
            result.AddRange(GetDescendantIds(child.Id, allAccounts));
        }
        return result;
    }
}