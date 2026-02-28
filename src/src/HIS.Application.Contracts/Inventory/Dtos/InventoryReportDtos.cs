using System;

namespace HIS.Inventory.Dtos;

public class LowStockReportDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string WarehouseName { get; set; }
    public decimal CurrentQuantity { get; set; }
    public decimal MinStockLevel { get; set; }
    public decimal Deficit => MinStockLevel - CurrentQuantity;
}

public class GetLowStockReportInput 
{
    public Guid? WarehouseId { get; set; }
}

public class StagnantStockReportDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string WarehouseName { get; set; }
    public decimal CurrentQuantity { get; set; }
    public DateTime? LastTransactionDate { get; set; }
    public int DaysStagnant { get; set; }
}

public class GetStagnantStockReportInput
{
    public Guid? WarehouseId { get; set; }
    public int ThresholdDays { get; set; } = 30; // Items with no transactions for this many days
}
