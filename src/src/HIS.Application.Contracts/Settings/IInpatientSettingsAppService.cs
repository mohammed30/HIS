using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HIS.Settings;

public interface IInpatientSettingsAppService : IApplicationService
{
    Task<InpatientSettingsDto> GetAsync();
    Task UpdateAsync(InpatientSettingsDto input);
}
