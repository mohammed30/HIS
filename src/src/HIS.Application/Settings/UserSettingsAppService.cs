using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.SettingManagement;
using Volo.Abp.Users;

namespace HIS.Settings;

[Authorize]
public class UserSettingsAppService : ApplicationService, IUserSettingsAppService
{
    private readonly ISettingManager _settingManager;

    public UserSettingsAppService(ISettingManager settingManager)
    {
        _settingManager = settingManager;
    }

    public async Task SetThemeAsync(string theme)
    {
        if (CurrentUser.Id.HasValue)
        {
            await _settingManager.SetForUserAsync(CurrentUser.Id.Value, HISSettings.User.Theme, theme);
        }
    }
}
