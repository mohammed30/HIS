using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace HIS.Inventory;

public class InventoryDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public InventoryDataSeedContributor(
        IRepository<Warehouse, Guid> warehouseRepository,
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _warehouseRepository = warehouseRepository;
        _inventoryItemRepository = inventoryItemRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // 1. Warehouses
        if (await _warehouseRepository.GetCountAsync() <= 0)
        {
            await CreateWarehouseAsync("Main Warehouse", "Building A - Ground Floor");
            await CreateWarehouseAsync("Pharmacy Warehouse", "Building B - 1st Floor");
            await CreateWarehouseAsync("Lab Warehouse", "Building C - Basement");
        }

        // 2. Inventory Items (Stock)
        if (await _inventoryItemRepository.GetCountAsync() <= 0)
        {
            var mainWarehouse = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Main Warehouse");
            var pharmacyWarehouse = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Pharmacy Warehouse");
            var labWarehouse = await _warehouseRepository.FirstOrDefaultAsync(x => x.Name == "Lab Warehouse");

            if (mainWarehouse != null)
            {
                await CreateInventoryItemAsync(mainWarehouse.Id, _guidGenerator.Create(), "Generic Supplies", InventoryItemType.Consumable, 500, 10);
                await CreateInventoryItemAsync(mainWarehouse.Id, _guidGenerator.Create(), "Office Furniture", InventoryItemType.Asset, 200, 50);
            }

            if (pharmacyWarehouse != null)
            {
                await CreateInventoryItemAsync(pharmacyWarehouse.Id, _guidGenerator.Create(), "Paracetamol 500mg", InventoryItemType.Medication, 1000, 5);
                await CreateInventoryItemAsync(pharmacyWarehouse.Id, _guidGenerator.Create(), "Amoxicillin 250mg", InventoryItemType.Medication, 500, 20);
                await CreateInventoryItemAsync(pharmacyWarehouse.Id, _guidGenerator.Create(), "Cough Syrup", InventoryItemType.Medication, 300, 15);
            }

            if (labWarehouse != null)
            {
                await CreateInventoryItemAsync(labWarehouse.Id, _guidGenerator.Create(), "Chemical Reagents", InventoryItemType.Reagent, 100, 150);
                await CreateInventoryItemAsync(labWarehouse.Id, _guidGenerator.Create(), "Covid-19 Test Kits", InventoryItemType.Consumable, 50, 200);
            }
        }
    }

    private async Task CreateWarehouseAsync(string name, string location)
    {
        var code = "WH-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        var warehouse = new Warehouse(_guidGenerator.Create(), name, location, code);
        await _warehouseRepository.InsertAsync(warehouse);
    }

    private async Task CreateInventoryItemAsync(Guid warehouseId, Guid productId, string productName, InventoryItemType type, decimal quantity, decimal avgCost)
    {
        var item = new InventoryItem(_guidGenerator.Create(), warehouseId, productId, productName, type, quantity, avgCost);
        await _inventoryItemRepository.InsertAsync(item);
    }
}
