import type { GetInsuranceClaimsInput, InsuranceClaimReportDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InsuranceClaimReportService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getList = (input: GetInsuranceClaimsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InsuranceClaimReportDto>>({
      method: 'GET',
      url: '/api/app/insurance-claim-report',
      params: { insuranceCompanyId: input.insuranceCompanyId, startDate: input.startDate, endDate: input.endDate, serviceType: input.serviceType, patientType: input.patientType, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getPrintDocument = (input: GetInsuranceClaimsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, number[]>({
      method: 'GET',
      url: '/api/app/insurance-claim-report/print-document',
      params: { insuranceCompanyId: input.insuranceCompanyId, startDate: input.startDate, endDate: input.endDate, serviceType: input.serviceType, patientType: input.patientType, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
}