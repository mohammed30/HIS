using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Settings;
using Volo.Abp.SettingManagement;

namespace HIS.Settings;

[Authorize]
public class PharmacySettingsAppService : ApplicationService, IPharmacySettingsAppService
{
    private readonly ISettingManager _settingManager;

    public PharmacySettingsAppService(ISettingManager settingManager)
    {
        _settingManager = settingManager;
    }

    public async Task<PharmacySettingsDto> GetAsync()
    {
        return new PharmacySettingsDto
        {
            AllowNegativeStock = await SettingProvider.GetAsync<bool>(HISSettings.Pharmacy.AllowNegativeStock)
        };
    }

    public async Task UpdateAsync(PharmacySettingsDto input)
    {
        await _settingManager.SetAsync(HISSettings.Pharmacy.AllowNegativeStock, input.AllowNegativeStock.ToString().ToLower(), GlobalSettingValueProvider.ProviderName, null);
    }
}
