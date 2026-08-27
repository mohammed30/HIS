using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace HIS.Reports
{
    public class GetInsuranceClaimsInput : PagedAndSortedResultRequestDto
    {
        public Guid? InsuranceCompanyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ServiceType { get; set; }
    }

    public class InsuranceClaimReportDto
    {
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientFileNumber { get; set; } = string.Empty;

        public Guid? InsuranceCompanyId { get; set; }
        public string InsuranceCompanyName { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public string SponsorName { get; set; } = string.Empty; // اسم العائل
        
        public List<InsuranceClaimItemDto> Items { get; set; } = new List<InsuranceClaimItemDto>();
        
        public decimal TotalPatientAmount { get; set; }
        public decimal TotalInsuranceAmount { get; set; }
        public decimal TotalInvoiceAmount { get; set; }
    }

    public class InsuranceClaimItemDto
    {
        public Guid InvoiceItemId { get; set; }
        public string DepartmentName { get; set; } = string.Empty; // المعمل/القسم
        public string ServiceCode { get; set; } = string.Empty; // CPT Code
        public string ServiceName { get; set; } = string.Empty;
        public string DiagnosisCode { get; set; } = string.Empty; // ICD-10
        public string ApprovalNumber { get; set; } = string.Empty; // رقم الموافقة
        public string DoctorName { get; set; } = string.Empty;
        
        public decimal TotalPrice { get; set; }
        public decimal PatientCoPay { get; set; } // تحمل المريض
        public decimal InsuranceCoverage { get; set; } // تحمل التأمين
    }
    
    public class InsuranceClaimPrintDataDto
    {
        public List<InsuranceClaimReportDto> Claims { get; set; } = new List<InsuranceClaimReportDto>();
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? InsuranceCompanyName { get; set; }
        public DateTime PrintDate { get; set; }
    }
}
