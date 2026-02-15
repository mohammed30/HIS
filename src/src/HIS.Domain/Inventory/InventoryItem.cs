using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Inventory;

public class InventoryItem : FullAuditedAggregateRoot<Guid>
{
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; } // Links to Pharmacy Drug or General Product
    public string ProductName { get; set; } // Denormalized name for display
    public InventoryItemType Type { get; set; }
    public decimal Quantity { get; set; }
    // For LIFO/FIFO, we might need a separate Transaction table to trace batches, 
    // but this aggregated entity keeps current stock.
    public decimal AverageCost { get; set; } 
    
    public decimal MinStockLevel { get; set; }
    public decimal ReorderLevel { get; set; }

    protected InventoryItem() { }

    public InventoryItem(Guid id, Guid warehouseId, Guid productId, string productName, InventoryItemType type, decimal quantity, decimal averageCost)
        : base(id)
    {
        WarehouseId = warehouseId;
        ProductId = productId;
        ProductName = productName;
        Type = type;
        Quantity = quantity;
        AverageCost = averageCost;
    }
}
