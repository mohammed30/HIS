using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;
using HIS.Permissions;
using Volo.Abp.Authorization.Permissions;
using System.Linq;

namespace HIS.Radiology;

public class RadiologyDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IPermissionManager _permissionManager;

    public RadiologyDataSeedContributor(IPermissionManager permissionManager)
    {
        _permissionManager = permissionManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // Grant permissions to the admin role
        // The admin role name is usually "admin" in ABP
        
        await GrantPermissionIfNotExistsAsync("R", "admin", HISPermissions.Radiology.Default);
        await GrantPermissionIfNotExistsAsync("R", "admin", HISPermissions.Radiology.Requests);
        
        // Also grant to "AdminStaff" if it exists
        await GrantPermissionIfNotExistsAsync("R", "AdminStaff", HISPermissions.Radiology.Default);
        await GrantPermissionIfNotExistsAsync("R", "AdminStaff", HISPermissions.Radiology.Requests);
    }

    private async Task GrantPermissionIfNotExistsAsync(string providerName, string providerKey, string permissionName)
    {
        var permission = await _permissionManager.GetAsync(permissionName, providerName, providerKey);
        if (permission == null || !permission.IsGranted)
        {
            await _permissionManager.SetAsync(permissionName, providerName, providerKey, true);
        }
    }
}
