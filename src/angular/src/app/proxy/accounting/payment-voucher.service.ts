import type { CreateUpdatePaymentVoucherDto, PaymentVoucherDto } from './dtos/models';
import type { VoucherFilterDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PaymentVoucherService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  cancel = (id: string, reason: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/payment-voucher/${id}/cancel`,
      params: { reason },
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdatePaymentVoucherDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentVoucherDto>({
      method: 'POST',
      url: '/api/app/payment-voucher',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/payment-voucher/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentVoucherDto>({
      method: 'GET',
      url: `/api/app/payment-voucher/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: VoucherFilterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PaymentVoucherDto>>({
      method: 'GET',
      url: '/api/app/payment-voucher',
      params: { filter: input.filter, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getPaymentPdf = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/payment-voucher/pdf/${id}`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdatePaymentVoucherDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentVoucherDto>({
      method: 'PUT',
      url: `/api/app/payment-voucher/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}