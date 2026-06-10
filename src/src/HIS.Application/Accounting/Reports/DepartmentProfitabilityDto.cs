using System;

namespace HIS.Accounting.Reports;

public class DepartmentProfitabilityDto
{
    public Guid CostCenterId { get; set; }
    public string CostCenterName { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetProfit => TotalRevenue - TotalExpense;
}
