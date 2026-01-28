using System;
using System.ComponentModel.DataAnnotations;

namespace HIS.Inventory.Dtos;

public class ReceiveStockDto
{
    [Required]
    public Guid WarehouseId { get; set; }
    
    [Required]
    public Guid ProductId { get; set; } // Points to ServiceItem/Medication
    
    public string ProductName { get; set; } // Denormalized name
    public InventoryItemType Type { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal UnitCost { get; set; }
    
    public Guid? SupplierId { get; set; }
    public string ReferenceNumber { get; set; } // Invoice #
}

public class IssueStockDto 
{
    [Required]
    public Guid WarehouseId { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    public Guid? DepartmentId { get; set; } // Who is consuming it
    public string ReferenceNumber { get; set; }
}
