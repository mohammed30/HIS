using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace HIS.Users
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IIdentityRoleAppService), typeof(IdentityRoleAppService))]
    public class MyIdentityRoleAppService : IdentityRoleAppService
    {
        public MyIdentityRoleAppService(
            IdentityRoleManager roleManager,
            IIdentityRoleRepository roleRepository)
            : base(roleManager, roleRepository)
        {
        }

        public override async Task<PagedResultDto<IdentityRoleDto>> GetListAsync(GetIdentityRolesInput input)
        {
            var result = await base.GetListAsync(input);
            
            // Remove 'admin' role from the returned list
            var adminRole = result.Items.FirstOrDefault(r => r.Name?.ToLower() == "admin");
            if (adminRole != null)
            {
                var itemsList = result.Items.ToList();
                itemsList.Remove(adminRole);
                return new PagedResultDto<IdentityRoleDto>(result.TotalCount - 1, itemsList);
            }

            return result;
        }
        
        public override async Task<ListResultDto<IdentityRoleDto>> GetAllListAsync()
        {
            var result = await base.GetAllListAsync();
            
            // Remove 'admin' role from the returned list
            var adminRole = result.Items.FirstOrDefault(r => r.Name?.ToLower() == "admin");
            if (adminRole != null)
            {
                var itemsList = result.Items.ToList();
                itemsList.Remove(adminRole);
                return new ListResultDto<IdentityRoleDto>(itemsList);
            }

            return result;
        }
    }
}
