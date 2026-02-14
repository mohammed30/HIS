using System;
using System.Collections.Generic;
using HIS.Services;
using Volo.Abp.Domain.Entities;

namespace HIS.Inventory;

public class PurchaseOrderLine : Entity<Guid>
{
    public Guid PurchaseOrderId { get; set; }
    // We link to InventoryItem (Product) - wait, usually PO is for Products that might not be in stock yet. 
    // But let's assume we pick from defined ServiceItems/Products. 
    // In our DB we have InventoryItems which are Stock+Cost. But the definition is likely ServiceItem or a Product Definition.
    // Let's check HISDbContext. InventoryItem seems to be the one.
    // But usually you order a "Product" (ServiceItem in this system seems to cover services/goods? No, InventoryItems has ProductId).
    // Let's check InventoryItem definition again.
    
    public Guid ProductId { get; set; } // Linking to ServiceItem
    public virtual ServiceItem Product { get; set; }
    // Re-checking HISDbContext: builder.Entity<HIS.Inventory.InventoryItem>(b => ... b.HasIndex(x => new { x.WarehouseId, x.ProductId }).IsUnique();
    // So InventoryItem IS the stock record. ProductId is the definition.
    // So PO Line should reference ProductId.
    
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Description { get; set; }

    protected PurchaseOrderLine() { }

    public PurchaseOrderLine(Guid id, Guid purchaseOrderId, Guid productId, decimal quantity, decimal unitPrice) : base(id)
    {
        PurchaseOrderId = purchaseOrderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalAmount = (quantity * unitPrice) - Discount;
    }
}
