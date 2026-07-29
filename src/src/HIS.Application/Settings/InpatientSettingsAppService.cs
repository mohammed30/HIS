using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.SettingManagement;

namespace HIS.Settings;

[Authorize]
public class InpatientSettingsAppService : ApplicationService, IInpatientSettingsAppService
{
    private readonly ISettingManager _settingManager;

    public InpatientSettingsAppService(ISettingManager settingManager)
    {
        _settingManager = settingManager;
    }

    public async Task<InpatientSettingsDto> GetAsync()
    {
        var depositAmount = await SettingProvider.GetOrNullAsync(HISSettings.Inpatient.AdmissionDepositAmount);
        var requireAdvance = await SettingProvider.GetOrNullAsync(HISSettings.Inpatient.RequireAdvancePayment);

        return new InpatientSettingsDto
        {
            AdmissionDepositAmount = decimal.TryParse(depositAmount, out var parsedDeposit) ? parsedDeposit : 1000m,
            RequireAdvancePayment = bool.TryParse(requireAdvance, out var parsedRequire) ? parsedRequire : false
        };
    }

    public async Task UpdateAsync(InpatientSettingsDto input)
    {
        await _settingManager.SetForTenantOrGlobalAsync(
            CurrentTenant.Id, 
            HISSettings.Inpatient.AdmissionDepositAmount, 
            input.AdmissionDepositAmount.ToString());
            
        await _settingManager.SetForTenantOrGlobalAsync(
            CurrentTenant.Id, 
            HISSettings.Inpatient.RequireAdvancePayment, 
            input.RequireAdvancePayment.ToString().ToLowerInvariant());
    }
}
