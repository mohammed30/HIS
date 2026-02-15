using System;

namespace HIS.Inventory.Dtos;

public class PriceComparisonDto
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderNumber { get; set; }
}
