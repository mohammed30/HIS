using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SimpleStateChecking;

namespace HIS.Permissions
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IPermissionAppService), typeof(PermissionAppService), typeof(MyPermissionAppService))]
    public class MyPermissionAppService : PermissionAppService
    {
        public MyPermissionAppService(
            IPermissionManager permissionManager, 
            IPermissionChecker permissionChecker,
            IPermissionDefinitionManager permissionDefinitionManager, 
            IResourcePermissionManager resourcePermissionManager,
            IResourcePermissionGrantRepository resourcePermissionGrantRepository,
            IOptions<PermissionManagementOptions> options,
            ISimpleStateCheckerManager<PermissionDefinition> simpleStateCheckerManager) 
            : base(permissionManager, permissionChecker, permissionDefinitionManager, resourcePermissionManager, resourcePermissionGrantRepository, options, simpleStateCheckerManager)
        {
        }

        public override async Task<GetPermissionListResultDto> GetAsync(string providerName, string providerKey)
        {
            var result = await base.GetAsync(providerName, providerKey);

            // Only show ABP groups for admin or AdminStaff. Otherwise, remove them.
            // providerName usually is "R" (Role) or "U" (User)
            // providerKey is the role name or user ID
            if (providerName == "R" && providerKey.ToLowerInvariant() != "admin" && providerKey.ToLowerInvariant() != "adminstaff")
            {
                result.Groups = result.Groups.Where(g => g.Name == "HIS").ToList();
            }

            return result;
        }
    }
}
