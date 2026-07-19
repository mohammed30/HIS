using System;
using System.Linq;
using System.Threading.Tasks;
using HIS.Inventory;
using HIS.Accounting;
using Shouldly;
using Xunit;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Volo.Abp;

namespace HIS.Inventory.Tests;

public abstract class InventoryTransferTests<TStartupModule> : HISTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly InventoryManager _inventoryManager;
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<InventoryBatch, Guid> _batchRepository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<InventoryTransaction, Guid> _transactionRepository;
    
    

    protected InventoryTransferTests()
    {
        _inventoryManager = GetRequiredService<InventoryManager>();
        _inventoryItemRepository = GetRequiredService<IRepository<InventoryItem, Guid>>();
        _batchRepository = GetRequiredService<IRepository<InventoryBatch, Guid>>();
        _warehouseRepository = GetRequiredService<IRepository<Warehouse, Guid>>();
        _transactionRepository = GetRequiredService<IRepository<InventoryTransaction, Guid>>();
        
        
    }

    [Fact]
    public async Task TransferStock_ShouldDeductFromSource_AndAddToDest()
    {
        Guid sourceWhId = Guid.NewGuid();
        Guid destWhId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();
        Guid sourceItemId = Guid.Empty;

        await WithUnitOfWorkAsync(async () =>
        {
            var sourceWh = new Warehouse(sourceWhId, "Main WH", "Loc1", "WH-01");
            var destWh = new Warehouse(destWhId, "Sub WH", "Loc2", "WH-02");
            await _warehouseRepository.InsertAsync(sourceWh);
            await _warehouseRepository.InsertAsync(destWh);

            var sourceItem = new InventoryItem(Guid.NewGuid(), sourceWhId, productId, "Paracetamol 500mg", InventoryItemType.Medication, 100m, 5m);
            await _inventoryItemRepository.InsertAsync(sourceItem);
            sourceItemId = sourceItem.Id;

            var batch = new InventoryBatch(Guid.NewGuid(), sourceItemId, "B001", 100m, 5m, DateTime.Now, "PO-001");
            await _batchRepository.InsertAsync(batch);

            // Seed Mock Accounts to bypass accounting validation
            var accountRepo = GetRequiredService<IRepository<Account, Guid>>();
            var mappingRepo = GetRequiredService<IRepository<AccountMapping, Guid>>();
            
            // Re-use existing accounts created by seeder or create if not exist
            var invAcc = await accountRepo.FirstOrDefaultAsync(a => a.Code == "1130");
            if (invAcc == null)
            {
                invAcc = new Account(Guid.NewGuid(), "1130", "Inventory", "المخزون", AccountType.Asset, null);
                await accountRepo.InsertAsync(invAcc, autoSave: true);
            }
            if (await mappingRepo.FirstOrDefaultAsync(m => m.MappingType == AccountMappingType.Inventory) == null)
                await mappingRepo.InsertAsync(new AccountMapping(Guid.NewGuid(), AccountMappingType.Inventory, invAcc.Id, true), autoSave: true);

            var cogsAcc = await accountRepo.FirstOrDefaultAsync(a => a.Code == "5200");
            if (cogsAcc == null)
            {
                cogsAcc = new Account(Guid.NewGuid(), "5200", "COGS", "تكلفة المبيعات", AccountType.Expense, null);
                await accountRepo.InsertAsync(cogsAcc, autoSave: true);
            }
            if (await mappingRepo.FirstOrDefaultAsync(m => m.MappingType == AccountMappingType.COGS) == null)
                await mappingRepo.InsertAsync(new AccountMapping(Guid.NewGuid(), AccountMappingType.COGS, cogsAcc.Id, true), autoSave: true);
            
            // Other mandatory mappings required by AccountingManager
            var otherMappings = new[] { 
                AccountMappingType.SalesRevenue, AccountMappingType.CashAccount, 
                AccountMappingType.VATOutput, AccountMappingType.VATInput, 
                AccountMappingType.InsuranceReceivable, AccountMappingType.CardPaymentBank, 
                AccountMappingType.PatientsReceivable 
            };
            foreach (var mapping in otherMappings)
            {
                if (await mappingRepo.FirstOrDefaultAsync(m => m.MappingType == mapping) == null)
                {
                    await mappingRepo.InsertAsync(new AccountMapping(Guid.NewGuid(), mapping, invAcc.Id, true), autoSave: true);
                }
            }
        });

        await WithUnitOfWorkAsync(async () =>
        {
            await _inventoryManager.TransferStockAsync(sourceWhId, destWhId, productId, 30m, "TRF-001");
        });

        await WithUnitOfWorkAsync(async () =>
        {
            var sourceItem = await _inventoryItemRepository.GetAsync(sourceItemId);
            sourceItem.Quantity.ShouldBe(70m);

            var destItem = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == destWhId && x.ProductId == productId);
            destItem.ShouldNotBeNull();
            destItem.Quantity.ShouldBe(30m);
            destItem.AverageCost.ShouldBe(5m);

            var sourceBatches = await _batchRepository.GetListAsync(x => x.InventoryItemId == sourceItemId);
            sourceBatches.First().Quantity.ShouldBe(70m);

            var destBatches = await _batchRepository.GetListAsync(x => x.InventoryItemId == destItem.Id);
            destBatches.Count.ShouldBe(1);
            destBatches.First().Quantity.ShouldBe(30m);
            destBatches.First().BatchNumber.ShouldBe("B001");
        });
    }

    [Fact]
    public async Task TransferStock_InsufficientStock_ShouldThrowException_AndRollback()
    {
        Guid sourceWhId = Guid.NewGuid();
        Guid destWhId = Guid.NewGuid();
        Guid productId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await _warehouseRepository.InsertAsync(new Warehouse(sourceWhId, "WH A", "", "A"));
            await _warehouseRepository.InsertAsync(new Warehouse(destWhId, "WH B", "", "B"));

            var sourceItem = new InventoryItem(Guid.NewGuid(), sourceWhId, productId, "Drug A", InventoryItemType.Medication, 10m, 2m);
            await _inventoryItemRepository.InsertAsync(sourceItem);
        });

        // Act & Assert Rollback automatically handled by UOW failure
        var exception = await Should.ThrowAsync<BusinessException>(async () =>
        {
            await WithUnitOfWorkAsync(async () =>
            {
                await _inventoryManager.TransferStockAsync(sourceWhId, destWhId, productId, 50m, "TRF-002");
            });
        });

        exception.Code.ShouldBe("Inventory:InsufficientStock");

        // Verify Rollback
        await WithUnitOfWorkAsync(async () =>
        {
            var item = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == sourceWhId && x.ProductId == productId);
            item.Quantity.ShouldBe(10m); // Quantity should NOT be deducted

            var destItem = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == destWhId && x.ProductId == productId);
            destItem.ShouldBeNull(); // Destination item should NOT be created
        });
    }
}

