using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Accounting;

namespace HIS.Inventory;

public class InventoryManager : DomainService
{
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<InventoryTransaction, Guid> _transactionRepository;
    private readonly AccountingManager _accountingManager;

    public InventoryManager(
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<InventoryTransaction, Guid> transactionRepository,
        AccountingManager accountingManager)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _transactionRepository = transactionRepository;
        _accountingManager = accountingManager;
    }

    public async Task ReceiveStockAsync(Guid warehouseId, Guid productId, string productName, InventoryItemType type, decimal quantity, decimal unitCost, string reference)
    {
        // 1. Update Inventory Item
        var item = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == warehouseId && x.ProductId == productId);
        if (item == null)
        {
            item = new InventoryItem(GuidGenerator.Create(), warehouseId, productId, productName ?? "Unknown Product", type, 0, 0);
            await _inventoryItemRepository.InsertAsync(item);
        }

        // Weighted Average Cost Calculation (Simplest for now, LIFO requires tracking stacks)
        var totalValue = (item.Quantity * item.AverageCost) + (quantity * unitCost);
        var newQuantity = item.Quantity + quantity;
        item.AverageCost = newQuantity > 0 ? totalValue / newQuantity : 0;
        item.Quantity = newQuantity;

        await _inventoryItemRepository.UpdateAsync(item);

        // 2. Create Transaction Log
        var transaction = new InventoryTransaction(
            GuidGenerator.Create(),
            item.Id,
            TransactionType.Receipt,
            quantity,
            unitCost,
            DateTime.Now,
            reference
        );
        await _transactionRepository.InsertAsync(transaction);

        // 3. Post to Accounting (Future Step: Needs Account Resolution)
        // await _accountingManager.CreateEntryAsync(...) 
        // We need specific Account Ids for Inventory and Payable to do this automatically.
    }

    public async Task IssueStockAsync(Guid warehouseId, Guid productId, decimal quantity, string reference)
    {
        var item = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == warehouseId && x.ProductId == productId);
        if (item == null || item.Quantity < quantity)
        {
            throw new Volo.Abp.BusinessException("Inventory:InsufficientStock");
        }

        item.Quantity -= quantity;
        await _inventoryItemRepository.UpdateAsync(item);

        var transaction = new InventoryTransaction(
            GuidGenerator.Create(),
            item.Id,
            TransactionType.Issue,
            quantity,
            item.AverageCost, // Cost at time of issue (Weighted Avg)
            DateTime.Now,
            reference
        );
        await _transactionRepository.InsertAsync(transaction);
        
        // Integration Point: Accounting (Dr Expense, Cr Inventory)
    }
}
