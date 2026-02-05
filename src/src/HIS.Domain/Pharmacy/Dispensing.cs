using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Pharmacy;

public class Dispensing : FullAuditedAggregateRoot<Guid>
{
    public Guid MedicalOrderId { get; set; }
    public Guid PatientId { get; set; }
    public List<DispensedItem> Items { get; set; }

    protected Dispensing()
    {
        Items = new List<DispensedItem>();
    }

    public Dispensing(Guid id, Guid medicalOrderId, Guid patientId)
        : base(id)
    {
        MedicalOrderId = medicalOrderId;
        PatientId = patientId;
        Items = new List<DispensedItem>();
    }

    public void AddItem(Guid inventoryItemId, Guid inventoryBatchId, decimal quantity, string batchNumber, decimal unitCost)
    {
        Items.Add(new DispensedItem(Id, inventoryItemId, inventoryBatchId, quantity, batchNumber, unitCost));
    }
}

public class DispensedItem
{
    public Guid DispensingId { get; set; }
    public Guid InventoryItemId { get; set; }
    public Guid InventoryBatchId { get; set; }
    public decimal Quantity { get; set; }
    public string BatchNumber { get; set; } // Snapshot
    public decimal UnitCost { get; set; }   // Snapshot for COGS

    public DispensedItem(Guid dispensingId, Guid inventoryItemId, Guid inventoryBatchId, decimal quantity, string batchNumber, decimal unitCost)
    {
        DispensingId = dispensingId;
        InventoryItemId = inventoryItemId;
        InventoryBatchId = inventoryBatchId;
        Quantity = quantity;
        BatchNumber = batchNumber;
        UnitCost = unitCost;
    }
}
