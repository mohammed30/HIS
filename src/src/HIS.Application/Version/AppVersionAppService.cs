using System.Reflection;
using System.Threading.Tasks;

namespace HIS.Version;

public class AppVersionAppService : HISAppService, IAppVersionAppService
{
    public Task<string> GetVersionAsync()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";
        return Task.FromResult(version);
    }
}
