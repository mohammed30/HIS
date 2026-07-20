using System;
using System.Threading.Tasks;
using HIS.Settings;
using Shouldly;
using Xunit;

namespace HIS.Settings.Tests;

public abstract class DoctorAppServiceTests<TStartupModule> : SettingsTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly IDoctorAppService _doctorAppService;

    protected DoctorAppServiceTests()
    {
        _doctorAppService = GetRequiredService<IDoctorAppService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDoctor()
    {
        // Arrange
        var department = await CreateDepartmentAsync();
        var specialty = await CreateSpecialtyAsync();

        var input = new CreateUpdateDoctorDto
        {
            Code = "DOC-001",
            NameAr = "د. أحمد",
            NameEn = "Dr. Ahmed",
            DepartmentId = department.Id,
            SpecialtyId = specialty.Id,
            ConsultationFee = 150,
            MorningConsultationFee = 100,
            EveningConsultationFee = 150,
            FollowUpFee = 50,
            AppointmentDuration = 20,
            IsActive = true
        };

        // Act
        var result = await _doctorAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.NameAr.ShouldBe(input.NameAr);
        result.DepartmentId.ShouldBe(department.Id);
        result.SpecialtyId.ShouldBe(specialty.Id);
        result.ConsultationFee.ShouldBe(input.ConsultationFee);
    }
}
