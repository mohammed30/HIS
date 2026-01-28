using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Inventory;

public class InventoryBatch : FullAuditedEntity<Guid>
{
    public Guid InventoryItemId { get; set; }
    public string BatchNumber { get; set; }
    public decimal Quantity { get; set; } // Remaining Quantity
    public decimal OriginalQuantity { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime ReceivedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string ReferenceNumber { get; set; }

    protected InventoryBatch() { }

    public InventoryBatch(
        Guid id, 
        Guid inventoryItemId, 
        string batchNumber, 
        decimal quantity, 
        decimal unitCost, 
        DateTime receivedDate, 
        string referenceNumber,
        DateTime? expiryDate = null)
        : base(id)
    {
        InventoryItemId = inventoryItemId;
        BatchNumber = batchNumber;
        Quantity = quantity;
        OriginalQuantity = quantity;
        UnitCost = unitCost;
        ReceivedDate = receivedDate;
        ReferenceNumber = referenceNumber;
        ExpiryDate = expiryDate;
    }
}
