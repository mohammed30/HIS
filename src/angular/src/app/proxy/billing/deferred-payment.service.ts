import type { CreateDeferredPaymentDto, DeferredPaymentDto, GetDeferredPaymentsInput } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DeferredPaymentService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateDeferredPaymentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DeferredPaymentDto>({
      method: 'POST',
      url: '/api/app/deferred-payment',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/deferred-payment/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DeferredPaymentDto>({
      method: 'GET',
      url: `/api/app/deferred-payment/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetDeferredPaymentsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DeferredPaymentDto>>({
      method: 'GET',
      url: '/api/app/deferred-payment',
      params: { searchText: input.searchText, patientId: input.patientId, status: input.status, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getOverdue = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DeferredPaymentDto[]>({
      method: 'GET',
      url: '/api/app/deferred-payment/overdue',
    },
    { apiName: this.apiName,...config });
  

  recordPayment = (id: string, amount: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DeferredPaymentDto>({
      method: 'POST',
      url: `/api/app/deferred-payment/${id}/record-payment`,
      params: { amount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateDeferredPaymentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DeferredPaymentDto>({
      method: 'PUT',
      url: `/api/app/deferred-payment/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}