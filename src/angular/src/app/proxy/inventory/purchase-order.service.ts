import type { CreateUpdatePurchaseOrderDto, PriceComparisonDto, PurchaseOrderDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PurchaseOrderService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  cancelOrder = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseOrderDto>({
      method: 'POST',
      url: `/api/app/purchase-order/${id}/cancel-order`,
    },
    { apiName: this.apiName,...config });
  

  confirmOrder = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseOrderDto>({
      method: 'POST',
      url: `/api/app/purchase-order/${id}/confirm-order`,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdatePurchaseOrderDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseOrderDto>({
      method: 'POST',
      url: '/api/app/purchase-order',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/purchase-order/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseOrderDto>({
      method: 'GET',
      url: `/api/app/purchase-order/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PurchaseOrderDto>>({
      method: 'GET',
      url: '/api/app/purchase-order',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getPriceComparison = (productId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PriceComparisonDto[]>({
      method: 'GET',
      url: `/api/app/purchase-order/price-comparison/${productId}`,
    },
    { apiName: this.apiName,...config });
  

  receiveOrder = (id: string, warehouseId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/purchase-order/${id}/receive-order/${warehouseId}`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdatePurchaseOrderDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseOrderDto>({
      method: 'PUT',
      url: `/api/app/purchase-order/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}