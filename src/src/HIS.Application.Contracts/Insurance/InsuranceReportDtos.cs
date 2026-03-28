using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Insurance;

public class InsuranceSummaryDto
{
    public Guid InsuranceCompanyId { get; set; }
    public string InsuranceCompanyName { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal TotalBilled { get; set; }
    public decimal TotalInsuranceShare { get; set; }
    public decimal TotalPatientShare { get; set; }
}

public class InsuranceDetailedClaimDto
{
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string InsurancePlanName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal InsuranceShare { get; set; }
    public decimal PatientShare { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class GetInsuranceReportInput : PagedAndSortedResultRequestDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? InsuranceCompanyId { get; set; }
    public Guid? InsurancePlanId { get; set; }
}

public interface IInsuranceReportAppService : IApplicationService
{
    Task<List<InsuranceSummaryDto>> GetSummaryReportAsync(GetInsuranceReportInput input);
    Task<PagedResultDto<InsuranceDetailedClaimDto>> GetDetailedClaimsReportAsync(GetInsuranceReportInput input);
    Task<Volo.Abp.Content.IRemoteStreamContent> ExportSummaryPdfAsync(GetInsuranceReportInput input);
}
