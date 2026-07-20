using System;
using System.Threading.Tasks;
using HIS.Inventory.Dtos;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace HIS.Inventory.Tests;

public abstract class InventoryAppServiceTests<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly IInventoryAppService _inventoryAppService;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;

    protected InventoryAppServiceTests()
    {
        _inventoryAppService = GetRequiredService<IInventoryAppService>();
        _warehouseRepository = GetRequiredService<IRepository<Warehouse, Guid>>();
        _inventoryItemRepository = GetRequiredService<IRepository<InventoryItem, Guid>>();
    }

    [Fact]
    public async Task CreateWarehouseAsync_ShouldCreateWarehouse()
    {
        // Arrange
        var input = new CreateUpdateWarehouseDto
        {
            Name = "New Test Warehouse",
            Location = "First Floor"
        };

        // Act
        var result = await _inventoryAppService.CreateWarehouseAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("New Test Warehouse");

        var entity = await _warehouseRepository.GetAsync(result.Id);
        entity.ShouldNotBeNull();
        entity.Name.ShouldBe("New Test Warehouse");
    }

    [Fact]
    public async Task GetWarehouseListAsync_ShouldReturnWarehouses()
    {
        // Arrange
        await WithUnitOfWorkAsync(async () =>
        {
            await _warehouseRepository.InsertAsync(new Warehouse(Guid.NewGuid(), "List WH 1", "Loc", "LWH-1"));
            await _warehouseRepository.InsertAsync(new Warehouse(Guid.NewGuid(), "List WH 2", "Loc", "LWH-2"));
        });

        // Act
        var result = await _inventoryAppService.GetWarehouseListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto());

        // Assert
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.ShouldContain(x => x.Name == "List WH 1");
        result.Items.ShouldContain(x => x.Name == "List WH 2");
    }
}
