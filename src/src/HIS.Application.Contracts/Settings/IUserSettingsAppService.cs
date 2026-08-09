using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HIS.Settings;

public interface IUserSettingsAppService : IApplicationService
{
    Task SetThemeAsync(string theme);
}
