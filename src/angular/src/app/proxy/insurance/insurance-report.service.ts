import type { GetInsuranceReportInput, InsuranceDetailedClaimDto, InsuranceSummaryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InsuranceReportService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  exportSummaryPdf = (input: GetInsuranceReportInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'POST',
      responseType: 'blob',
      url: '/api/app/insurance-report/export-summary-pdf',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getDetailedClaimsReport = (input: GetInsuranceReportInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InsuranceDetailedClaimDto>>({
      method: 'GET',
      url: '/api/app/insurance-report/detailed-claims-report',
      params: { fromDate: input.fromDate, toDate: input.toDate, insuranceCompanyId: input.insuranceCompanyId, insurancePlanId: input.insurancePlanId, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getSummaryReport = (input: GetInsuranceReportInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InsuranceSummaryDto[]>({
      method: 'GET',
      url: '/api/app/insurance-report/summary-report',
      params: { fromDate: input.fromDate, toDate: input.toDate, insuranceCompanyId: input.insuranceCompanyId, insurancePlanId: input.insurancePlanId, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
}