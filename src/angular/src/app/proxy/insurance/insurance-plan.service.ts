import type { CreateUpdateInsurancePlanDto, GetInsurancePlansInput, InsurancePlanDto, LookupDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InsurancePlanService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateInsurancePlanDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InsurancePlanDto>({
      method: 'POST',
      url: '/api/app/insurance-plan',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/insurance-plan/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InsurancePlanDto>({
      method: 'GET',
      url: `/api/app/insurance-plan/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetInsurancePlansInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InsurancePlanDto>>({
      method: 'GET',
      url: '/api/app/insurance-plan',
      params: { searchText: input.searchText, insuranceCompanyId: input.insuranceCompanyId, isActive: input.isActive, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getLookup = (companyId?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto[]>({
      method: 'GET',
      url: '/api/app/insurance-plan/lookup',
      params: { companyId },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateInsurancePlanDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InsurancePlanDto>({
      method: 'PUT',
      url: `/api/app/insurance-plan/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}