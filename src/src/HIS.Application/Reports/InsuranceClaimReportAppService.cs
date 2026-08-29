using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Billing;
using HIS.Insurance;
using HIS.Patients;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using QuestPDF.Fluent;

namespace HIS.Reports
{
    public class InsuranceClaimReportAppService : ApplicationService, IInsuranceClaimReportAppService
    {
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<PatientInsurance, Guid> _patientInsuranceRepository;
        private readonly IRepository<InsurancePlan, Guid> _insurancePlanRepository;
        private readonly IRepository<InsuranceCompany, Guid> _insuranceCompanyRepository;
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<HIS.Inpatient.Admission, Guid> _admissionRepository;

        public InsuranceClaimReportAppService(
            IRepository<Invoice, Guid> invoiceRepository,
            IRepository<PatientInsurance, Guid> patientInsuranceRepository,
            IRepository<InsurancePlan, Guid> insurancePlanRepository,
            IRepository<InsuranceCompany, Guid> insuranceCompanyRepository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<HIS.Inpatient.Admission, Guid> admissionRepository)
        {
            _invoiceRepository = invoiceRepository;
            _patientInsuranceRepository = patientInsuranceRepository;
            _insurancePlanRepository = insurancePlanRepository;
            _insuranceCompanyRepository = insuranceCompanyRepository;
            _patientRepository = patientRepository;
            _admissionRepository = admissionRepository;
        }

