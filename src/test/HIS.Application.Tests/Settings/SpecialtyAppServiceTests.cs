using System;
using System.Threading.Tasks;
using HIS.Settings;
using Shouldly;
using Xunit;

namespace HIS.Settings.Tests;

public abstract class SpecialtyAppServiceTests<TStartupModule> : SettingsTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly ISpecialtyAppService _specialtyAppService;

    protected SpecialtyAppServiceTests()
    {
        _specialtyAppService = GetRequiredService<ISpecialtyAppService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateSpecialty()
    {
        // Arrange
        var input = new CreateUpdateSpecialtyDto
        {
            Code = "SPEC-001",
            NameAr = "تخصص القلب",
            NameEn = "Cardiology",
            IsActive = true
        };

        // Act
        var result = await _specialtyAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.NameAr.ShouldBe(input.NameAr);
        result.IsActive.ShouldBeTrue();
    }
}
