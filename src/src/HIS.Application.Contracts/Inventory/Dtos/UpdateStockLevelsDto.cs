using System;
using System.ComponentModel.DataAnnotations;

namespace HIS.Inventory.Dtos;

public class UpdateStockLevelsDto
{
    [Range(0, double.MaxValue)]
    public decimal MinStockLevel { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal ReorderLevel { get; set; }
}
