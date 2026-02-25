using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HIS.Dashboard
{
    public interface IDashboardAppService : IApplicationService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
    }
}
