using System;
using System.Threading.Tasks;
using HIS.Insurance;
using Shouldly;
using Xunit;
using Volo.Abp.Modularity;

namespace HIS.Insurance.Tests;

public abstract class InsuranceReportAppServiceTests<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IInsuranceReportAppService _insuranceReportAppService;

    protected InsuranceReportAppServiceTests()
    {
        _insuranceReportAppService = GetRequiredService<IInsuranceReportAppService>();
    }

    [Fact]
    public async Task GetSummaryReportAsync_Should_Return_Data()
    {
        // Arrange
        var input = new GetInsuranceReportInput
        {
            FromDate = DateTime.Now.AddMonths(-1),
            ToDate = DateTime.Now
        };

        // Act
        var result = await _insuranceReportAppService.GetSummaryReportAsync(input);

        // Assert
        result.ShouldNotBeNull();
    }
}
