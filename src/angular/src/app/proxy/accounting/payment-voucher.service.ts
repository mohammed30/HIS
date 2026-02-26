import type { CreateUpdatePaymentVoucherDto, PaymentVoucherDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PaymentVoucherService {
  private restService = inject(RestService);
  apiName = 'Default';
  

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
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PaymentVoucherDto>>({
      method: 'GET',
      url: '/api/app/payment-voucher',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
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