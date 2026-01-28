using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Inventory.Dtos;

public class InventoryTransactionDto : EntityDto<Guid>
{
    public Guid InventoryItemId { get; set; }
    public TransactionType TransactionType { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalValue => Quantity * UnitCost;
    public DateTime TransactionDate { get; set; }
    public string ReferenceNumber { get; set; }
}
