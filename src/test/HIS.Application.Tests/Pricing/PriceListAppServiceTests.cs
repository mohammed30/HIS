using System;
using System.Threading.Tasks;
using HIS.Pricing;
using HIS.Pricing;
using Shouldly;
using Xunit;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using HIS.Services;

namespace HIS.Pricing.Tests;

public abstract class PriceListAppServiceTests<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IPriceListAppService _priceListAppService;
    private readonly IRepository<PriceList, Guid> _priceListRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;

    protected PriceListAppServiceTests()
    {
        _priceListAppService = GetRequiredService<IPriceListAppService>();
        _priceListRepository = GetRequiredService<IRepository<PriceList, Guid>>();
        _serviceItemRepository = GetRequiredService<IRepository<ServiceItem, Guid>>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_PriceList()
    {
        // Arrange
        var input = new CreateUpdatePriceListDto
        {
            Code = "PL-001",
            NameAr = "قائمة الأسعار الافتراضية",
            NameEn = "Default Price List",
            IsActive = true
        };

        // Act
        var result = await _priceListAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Code.ShouldBe("PL-001");
        
        var listInDb = await _priceListRepository.GetAsync(result.Id);
        listInDb.ShouldNotBeNull();
    }

    [Fact]
    public async Task SetPriceAsync_Should_Set_Service_Price()
    {
        // Arrange
        Guid priceListId = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _priceListRepository.InsertAsync(new PriceList(priceListId, "قائمة 2", false, DateTime.Now));
            var service = new ServiceItem(serviceItemId, "SRV-002", "أشعة سينية", ServiceCategory.Radiology, null);
            await _serviceItemRepository.InsertAsync(service);
        });

        var input = new CreateUpdateServicePriceDto
        {
            PriceListId = priceListId,
            ServiceItemId = serviceItemId,
            Amount = 250m
        };

        // Act
        var result = await _priceListAppService.SetPriceAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Amount.ShouldBe(250m);
    }
}
