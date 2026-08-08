using System;
using System.Collections.Generic;
using Volo.Abp.Application.Services;

namespace HIS.Settings;

public class DoctorRevenueReportInput
{
    public Guid? DoctorId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class DoctorRevenueLineDto
{
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorCode { get; set; } = string.Empty;
    public decimal DoctorPercentage { get; set; }
    public decimal HospitalPercentage { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal DoctorAmount { get; set; }
    public decimal HospitalAmount { get; set; }
    public string? AccountCode { get; set; }
}

public class DoctorRevenueReportDto
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public List<DoctorRevenueLineDto> Lines { get; set; } = new();
    public decimal TotalRevenue { get; set; }
    public decimal TotalDoctorAmount { get; set; }
    public decimal TotalHospitalAmount { get; set; }
}

public interface IDoctorRevenueReportAppService : IApplicationService
{
    System.Threading.Tasks.Task<DoctorRevenueReportDto> GetReportAsync(DoctorRevenueReportInput input);
}
