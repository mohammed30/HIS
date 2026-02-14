using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Inventory;

public class InventoryTransaction : FullAuditedAggregateRoot<Guid>
{
    public Guid InventoryItemId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; } 
    public DateTime TransactionDate { get; set; }
    public string ReferenceNumber { get; set; } // PO Number or Order Id
    public Guid? DepartmentId { get; set; }

    protected InventoryTransaction() { }

    public InventoryTransaction(Guid id, Guid inventoryItemId, TransactionType transactionType, decimal quantity, decimal unitCost, DateTime transactionDate, string referenceNumber, Guid? departmentId = null)
        : base(id)
    {
        InventoryItemId = inventoryItemId;
        TransactionType = transactionType;
        Quantity = quantity;
        UnitCost = unitCost;
        TransactionDate = transactionDate;
        ReferenceNumber = referenceNumber;
        DepartmentId = departmentId;
    }
}
