using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.Authorization.Permissions;

namespace HIS.Users
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IIdentityUserAppService), typeof(IdentityUserAppService))]
    public class MyIdentityUserAppService : IdentityUserAppService
    {
        public MyIdentityUserAppService(
            IdentityUserManager userManager,
            IIdentityUserRepository userRepository,
            IIdentityRoleRepository roleRepository,
            IOptions<IdentityOptions> identityOptions,
            IPermissionChecker permissionChecker)
            : base(userManager, userRepository, roleRepository, identityOptions, permissionChecker)
        {
        }

        public override async Task<PagedResultDto<IdentityUserDto>> GetListAsync(GetIdentityUsersInput input)
        {
            var result = await base.GetListAsync(input);
            
            // Remove 'admin' from the returned list
            var adminUser = result.Items.FirstOrDefault(u => u.UserName?.ToLower() == "admin");
            if (adminUser != null)
            {
                var itemsList = result.Items.ToList();
                itemsList.Remove(adminUser);
                return new PagedResultDto<IdentityUserDto>(result.TotalCount - 1, itemsList);
            }

            return result;
        }
    }
}
