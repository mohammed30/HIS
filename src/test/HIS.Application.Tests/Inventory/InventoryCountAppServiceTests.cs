using System;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace HIS.Inventory.Tests;

public abstract class InventoryCountAppServiceTests<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly IInventoryCountAppService _inventoryCountAppService;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<InventoryCount, Guid> _countRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<AccountMapping, Guid> _accountMappingRepository;

    protected InventoryCountAppServiceTests()
    {
        _inventoryCountAppService = GetRequiredService<IInventoryCountAppService>();
        _warehouseRepository = GetRequiredService<IRepository<Warehouse, Guid>>();
        _inventoryItemRepository = GetRequiredService<IRepository<InventoryItem, Guid>>();
        _countRepository = GetRequiredService<IRepository<InventoryCount, Guid>>();
        _accountRepository = GetRequiredService<IRepository<Account, Guid>>();
        _accountMappingRepository = GetRequiredService<IRepository<AccountMapping, Guid>>();
    }

    private async Task SeedAccountingForTestAsync()
    {
        var invAcc = await _accountRepository.FirstOrDefaultAsync(a => a.Code == "1130");
        if (invAcc == null)
        {
            invAcc = new Account(Guid.NewGuid(), "1130", "Inventory", "المخزون", AccountType.Asset, null);
            await _accountRepository.InsertAsync(invAcc, autoSave: true);
        }

        var invMapping = await _accountMappingRepository.FirstOrDefaultAsync(m => m.MappingType == AccountMappingType.Inventory);
        if (invMapping == null)
            await _accountMappingRepository.InsertAsync(new AccountMapping(Guid.NewGuid(), AccountMappingType.Inventory, invAcc.Id, true), autoSave: true);
        else if (invMapping.AccountId == null)
        {
            invMapping.AccountId = invAcc.Id;
            await _accountMappingRepository.UpdateAsync(invMapping, autoSave: true);
        }

        var cogsAcc = await _accountRepository.FirstOrDefaultAsync(a => a.Code == "5200");
        if (cogsAcc == null)
        {
            cogsAcc = new Account(Guid.NewGuid(), "5200", "COGS", "تكلفة المبيعات", AccountType.Expense, null);
            await _accountRepository.InsertAsync(cogsAcc, autoSave: true);
        }
        var cogsMapping = await _accountMappingRepository.FirstOrDefaultAsync(m => m.MappingType == AccountMappingType.COGS);
        if (cogsMapping == null)
            await _accountMappingRepository.InsertAsync(new AccountMapping(Guid.NewGuid(), AccountMappingType.COGS, cogsAcc.Id, true), autoSave: true);
        else if (cogsMapping.AccountId == null)
        {
            cogsMapping.AccountId = cogsAcc.Id;
            await _accountMappingRepository.UpdateAsync(cogsMapping, autoSave: true);
        }
        
        var otherMappings = new[] { 
            AccountMappingType.SalesRevenue, AccountMappingType.CashAccount, 
            AccountMappingType.VATOutput, AccountMappingType.VATInput, 
            AccountMappingType.InsuranceReceivable, AccountMappingType.CardPaymentBank, 
            AccountMappingType.PatientsReceivable 
        };
        foreach (var mapping in otherMappings)
        {
            var existingMapping = await _accountMappingRepository.FirstOrDefaultAsync(m => m.MappingType == mapping);
            if (existingMapping == null)
            {
                await _accountMappingRepository.InsertAsync(new AccountMapping(Guid.NewGuid(), mapping, invAcc.Id, true), autoSave: true);
            }
            else if (existingMapping.AccountId == null)
            {
                existingMapping.AccountId = invAcc.Id;
                await _accountMappingRepository.UpdateAsync(existingMapping, autoSave: true);
            }
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDraftCount_WithSnapshotOfItems()
    {
        // Arrange
        Guid warehouseId = Guid.NewGuid();
        Guid productId1 = Guid.NewGuid();
        Guid productId2 = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _warehouseRepository.InsertAsync(new Warehouse(warehouseId, "Test WH", "Loc", "WH-TEST"));
            await _inventoryItemRepository.InsertAsync(new InventoryItem(Guid.NewGuid(), warehouseId, productId1, "Item 1", InventoryItemType.Medication, 100m, 5m));
            await _inventoryItemRepository.InsertAsync(new InventoryItem(Guid.NewGuid(), warehouseId, productId2, "Item 2", InventoryItemType.Consumable, 50m, 10m));
        });

        // Act
        InventoryCountDto result = null;
        await WithUnitOfWorkAsync(async () =>
        {
            result = await _inventoryCountAppService.CreateAsync(new CreateInventoryCountDto
            {
                WarehouseId = warehouseId,
                CountDate = DateTime.Now,
                Notes = "Annual Count"
            });
        });

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(InventoryCountStatus.Draft);
        result.Notes.ShouldBe("Annual Count");
        result.Items.Count.ShouldBe(2);
        result.Items.Any(x => x.SystemQuantity == 100m).ShouldBeTrue();
        result.Items.Any(x => x.SystemQuantity == 50m).ShouldBeTrue();
    }

    [Fact]
    public async Task FinalizeAsync_ShouldUpdateInventory_WhenThereIsDifference()
    {
        // Arrange
        Guid warehouseId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _warehouseRepository.InsertAsync(new Warehouse(warehouseId, "Test WH 2", "Loc", "WH-TEST2"));
            await _inventoryItemRepository.InsertAsync(new InventoryItem(Guid.NewGuid(), warehouseId, productId, "Item To Count", InventoryItemType.Medication, 100m, 5m));
            await SeedAccountingForTestAsync();
        });

        InventoryCountDto count = null;
        await WithUnitOfWorkAsync(async () =>
        {
            count = await _inventoryCountAppService.CreateAsync(new CreateInventoryCountDto
            {
                WarehouseId = warehouseId,
                CountDate = DateTime.Now
            });
        });

        var countItem = count.Items.First();

        await WithUnitOfWorkAsync(async () =>
        {
            // Update counted quantity to 120 (Surplus of 20)
            await _inventoryCountAppService.UpdateItemAsync(count.Id, new UpdateInventoryCountItemDto
            {
                Id = countItem.Id,
                CountedQuantity = 120m,
                Notes = "Found extra"
            });
        });

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            await _inventoryCountAppService.FinalizeAsync(count.Id);
        });

        // Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var updatedCount = await _inventoryCountAppService.GetAsync(count.Id);
            updatedCount.Status.ShouldBe(InventoryCountStatus.Completed);

            var item = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == warehouseId && x.ProductId == productId);
            item.Quantity.ShouldBe(120m); // Was 100, now 120
        });
    }

    [Fact]
    public async Task FinalizeAsync_ShouldIssueInventory_WhenThereIsDeficit()
    {
        // Arrange
        Guid warehouseId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _warehouseRepository.InsertAsync(new Warehouse(warehouseId, "Test WH 3", "Loc", "WH-TEST3"));
            await _inventoryItemRepository.InsertAsync(new InventoryItem(Guid.NewGuid(), warehouseId, productId, "Item To Count 2", InventoryItemType.Medication, 100m, 5m));
            await SeedAccountingForTestAsync();
        });

        InventoryCountDto count = null;
        await WithUnitOfWorkAsync(async () =>
        {
            count = await _inventoryCountAppService.CreateAsync(new CreateInventoryCountDto
            {
                WarehouseId = warehouseId,
                CountDate = DateTime.Now
            });
        });

        var countItem = count.Items.First();

        await WithUnitOfWorkAsync(async () =>
        {
            // Update counted quantity to 80 (Deficit of 20)
            await _inventoryCountAppService.UpdateItemAsync(count.Id, new UpdateInventoryCountItemDto
            {
                Id = countItem.Id,
                CountedQuantity = 80m,
                Notes = "Lost items"
            });
        });

        // Act
        await WithUnitOfWorkAsync(async () =>
        {
            await _inventoryCountAppService.FinalizeAsync(count.Id);
        });

        // Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var updatedCount = await _inventoryCountAppService.GetAsync(count.Id);
            updatedCount.Status.ShouldBe(InventoryCountStatus.Completed);

            var item = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == warehouseId && x.ProductId == productId);
            item.Quantity.ShouldBe(80m); // Was 100, now 80
        });
    }
}
