import { RestService, PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { PurchaseInvoiceDto, CreateUpdatePurchaseInvoiceDto } from './dtos/models';

@Injectable({
  providedIn: 'root',
})
export class PurchaseInvoiceService {
  apiName = 'Default';

  constructor(private restService: RestService) {}

  create = (input: CreateUpdatePurchaseInvoiceDto) =>
    this.restService.request<any, PurchaseInvoiceDto>({
      method: 'POST',
      url: '/api/app/purchase-invoice',
      body: input,
    }, { apiName: this.apiName });

  update = (id: string, input: CreateUpdatePurchaseInvoiceDto) =>
    this.restService.request<any, PurchaseInvoiceDto>({
      method: 'PUT',
      url: `/api/app/purchase-invoice/${id}`,
      body: input,
    }, { apiName: this.apiName });

  get = (id: string) =>
    this.restService.request<any, PurchaseInvoiceDto>({
      method: 'GET',
      url: `/api/app/purchase-invoice/${id}`,
    }, { apiName: this.apiName });

  getList = (input: PagedAndSortedResultRequestDto) =>
    this.restService.request<any, PagedResultDto<PurchaseInvoiceDto>>({
      method: 'GET',
      url: '/api/app/purchase-invoice',
      params: { 
        maxResultCount: input.maxResultCount, 
        skipCount: input.skipCount, 
        sorting: input.sorting 
      },
    }, { apiName: this.apiName });

  delete = (id: string) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/purchase-invoice/${id}`,
    }, { apiName: this.apiName });

  postInvoice = (id: string, warehouseId: string) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/purchase-invoice/${id}/post-invoice`,
      params: { warehouseId },
    }, { apiName: this.apiName });
}
