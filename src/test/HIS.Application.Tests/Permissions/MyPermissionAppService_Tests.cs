using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.PermissionManagement;
using Xunit;
using NSubstitute;
using Microsoft.Extensions.Options;
using Volo.Abp.SimpleStateChecking;

namespace HIS.Permissions
{
    public class MyPermissionAppService_Tests
    {
        private readonly MyPermissionAppService _permissionAppService;

        public MyPermissionAppService_Tests()
        {
            var permissionManager = Substitute.For<IPermissionManager>();
            var permissionChecker = Substitute.For<IPermissionChecker>();
            var permissionDefManager = Substitute.For<IPermissionDefinitionManager>();
            var resourcePermissionManager = Substitute.For<IResourcePermissionManager>();
            var resourceGrantRepo = Substitute.For<IResourcePermissionGrantRepository>();
            var options = Substitute.For<IOptions<PermissionManagementOptions>>();
            options.Value.Returns(new PermissionManagementOptions());
            var stateChecker = Substitute.For<ISimpleStateCheckerManager<PermissionDefinition>>();

            _permissionAppService = new MyPermissionAppService(
                permissionManager,
                permissionChecker,
                permissionDefManager,
                resourcePermissionManager,
                resourceGrantRepo,
                options,
                stateChecker
            );
        }

        [Fact]
        public void Logic_Test_Should_Not_Crash()
        {
            // Just verifying it compiles and instantiates
            _permissionAppService.ShouldNotBeNull();
        }
    }
}
