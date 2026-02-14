using System;

namespace HIS.Inventory.Dtos;

public class DepartmentConsumptionReportDto
{
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal TotalCost { get; set; }
}

public class GetConsumptionReportInput 
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? DepartmentId { get; set; }
}
