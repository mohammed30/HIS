using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using HIS.Accounting.Dtos;

namespace HIS.Accounting;

public interface IAccountAppService : ICrudAppService<AccountDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateAccountDto>
{
    Task<IncomeStatementDto> GetIncomeStatementAsync(DateRangeDto input);

    Task<BalanceSheetDto> GetBalanceSheetAsync(DateRangeDto input);

    Task<CashFlowStatementDto> GetCashFlowStatementAsync(DateRangeDto input);

    Task<ChangesInEquityDto> GetChangesInEquityAsync(DateRangeDto input);

    Task<DailyAccountsReportDto> GetDailyAccountsReportAsync(DateRangeDto input);

    Task<CustomerDebtsReportDto> GetCustomerDebtsReportAsync();

    Task<DiscountsReportDto> GetDiscountsReportAsync(DateRangeDto input);

    Task<AccountStatementDto> GetAccountStatementAsync(AccountStatementInputDto input);

    Task<List<AccountSummaryDto>> GetAccountSummaryAsync(DateRangeDto input);

    Task<List<AccountLookupDto>> GetLookupAsync();
}
