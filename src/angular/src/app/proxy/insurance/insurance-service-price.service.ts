import type { CreateUpdateInsuranceServicePriceDto, GetInsuranceServicePricesInput, InsuranceServicePriceDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InsuranceServicePriceService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateInsuranceServicePriceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InsuranceServicePriceDto>({
      method: 'POST',
      url: '/api/app/insurance-service-price',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/insurance-service-price/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InsuranceServicePriceDto>({
      method: 'GET',
      url: `/api/app/insurance-service-price/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetInsuranceServicePricesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InsuranceServicePriceDto>>({
      method: 'GET',
      url: '/api/app/insurance-service-price',
      params: { insurancePlanId: input.insurancePlanId, serviceItemId: input.serviceItemId, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateInsuranceServicePriceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InsuranceServicePriceDto>({
      method: 'PUT',
      url: `/api/app/insurance-service-price/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}