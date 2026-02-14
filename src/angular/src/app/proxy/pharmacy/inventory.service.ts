import { RestService, PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateStockTransferDto, StockTransferDto, InventoryItemDto } from './models';

@Injectable({
    providedIn: 'root',
})
export class InventoryService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    getTransfers(input: PagedAndSortedResultRequestDto): Observable<PagedResultDto<StockTransferDto>> {
        return this.restService.request({
            url: `/api/app/inventory/transfers`,
            method: 'GET',
            params: input,
        }, { apiName: this.apiName });
    }

    createTransfer(input: CreateStockTransferDto): Observable<StockTransferDto> {
        return this.restService.request({
            url: `/api/app/inventory/transfer`,
            method: 'POST',
            body: input,
        }, { apiName: this.apiName });
    }

    processTransfer(id: string): Observable<void> {
        return this.restService.request({
            url: `/api/app/inventory/${id}/process`,
            method: 'POST',
        }, { apiName: this.apiName });
    }

    getLowStockReport(input: PagedAndSortedResultRequestDto): Observable<PagedResultDto<InventoryItemDto>> {
        return this.restService.request({
            url: `/api/app/inventory/low-stock-report`,
            method: 'GET',
            params: input,
        }, { apiName: this.apiName });
    }
}
