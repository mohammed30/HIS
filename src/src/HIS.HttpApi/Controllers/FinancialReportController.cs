using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace HIS.Accounting;

[RemoteService(Name = "FinancialReport")]
[Area("app")]
[Route("api/app/financial-reports")]
public class FinancialReportController : AbpControllerBase
{
    protected IAccountAppService AccountAppService { get; }

    public FinancialReportController(IAccountAppService accountAppService)
    {
        AccountAppService = accountAppService;
    }

    [HttpGet("income-statement")]
    public virtual Task<IncomeStatementDto> GetIncomeStatementAsync(DateRangeDto input)
    {
        return AccountAppService.GetIncomeStatementAsync(input);
    }

    [HttpGet("balance-sheet")]
    public virtual Task<BalanceSheetDto> GetBalanceSheetAsync(DateRangeDto input)
    {
        return AccountAppService.GetBalanceSheetAsync(input);
    }

    [HttpGet("cash-flow-statement")]
    public virtual Task<CashFlowStatementDto> GetCashFlowStatementAsync(DateRangeDto input)
    {
        return AccountAppService.GetCashFlowStatementAsync(input);
    }

    [HttpGet("changes-in-equity")]
    public virtual Task<ChangesInEquityDto> GetChangesInEquityAsync(DateRangeDto input)
    {
        return AccountAppService.GetChangesInEquityAsync(input);
    }
}
