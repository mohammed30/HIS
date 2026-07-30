using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Reports
{
    public interface IUserFinancialReportAppService : IApplicationService
    {
        Task<PagedResultDto<UserFinancialTransactionDto>> GetListAsync(GetUserFinancialTransactionsInput input);
    }
}
