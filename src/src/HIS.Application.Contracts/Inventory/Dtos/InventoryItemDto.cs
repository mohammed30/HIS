using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Inventory.Dtos;

public class InventoryItemDto : EntityDto<Guid>
{
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } // Will need to resolve Product/Service Item name
    public string ProductCode { get; set; } // Added for search and display
    public InventoryItemType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal AverageCost { get; set; }
    public decimal MinStockLevel { get; set; }
    public decimal ReorderLevel { get; set; }
    public string Barcode { get; set; }
}
