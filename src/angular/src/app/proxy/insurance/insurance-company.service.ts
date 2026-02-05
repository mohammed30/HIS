import type { CreateUpdateInsuranceCompanyDto, GetInsuranceCompaniesInput, InsuranceCompanyDto, LookupDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InsuranceCompanyService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateInsuranceCompanyDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InsuranceCompanyDto>({
      method: 'POST',
      url: '/api/app/insurance-company',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/insurance-company/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InsuranceCompanyDto>({
      method: 'GET',
      url: `/api/app/insurance-company/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetInsuranceCompaniesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InsuranceCompanyDto>>({
      method: 'GET',
      url: '/api/app/insurance-company',
      params: { searchText: input.searchText, isActive: input.isActive, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto[]>({
      method: 'GET',
      url: '/api/app/insurance-company/lookup',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateInsuranceCompanyDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InsuranceCompanyDto>({
      method: 'PUT',
      url: `/api/app/insurance-company/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}