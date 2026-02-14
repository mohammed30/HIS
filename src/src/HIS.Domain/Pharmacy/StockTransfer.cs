using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace HIS.Pharmacy;

public class StockTransfer : FullAuditedAggregateRoot<Guid>
{
    public string TransferNumber { get; set; } // Auto-generated
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public TransferStatus Status { get; set; }
    public DateTime? TransferDate { get; set; }
    public string? Notes { get; set; }
    
    public virtual ICollection<StockTransferItem> Items { get; set; }

    protected StockTransfer() 
    {
        Items = new List<StockTransferItem>();
    }

    public StockTransfer(Guid id, string transferNumber, Guid fromWarehouseId, Guid toWarehouseId) 
        : base(id)
    {
        TransferNumber = transferNumber;
        FromWarehouseId = fromWarehouseId;
        ToWarehouseId = toWarehouseId;
        Status = TransferStatus.Draft;
        Items = new List<StockTransferItem>();
    }
}

public class StockTransferItem : FullAuditedEntity<Guid>
{
    public Guid StockTransferId { get; set; }
    public Guid DrugId { get; set; }
    public int Quantity { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }

    protected StockTransferItem() { }

    public StockTransferItem(Guid id, Guid stockTransferId, Guid drugId, int quantity, string? batchNumber = null, DateTime? expiryDate = null)
        : base(id)
    {
        StockTransferId = stockTransferId;
        DrugId = drugId;
        Quantity = quantity;
        BatchNumber = batchNumber;
        ExpiryDate = expiryDate;
    }
}


