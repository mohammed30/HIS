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
}
