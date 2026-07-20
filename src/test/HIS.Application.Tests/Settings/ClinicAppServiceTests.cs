using System;
using System.Threading.Tasks;
using HIS.Settings;
using Shouldly;
using Xunit;

namespace HIS.Settings.Tests;

public abstract class ClinicAppServiceTests<TStartupModule> : SettingsTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly IClinicAppService _clinicAppService;

    protected ClinicAppServiceTests()
    {
        _clinicAppService = GetRequiredService<IClinicAppService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateClinic()
    {
        // Arrange
        var department = await CreateDepartmentAsync();

        var input = new CreateUpdateClinicDto
        {
            Code = "CL-001",
            NameAr = "عيادة القلب",
            NameEn = "Cardiology Clinic",
            DepartmentId = department.Id,
            Location = "Floor 2",
            RoomNumber = "201",
            Capacity = 5,
            AppointmentDuration = 15,
            ConsultationFee = 100,
            IsActive = true
        };

        // Act
        var result = await _clinicAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.NameAr.ShouldBe(input.NameAr);
        result.DepartmentId.ShouldBe(department.Id);
        result.ConsultationFee.ShouldBe(input.ConsultationFee);
    }
}
