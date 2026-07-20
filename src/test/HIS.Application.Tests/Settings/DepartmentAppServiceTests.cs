using System;
using System.Threading.Tasks;
using HIS.Settings;
using Shouldly;
using Xunit;

namespace HIS.Settings.Tests;

public abstract class DepartmentAppServiceTests<TStartupModule> : SettingsTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly IDepartmentAppService _departmentAppService;

    protected DepartmentAppServiceTests()
    {
        _departmentAppService = GetRequiredService<IDepartmentAppService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDepartment()
    {
        // Arrange
        var input = new CreateUpdateDepartmentDto
        {
            Code = "DEP-001",
            NameAr = "قسم الجراحة",
            NameEn = "Surgery Department",
            IsActive = true,
            IsMedical = true
        };

        // Act
        var result = await _departmentAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.NameAr.ShouldBe(input.NameAr);
        result.IsMedical.ShouldBeTrue();
    }
}
