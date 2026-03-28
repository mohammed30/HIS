using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Billing;
using HIS.Patients;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Insurance;

public class InsuranceReportAppService : ApplicationService, IInsuranceReportAppService
{
    private readonly IRepository<Invoice, Guid> _invoiceRepository;
    private readonly IRepository<InsuranceCompany, Guid> _companyRepository;
    private readonly IRepository<InsurancePlan, Guid> _planRepository;
    private readonly IRepository<PatientInsurance, Guid> _patientInsuranceRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;

    public InsuranceReportAppService(
        IRepository<Invoice, Guid> invoiceRepository,
        IRepository<InsuranceCompany, Guid> companyRepository,
        IRepository<InsurancePlan, Guid> planRepository,
        IRepository<PatientInsurance, Guid> patientInsuranceRepository,
        IRepository<Patient, Guid> patientRepository)
    {
        _invoiceRepository = invoiceRepository;
        _companyRepository = companyRepository;
        _planRepository = planRepository;
        _patientInsuranceRepository = patientInsuranceRepository;
        _patientRepository = patientRepository;
    }

    public async Task<List<InsuranceSummaryDto>> GetSummaryReportAsync(GetInsuranceReportInput input)
    {
        var invoiceQuery = await _invoiceRepository.GetQueryableAsync();
        var patientInsuranceQuery = await _patientInsuranceRepository.GetQueryableAsync();
        var planQuery = await _planRepository.GetQueryableAsync();
        var companyQuery = await _companyRepository.GetQueryableAsync();

        var query = from invoice in invoiceQuery
                    join pi in patientInsuranceQuery on invoice.PatientInsuranceId equals pi.Id
                    join plan in planQuery on pi.InsurancePlanId equals plan.Id
                    join company in companyQuery on plan.InsuranceCompanyId equals company.Id
                    where invoice.PatientInsuranceId != null
                    select new { invoice, company };

        if (input.FromDate.HasValue)
            query = query.Where(x => x.invoice.InvoiceDate >= input.FromDate.Value);
        
        if (input.ToDate.HasValue)
            query = query.Where(x => x.invoice.InvoiceDate <= input.ToDate.Value);

        if (input.InsuranceCompanyId.HasValue)
            query = query.Where(x => x.company.Id == input.InsuranceCompanyId.Value);

        var groupedResults = query.GroupBy(x => new { x.company.Id, x.company.NameAr })
            .Select(g => new InsuranceSummaryDto
            {
                InsuranceCompanyId = g.Key.Id,
                InsuranceCompanyName = g.Key.NameAr,
                InvoiceCount = g.Count(),
                TotalBilled = g.Sum(x => x.invoice.NetAmount),
                TotalInsuranceShare = g.Sum(x => x.invoice.InsuranceCoverage),
                TotalPatientShare = g.Sum(x => x.invoice.CoPaymentAmount)
            });

        return groupedResults.ToList();
    }

    public async Task<PagedResultDto<InsuranceDetailedClaimDto>> GetDetailedClaimsReportAsync(GetInsuranceReportInput input)
    {
        var invoiceQuery = await _invoiceRepository.GetQueryableAsync();
        var patientInsuranceQuery = await _patientInsuranceRepository.GetQueryableAsync();
        var planQuery = await _planRepository.GetQueryableAsync();
        var companyQuery = await _companyRepository.GetQueryableAsync();
        var patientQuery = await _patientRepository.GetQueryableAsync();

        var query = from invoice in invoiceQuery
                    join pi in patientInsuranceQuery on invoice.PatientInsuranceId equals pi.Id
                    join plan in planQuery on pi.InsurancePlanId equals plan.Id
                    join company in companyQuery on plan.InsuranceCompanyId equals company.Id
                    join patient in patientQuery on invoice.PatientId equals patient.Id
                    where invoice.PatientInsuranceId != null
                    select new { invoice, company, plan, patient };

        if (input.FromDate.HasValue)
            query = query.Where(x => x.invoice.InvoiceDate >= input.FromDate.Value);
        
        if (input.ToDate.HasValue)
            query = query.Where(x => x.invoice.InvoiceDate <= input.ToDate.Value);

        if (input.InsuranceCompanyId.HasValue)
            query = query.Where(x => x.company.Id == input.InsuranceCompanyId.Value);

        if (input.InsurancePlanId.HasValue)
            query = query.Where(x => x.plan.Id == input.InsurancePlanId.Value);

        var totalCount = query.Count();
        
        var items = query.OrderByDescending(x => x.invoice.InvoiceDate)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Select(x => new InsuranceDetailedClaimDto
            {
                InvoiceId = x.invoice.Id,
                InvoiceNumber = x.invoice.InvoiceNumber,
                InvoiceDate = x.invoice.InvoiceDate,
                PatientId = x.patient.Id,
                PatientName = x.patient.FirstNameAr + " " + x.patient.LastNameAr,
                InsurancePlanName = x.plan.NameAr,
                TotalAmount = x.invoice.NetAmount,
                InsuranceShare = x.invoice.InsuranceCoverage,
                PatientShare = x.invoice.CoPaymentAmount,
                Status = x.invoice.Status.ToString()
            }).ToList();

        return new PagedResultDto<InsuranceDetailedClaimDto>(totalCount, items);
    }

    public async Task<Volo.Abp.Content.IRemoteStreamContent> ExportSummaryPdfAsync(GetInsuranceReportInput input)
    {
        // Placeholder for QuestPDF implementation if needed, 
        // for now we can rely on frontend table or implement later.
        throw new NotImplementedException("PDF Export will be implemented based on requirements.");
    }
}
