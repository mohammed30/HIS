import type { CreateStockTransferDto, StockTransferDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { InventoryItemDto } from '../inventory/dtos/models';

@Injectable({
  providedIn: 'root',
})
export class InventoryService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  createTransfer = (input: CreateStockTransferDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, StockTransferDto>({
      method: 'POST',
      url: '/api/app/inventory/transfer',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getLowStockReport = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InventoryItemDto>>({
      method: 'GET',
      url: '/api/app/inventory/low-stock-report',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getTransfers = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<StockTransferDto>>({
      method: 'GET',
      url: '/api/app/inventory/transfers',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  processTransfer = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/inventory/${id}/process-transfer`,
    },
    { apiName: this.apiName,...config });
}