import type { CreateUpdatePurchaseInvoiceDto, PurchaseInvoiceDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PurchaseInvoiceService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdatePurchaseInvoiceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseInvoiceDto>({
      method: 'POST',
      url: '/api/app/purchase-invoice',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/purchase-invoice/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseInvoiceDto>({
      method: 'GET',
      url: `/api/app/purchase-invoice/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PurchaseInvoiceDto>>({
      method: 'GET',
      url: '/api/app/purchase-invoice',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  postInvoice = (id: string, warehouseId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/purchase-invoice/${id}/invoice/${warehouseId}`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdatePurchaseInvoiceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseInvoiceDto>({
      method: 'PUT',
      url: `/api/app/purchase-invoice/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}