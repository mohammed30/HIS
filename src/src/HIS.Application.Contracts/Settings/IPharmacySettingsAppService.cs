using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HIS.Settings;

public interface IPharmacySettingsAppService : IApplicationService
{
    Task<PharmacySettingsDto> GetAsync();
    Task UpdateAsync(PharmacySettingsDto input);
}
