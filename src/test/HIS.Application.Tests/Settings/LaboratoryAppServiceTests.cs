using System;
using System.Threading.Tasks;
using HIS.Settings;
using Shouldly;
using Xunit;

namespace HIS.Settings.Tests;

public abstract class LaboratoryAppServiceTests<TStartupModule> : SettingsTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly ILaboratoryAppService _laboratoryAppService;

    protected LaboratoryAppServiceTests()
    {
        _laboratoryAppService = GetRequiredService<ILaboratoryAppService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateLaboratory()
    {
        // Arrange
        var input = new CreateUpdateLaboratoryDto
        {
            Code = "LAB-001",
            NameAr = "المختبر الرئيسي",
            NameEn = "Main Lab",
            IsActive = true,
            Is24Hours = true
        };

        // Act
        var result = await _laboratoryAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.NameAr.ShouldBe(input.NameAr);
        result.Is24Hours.ShouldBeTrue();
    }
}
