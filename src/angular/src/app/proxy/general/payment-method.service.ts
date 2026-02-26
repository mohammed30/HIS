import type { CreateUpdatePaymentMethodDto, PaymentMethodDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PaymentMethodService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdatePaymentMethodDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentMethodDto>({
      method: 'POST',
      url: '/api/app/payment-method',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/payment-method/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentMethodDto>({
      method: 'GET',
      url: `/api/app/payment-method/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PaymentMethodDto>>({
      method: 'GET',
      url: '/api/app/payment-method',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdatePaymentMethodDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentMethodDto>({
      method: 'PUT',
      url: `/api/app/payment-method/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}