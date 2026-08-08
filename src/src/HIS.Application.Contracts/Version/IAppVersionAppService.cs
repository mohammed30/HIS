using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HIS.Version;

public interface IAppVersionAppService : IApplicationService
{
    Task<string> GetVersionAsync();
}
