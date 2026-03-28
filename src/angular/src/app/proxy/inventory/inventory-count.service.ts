import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { InventoryCountDto, CreateInventoryCountDto, UpdateInventoryCountItemDto, GetInventoryCountsInput } from './dtos/models';

@Injectable({
  providedIn: 'root',
})
export class InventoryCountService {
  private restService = inject(RestService);
  apiName = 'Default';

  create = (input: CreateInventoryCountDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InventoryCountDto>({
      method: 'POST',
      url: '/api/app/inventory-count',
      body: input,
    },
    { apiName: this.apiName,...config });

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InventoryCountDto>({
      method: 'GET',
      url: `/api/app/inventory-count/${id}`,
    },
    { apiName: this.apiName,...config });

  getList = (input: GetInventoryCountsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InventoryCountDto>>({
      method: 'GET',
      url: '/api/app/inventory-count',
      params: { 
        warehouseId: input.warehouseId, 
        status: input.status, 
        fromDate: input.fromDate, 
        toDate: input.toDate,
        sorting: input.sorting,
        skipCount: input.skipCount,
        maxResultCount: input.maxResultCount
      },
    },
    { apiName: this.apiName,...config });

  updateItem = (countId: string, input: UpdateInventoryCountItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: `/api/app/inventory-count/${countId}/item`,
      body: input,
    },
    { apiName: this.apiName,...config });

  finalize = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/inventory-count/${id}/finalize`,
    },
    { apiName: this.apiName,...config });

  cancel = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/inventory-count/${id}/cancel`,
    },
    { apiName: this.apiName,...config });
}
