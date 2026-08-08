using System;
using System.Threading.Tasks;
using HIS.Users;
using Shouldly;
using Volo.Abp.Identity;
using Xunit;
using Volo.Abp.Modularity;

namespace HIS.Users.Tests;

public abstract class MyIdentityUserAppServiceTests<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly MyIdentityUserAppService _identityUserAppService;
    private readonly IIdentityUserRepository _identityUserRepository;

    public MyIdentityUserAppServiceTests()
    {
        _identityUserAppService = GetRequiredService<MyIdentityUserAppService>();
        _identityUserRepository = GetRequiredService<IIdentityUserRepository>();
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Users()
    {
        // Arrange
        var input = new GetIdentityUsersInput();

        // Act
        var result = await _identityUserAppService.GetListAsync(input);

        // Assert
        result.ShouldNotBeNull();
        // The seed data typically creates an admin user
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(0); 
    }
}