        public async Task<PagedResultDto<InsuranceClaimReportDto>> GetListAsync(GetInsuranceClaimsInput input)
        {
            var query = await _invoiceRepository.WithDetailsAsync(x => x.Items);

            var invoices = query
                .WhereIf(input.StartDate.HasValue, x => x.InvoiceDate >= input.StartDate.Value.Date)
                .WhereIf(input.EndDate.HasValue, x => x.InvoiceDate <= input.EndDate.Value.Date.AddDays(1).AddTicks(-1))
                .Where(x => x.PatientInsuranceId != null) 
                .ToList();

            var patientIds = invoices.Select(x => x.PatientId).Distinct().ToList();
            
            // Chunking the patientIds to avoid SQL parameter limits
            var admissions = new List<HIS.Inpatient.Admission>();
            int chunkSize = 1000;
            for (int i = 0; i < patientIds.Count; i += chunkSize)
            {
                var chunk = patientIds.Skip(i).Take(chunkSize).ToList();
                var chunkAdmissions = await _admissionRepository.GetListAsync(x => chunk.Contains(x.PatientId));
                admissions.AddRange(chunkAdmissions);
            }

            var inpatientInvoiceIds = new HashSet<Guid>();
            foreach (var invoice in invoices)
            {
                bool isAdmitted = admissions.Any(a => 
                    a.PatientId == invoice.PatientId && 
                    invoice.InvoiceDate >= a.AdmissionDate.Date && 
                    (a.DischargeDate == null || invoice.InvoiceDate <= a.DischargeDate.Value.Date.AddDays(1).AddTicks(-1))
                );

                if (isAdmitted)
                {
                    inpatientInvoiceIds.Add(invoice.Id);
                }
            }

            if (input.PatientType == 1) // Inpatient (منوم)
            {
                invoices = invoices.Where(x => inpatientInvoiceIds.Contains(x.Id)).ToList();
            }
            else if (input.PatientType == 2) // Outpatient (خارجي)
            {
                invoices = invoices.Where(x => !inpatientInvoiceIds.Contains(x.Id)).ToList();
            }

            var result = new List<InsuranceClaimReportDto>();

            foreach (var invoice in invoices)
            {
                var patientInsurance = await _patientInsuranceRepository.FindAsync(invoice.PatientInsuranceId.Value);
                HIS.Insurance.InsurancePlan insurancePlan = null;
                string policyNumber = string.Empty;

                if (patientInsurance != null)
                {
                    insurancePlan = await _insurancePlanRepository.FindAsync(patientInsurance.InsurancePlanId);
                    policyNumber = patientInsurance.PolicyNumber;
                }
                else
                {
                    // Fallback: If PatientInsuranceId actually stores an InsurancePlanId (as used in Reception UI)
                    insurancePlan = await _insurancePlanRepository.FindAsync(invoice.PatientInsuranceId.Value);
                    policyNumber = "-";
                }

                if (insurancePlan == null) continue;

                if (input.InsuranceCompanyId.HasValue && insurancePlan.InsuranceCompanyId != input.InsuranceCompanyId.Value)
                    continue;

                var insuranceCompany = await _insuranceCompanyRepository.FindAsync(insurancePlan.InsuranceCompanyId);
                var patient = await _patientRepository.FindAsync(invoice.PatientId);

                var patientFullName = patient != null ? $"{patient.FirstNameAr} {patient.LastNameAr}".Trim() : "";

                var claimDto = new InsuranceClaimReportDto
                {
                    InvoiceId = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceDate = invoice.InvoiceDate,
                    PatientId = invoice.PatientId,
                    PatientName = patientFullName,
                    PatientFileNumber = patient?.MRN ?? "",
                    InsuranceCompanyId = insurancePlan.InsuranceCompanyId,
                    InsuranceCompanyName = insuranceCompany?.NameAr ?? "",
                    PolicyNumber = policyNumber,
                    SponsorName = patientFullName, 
                    Items = new List<InsuranceClaimItemDto>()
                };

                foreach (var item in invoice.Items)
                {
                    if (!item.IsCoveredByInsurance) continue;

                    var insuranceCoverage = item.TotalPrice * (item.InsurancePercentage / 100);
                    var patientCoPay = item.TotalPrice - insuranceCoverage;

                    claimDto.Items.Add(new InsuranceClaimItemDto
                    {
                        InvoiceItemId = item.Id,
                        DepartmentName = item.ServiceType.ToString(),
                        ServiceCode = item.ServiceCode ?? "",
                        ServiceName = item.Description,
                        DiagnosisCode = "", 
                        ApprovalNumber = "", 
                        DoctorName = "",
                        TotalPrice = item.TotalPrice,
                        PatientCoPay = patientCoPay,
                        InsuranceCoverage = insuranceCoverage
                    });
                }

                if (claimDto.Items.Any())
                {
                    claimDto.TotalPatientAmount = claimDto.Items.Sum(x => x.PatientCoPay);
                    claimDto.TotalInsuranceAmount = claimDto.Items.Sum(x => x.InsuranceCoverage);
                    claimDto.TotalInvoiceAmount = claimDto.Items.Sum(x => x.TotalPrice);
                    result.Add(claimDto);
                }
            }
            
            var sortedResult = result.OrderByDescending(x => x.InvoiceDate).ToList();
            var paginatedResult = sortedResult.Skip(input.SkipCount).Take(input.MaxResultCount).ToList();

            return new PagedResultDto<InsuranceClaimReportDto>(sortedResult.Count, paginatedResult);
        }

        public async Task<byte[]> GetPrintDocumentAsync(GetInsuranceClaimsInput input)
        {
            input.MaxResultCount = 10000;
            input.SkipCount = 0;

            var result = await GetListAsync(input);
            string? companyName = null;

            if (input.InsuranceCompanyId.HasValue)
            {
                var company = await _insuranceCompanyRepository.FindAsync(input.InsuranceCompanyId.Value);
                companyName = company?.NameAr;
            }

            var printData = new InsuranceClaimPrintDataDto
            {
                Claims = result.Items.ToList(),
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                InsuranceCompanyName = companyName,
                PrintDate = DateTime.Now,
                PatientType = input.PatientType
            };

            var document = new HIS.Reports.Printing.InsuranceClaimReportDocument
            {
                ReportData = printData
            };

            return document.GeneratePdf();
        }
    }
}
