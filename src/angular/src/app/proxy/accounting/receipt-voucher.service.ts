import type { CreateUpdateReceiptVoucherDto, ReceiptVoucherDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ReceiptVoucherService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateReceiptVoucherDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ReceiptVoucherDto>({
      method: 'POST',
      url: '/api/app/receipt-voucher',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/receipt-voucher/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ReceiptVoucherDto>({
      method: 'GET',
      url: `/api/app/receipt-voucher/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ReceiptVoucherDto>>({
      method: 'GET',
      url: '/api/app/receipt-voucher',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getReceiptPdf = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/receipt-voucher/pdf/${id}`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateReceiptVoucherDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ReceiptVoucherDto>({
      method: 'PUT',
      url: `/api/app/receipt-voucher/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}