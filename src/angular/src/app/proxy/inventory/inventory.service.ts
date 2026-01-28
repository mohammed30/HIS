import { Injectable } from '@angular/core';
import { RestService, PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import {
    WarehouseDto,
    CreateUpdateWarehouseDto,
    InventoryItemDto,
    ReceiveStockDto,
    IssueStockDto
} from './models';

@Injectable({
    providedIn: 'root',
})
export class InventoryService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    // Warehouse
    getWarehouseList(input: PagedAndSortedResultRequestDto) {
        return this.restService.request<any, PagedResultDto<WarehouseDto>>({
            method: 'GET',
            url: '/api/app/inventory/warehouse',
            params: input,
        },
            { apiName: this.apiName });
    }

    createWarehouse(input: CreateUpdateWarehouseDto) {
        return this.restService.request<any, WarehouseDto>({
            method: 'POST',
            url: '/api/app/inventory/warehouse',
            body: input,
        },
            { apiName: this.apiName });
    }

    updateWarehouse = (id: string, input: CreateUpdateWarehouseDto) =>
        this.restService.request<any, WarehouseDto>({
            method: 'PUT',
            url: `/api/app/inventory/warehouse/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    deleteWarehouse = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/inventory/warehouse/${id}`,
        },
            { apiName: this.apiName });

    // Stock
    getStockLevels(warehouseId: string) {
        return this.restService.request<any, PagedResultDto<InventoryItemDto>>({
            method: 'GET',
            url: `/api/app/inventory/stock-levels`,
            params: { warehouseId }
        },
            { apiName: this.apiName });
    }

    receiveStock(input: ReceiveStockDto) {
        return this.restService.request<any, void>({
            method: 'POST',
            url: '/api/app/inventory/receive-stock',
            body: input,
        },
            { apiName: this.apiName });
    }

    issueStock(input: IssueStockDto) {
        return this.restService.request<any, void>({
            method: 'POST',
            url: '/api/app/inventory/issue-stock',
            body: input,
        },
            { apiName: this.apiName });
    }
}
