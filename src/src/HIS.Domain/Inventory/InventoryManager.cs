using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Accounting;
using HIS.Settings;
using System.Linq;
using Volo.Abp.Settings;

namespace HIS.Inventory;

public class InventoryManager : DomainService
{
    private readonly IRepository<InventoryItem, Guid> _inventoryItemRepository;
    private readonly IRepository<InventoryTransaction, Guid> _transactionRepository;
    private readonly IRepository<InventoryBatch, Guid> _batchRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<Department, Guid> _departmentRepository;
    private readonly AccountingManager _accountingManager;
    protected ISettingProvider SettingProvider { get; }

    public InventoryManager(
        IRepository<InventoryItem, Guid> inventoryItemRepository,
        IRepository<InventoryTransaction, Guid> transactionRepository,
        IRepository<InventoryBatch, Guid> batchRepository,
        IRepository<Account, Guid> accountRepository,
        IRepository<Department, Guid> departmentRepository,
        AccountingManager accountingManager,
        ISettingProvider settingProvider)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _transactionRepository = transactionRepository;
        _batchRepository = batchRepository;
        _accountRepository = accountRepository;
        _departmentRepository = departmentRepository;
        _accountingManager = accountingManager;
        SettingProvider = settingProvider;
    }

    public async Task ReceiveStockAsync(Guid warehouseId, Guid productId, string productName, InventoryItemType type, decimal quantity, decimal unitCost, string reference, string batchNumber = null, DateTime? expiryDate = null)
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

        // 2. Create Inventory Batch
        var batch = new InventoryBatch(
            GuidGenerator.Create(),
            item.Id,
            batchNumber ?? Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(), // Use provided or auto-gen
            quantity,
            unitCost,
            DateTime.Now,
            reference,
            expiryDate
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
            var description = $"توريد مخزني: {productName} (مرجع: {reference})";
            var entry = await _accountingManager.CreateEntryAsync(DateTime.Now, reference, description);

            // Debit Inventory
            entry.AddLine(GuidGenerator, inventoryAccount.Id, totalAmount, 0);
            // Credit Payable
            entry.AddLine(GuidGenerator, payableAccount.Id, 0, totalAmount);

            await _accountingManager.PostEntryAsync(entry);
        }
    }

    public async Task IssueStockAsync(Guid warehouseId, Guid productId, decimal quantity, string reference, Guid? departmentId = null)
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
            reference,
            departmentId
        );
        await _transactionRepository.InsertAsync(transaction);
        
        // Integration Point: Accounting (Dr Expense, Cr Inventory)
        // Integration Point: Accounting (Dr Expense, Cr Inventory)
        var inventoryAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1130"); // Inventory
        Account expenseAccount = null;

        if (departmentId.HasValue)
        {
             var department = await _departmentRepository.GetAsync(departmentId.Value);
             if (department.CostCenterId.HasValue)
             {
                 expenseAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Id == department.CostCenterId.Value);
             }
        }

        if (expenseAccount == null)
        {
             expenseAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "5200");   // Supplies Expense (Default)
        }

        if (inventoryAccount != null && expenseAccount != null)
        {
            var totalAmount = totalCostOfIssue;
            var description = $"صرف مخزني: {item.ProductName} (مرجع: {reference})";
            var entry = await _accountingManager.CreateEntryAsync(DateTime.Now, reference, description);

            // Debit Expense
            entry.AddLine(GuidGenerator, expenseAccount.Id, totalAmount, 0);
            // Credit Inventory
            entry.AddLine(GuidGenerator, inventoryAccount.Id, 0, totalAmount);

            await _accountingManager.PostEntryAsync(entry);
        }
    }

    public async Task<System.Collections.Generic.List<(Guid BatchId, decimal Quantity, decimal UnitCost, string BatchNumber)>> DispenseStockAsync(Guid warehouseId, Guid productId, decimal quantity, string reference)
    {
        var item = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == warehouseId && x.ProductId == productId);
        
        bool allowNegativeStock = await SettingProvider.GetAsync<bool>(HISSettings.Pharmacy.AllowNegativeStock);

        if (item == null || item.Quantity < quantity)
        {
            if (!allowNegativeStock)
            {
                throw new Volo.Abp.BusinessException("Inventory:InsufficientStock");
            }
        }

        // If item doesn't exist, create it (happens only if allowNegativeStock is true or we already moved past throw)
        if (item == null)
        {
            item = new InventoryItem(GuidGenerator.Create(), warehouseId, productId, "POS Product", InventoryItemType.Medication, 0, 0);
            await _inventoryItemRepository.InsertAsync(item);
        }

        decimal remainingQtyToIssue = quantity;
        decimal totalCostOfIssue = 0;
        var dispensedDetails = new System.Collections.Generic.List<(Guid BatchId, decimal Quantity, decimal UnitCost, string BatchNumber)>();

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

            dispensedDetails.Add((batch.Id, qtyTaken, batch.UnitCost, batch.BatchNumber));

            await _batchRepository.UpdateAsync(batch);
        }

        if (remainingQtyToIssue > 0)
        {
             // If we have remaining quantity but no more batches (or no batches at all), 
             // we allow it to proceed (Negative Stock)
             totalCostOfIssue += (remainingQtyToIssue * item.AverageCost);
        }

        item.Quantity -= quantity;
        await _inventoryItemRepository.UpdateAsync(item);

        var transaction = new InventoryTransaction(
            GuidGenerator.Create(),
            item.Id,
            TransactionType.Dispensing, 
            quantity,
            quantity > 0 ? totalCostOfIssue / quantity : 0, 
            DateTime.Now,
            reference
        );
        await _transactionRepository.InsertAsync(transaction);
        
        // Accounting: Dr Expense, Cr Inventory
        var inventoryAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1130");
        var expenseAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "5200"); 

        if (inventoryAccount != null && expenseAccount != null)
        {
             var entry = await _accountingManager.CreateEntryAsync(DateTime.Now, reference, $"صرف علاج: {item.ProductName}");
             entry.AddLine(GuidGenerator, expenseAccount.Id, totalCostOfIssue, 0);
             entry.AddLine(GuidGenerator, inventoryAccount.Id, 0, totalCostOfIssue);
             await _accountingManager.PostEntryAsync(entry);
        }

        return dispensedDetails;
    }

    public async Task ReturnStockAsync(Guid warehouseId, Guid productId, decimal quantity, string reference)
    {
        var item = await _inventoryItemRepository.FirstOrDefaultAsync(x => x.WarehouseId == warehouseId && x.ProductId == productId);
        if (item == null)
        {
            throw new Volo.Abp.BusinessException("Inventory:ProductNotFoundInWarehouse");
        }

        // Increase quantity
        item.Quantity += quantity;
        await _inventoryItemRepository.UpdateAsync(item);

        // Record transaction
        var transaction = new InventoryTransaction(
            GuidGenerator.Create(),
            item.Id,
            TransactionType.Receipt, // Or add a Return type
            quantity,
            item.AverageCost,
            DateTime.Now,
            "Return: " + reference
        );
        await _transactionRepository.InsertAsync(transaction);

        // Reverse Accounting: Dr Inventory, Cr Expense
        var inventoryAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1130");
        var expenseAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "5200");

        if (inventoryAccount != null && expenseAccount != null)
        {
            var totalAmount = quantity * item.AverageCost;
            var entry = await _accountingManager.CreateEntryAsync(DateTime.Now, reference, $"مرتجع علاج: {item.ProductName}");
            
            // Debit Inventory
            entry.AddLine(GuidGenerator, inventoryAccount.Id, totalAmount, 0);
            // Credit Expense
            entry.AddLine(GuidGenerator, expenseAccount.Id, 0, totalAmount);

            await _accountingManager.PostEntryAsync(entry);
        }
    }
}
