import type { CreateUpdateWarehouseDto, DepartmentConsumptionReportDto, GetConsumptionReportInput, InventoryItemDto, InventoryTransactionDto, IssueStockDto, ReceiveStockDto, UpdateStockLevelsDto, WarehouseDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InventoryService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  createWarehouse = (input: CreateUpdateWarehouseDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, WarehouseDto>({
      method: 'POST',
      url: '/api/app/inventory/warehouse',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deleteWarehouse = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/inventory/warehouse/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getConsumptionReport = (input: GetConsumptionReportInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DepartmentConsumptionReportDto[]>({
      method: 'GET',
      url: '/api/app/inventory/consumption-report',
      params: { startDate: input.startDate, endDate: input.endDate, departmentId: input.departmentId },
    },
    { apiName: this.apiName,...config });
  

  getItem = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InventoryItemDto>({
      method: 'GET',
      url: `/api/app/inventory/item/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getItemTransactions = (inventoryItemId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InventoryTransactionDto[]>({
      method: 'GET',
      url: `/api/app/inventory/item-transactions/${inventoryItemId}`,
    },
    { apiName: this.apiName,...config });
  

  getStockLevels = (warehouseId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InventoryItemDto>>({
      method: 'GET',
      url: '/api/app/inventory/stock-levels',
      params: { warehouseId },
    },
    { apiName: this.apiName,...config });
  

  getWarehouseList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<WarehouseDto>>({
      method: 'GET',
      url: '/api/app/inventory/warehouse',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  issueStock = (input: IssueStockDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/inventory/issue-stock',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  receiveStock = (input: ReceiveStockDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/inventory/receive-stock',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateStockLevels = (id: string, input: UpdateStockLevelsDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: `/api/app/inventory/stock-levels/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateWarehouse = (id: string, input: CreateUpdateWarehouseDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, WarehouseDto>({
      method: 'PUT',
      url: `/api/app/inventory/warehouse/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}