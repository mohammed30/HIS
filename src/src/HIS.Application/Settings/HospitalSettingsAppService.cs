using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Settings;
using Volo.Abp.SettingManagement;

namespace HIS.Settings;

[Authorize]
public class HospitalSettingsAppService : ApplicationService, IHospitalSettingsAppService
{
    private readonly ISettingManager _settingManager;

    public HospitalSettingsAppService(ISettingManager settingManager)
    {
        _settingManager = settingManager;
    }

    public async Task<HospitalSettingsDto> GetAsync()
    {
        var settings = new HospitalSettingsDto
        {
            HospitalName = await SettingProvider.GetOrNullAsync(HISSettings.HospitalName) ?? string.Empty,
            HospitalAddress = await SettingProvider.GetOrNullAsync(HISSettings.HospitalAddress),
            HospitalPhone = await SettingProvider.GetOrNullAsync(HISSettings.HospitalPhone),
            HospitalEmail = await SettingProvider.GetOrNullAsync(HISSettings.HospitalEmail),
            HospitalLogo = await SettingProvider.GetOrNullAsync(HISSettings.HospitalLogo),
            HospitalTaxNumber = await SettingProvider.GetOrNullAsync(HISSettings.HospitalTaxNumber)
        };

        return settings;
    }

    public async Task UpdateAsync(HospitalSettingsDto input)
    {
        await _settingManager.SetAsync(HISSettings.HospitalName, input.HospitalName, GlobalSettingValueProvider.ProviderName, null);
        await _settingManager.SetAsync(HISSettings.HospitalAddress, input.HospitalAddress, GlobalSettingValueProvider.ProviderName, null);
        await _settingManager.SetAsync(HISSettings.HospitalPhone, input.HospitalPhone, GlobalSettingValueProvider.ProviderName, null);
        await _settingManager.SetAsync(HISSettings.HospitalEmail, input.HospitalEmail, GlobalSettingValueProvider.ProviderName, null);
        await _settingManager.SetAsync(HISSettings.HospitalLogo, input.HospitalLogo, GlobalSettingValueProvider.ProviderName, null);
        await _settingManager.SetAsync(HISSettings.HospitalTaxNumber, input.HospitalTaxNumber, GlobalSettingValueProvider.ProviderName, null);
    }
}
