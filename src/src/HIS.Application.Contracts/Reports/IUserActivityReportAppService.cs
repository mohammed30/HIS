using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Reports
{
    public interface IUserActivityReportAppService : IApplicationService
    {
        Task<PagedResultDto<UserActivityFrequencyDto>> GetListAsync(GetUserActivityFrequencyInput input);
    }
}
