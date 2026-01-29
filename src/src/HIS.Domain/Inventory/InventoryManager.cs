using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Accounting;
using System.Linq;

namespace HIS.Inventory;

public class InventoryManager : DomainService
{
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<InventoryTransaction, Guid> _transactionRepository;
    private readonly IRepository<InventoryBatch, Guid> _batchRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly AccountingManager _accountingManager;

    public InventoryManager(
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<InventoryTransaction, Guid> transactionRepository,
        IRepository<InventoryBatch, Guid> batchRepository,
        IRepository<Account, Guid> accountRepository,
        AccountingManager accountingManager)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _transactionRepository = transactionRepository;
        _batchRepository = batchRepository;
        _accountRepository = accountRepository;
        _accountingManager = accountingManager;
    }

    public async Task ReceiveStockAsync(Guid warehouseId, Guid productId, string productName, InventoryItemType type, decimal quantity, decimal unitCost, string reference)
    {
        // 1. Update Inventory Item (Total Qty Only)
        var item = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == warehouseId && x.ProductId == productId);
        if (item == null)
        {
            item = new InventoryItem(GuidGenerator.Create(), warehouseId, productId, productName ?? "Unknown Product", type, 0, 0);
            await _inventoryItemRepository.InsertAsync(item);
        }

        // Weighted Average Cost Calculation (Still useful for general reporting)
        var totalValue = (item.Quantity * item.AverageCost) + (quantity * unitCost);
        var newQuantity = item.Quantity + quantity;
        item.AverageCost = newQuantity > 0 ? totalValue / newQuantity : 0;
        item.Quantity = newQuantity;

        await _inventoryItemRepository.UpdateAsync(item);

        // 2. Create Inventory Batch (LIFO Support)
        var batch = new InventoryBatch(
            GuidGenerator.Create(),
            item.Id,
            Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(), // Auto-gen batch ID for now
            quantity,
            unitCost,
            DateTime.Now,
            reference
        );
        await _batchRepository.InsertAsync(batch);

        // 3. Create Transaction Log
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

        // 4. Post to Accounting
        // Look up default accounts (In a real app, these would be in Settings or Warehouse/Supplier config)
        var inventoryAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1130"); // Inventory
        var payableAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "2110");   // Accounts Payable

        if (inventoryAccount != null && payableAccount != null)
        {
            var totalAmount = quantity * unitCost;
            var description = $"Stock Receipt: {productName} (Ref: {reference})";
            var entry = await _accountingManager.CreateEntryAsync(DateTime.Now, reference, description);

            // Debit Inventory
            entry.AddLine(GuidGenerator, inventoryAccount.Id, totalAmount, 0);
            // Credit Payable
            entry.AddLine(GuidGenerator, payableAccount.Id, 0, totalAmount);

            await _accountingManager.PostEntryAsync(entry);
        }
    }

    public async Task IssueStockAsync(Guid warehouseId, Guid productId, decimal quantity, string reference)
    {
        var item = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == warehouseId && x.ProductId == productId);
        if (item == null || item.Quantity < quantity)
        {
            throw new Volo.Abp.BusinessException("Inventory:InsufficientStock");
        }

        decimal remainingQtyToIssue = quantity;
        decimal totalCostOfIssue = 0;

        // LIFO Logic: Get batches ordered by ReceivedDate DESC
        var batches = await _batchRepository.GetListAsync(x => x.InventoryItemId == item.Id && x.Quantity > 0);
        batches = batches.OrderByDescending(x => x.ReceivedDate).ToList();

        foreach (var batch in batches)
        {
            if (remainingQtyToIssue <= 0) break;

            decimal qtyTaken = Math.Min(batch.Quantity, remainingQtyToIssue);
            
            batch.Quantity -= qtyTaken;
            remainingQtyToIssue -= qtyTaken;
            totalCostOfIssue += (qtyTaken * batch.UnitCost);

            await _batchRepository.UpdateAsync(batch);
        }

        if (remainingQtyToIssue > 0)
        {
             // Fallback if batches are missing but item quantity exists (data inconsistency)
             // Use Weighted Average Cost for remaining
             totalCostOfIssue += (remainingQtyToIssue * item.AverageCost);
        }

        item.Quantity -= quantity;
        await _inventoryItemRepository.UpdateAsync(item);

        var transaction = new InventoryTransaction(
            GuidGenerator.Create(),
            item.Id,
            TransactionType.Issue,
            quantity,
            quantity > 0 ? totalCostOfIssue / quantity : 0, // Effective Unit Cost for this Issue
            DateTime.Now,
            reference
        );
        await _transactionRepository.InsertAsync(transaction);
        
        // Integration Point: Accounting (Dr Expense, Cr Inventory)
        var inventoryAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1130"); // Inventory
        var expenseAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "5200");   // Supplies Expense

        if (inventoryAccount != null && expenseAccount != null)
        {
            var totalAmount = totalCostOfIssue;
            var description = $"Stock Issue: {item.ProductName} (Ref: {reference})";
            var entry = await _accountingManager.CreateEntryAsync(DateTime.Now, reference, description);

            // Debit Expense
            entry.AddLine(GuidGenerator, expenseAccount.Id, totalAmount, 0);
            // Credit Inventory
            entry.AddLine(GuidGenerator, inventoryAccount.Id, 0, totalAmount);

            await _accountingManager.PostEntryAsync(entry);
        }
    }
}
