import { RestService } from '@abp/ng.core';
import { PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { PurchaseOrderDto, CreateUpdatePurchaseOrderDto } from './dtos/purchase-order-dto';

@Injectable({
    providedIn: 'root',
})
export class PurchaseOrderService {
    apiName = 'Default';

    create = (input: CreateUpdatePurchaseOrderDto) =>
        this.restService.request<any, PurchaseOrderDto>({
            method: 'POST',
            url: '/api/app/purchase-order',
            body: input,
        },
            { apiName: this.apiName });

    update = (id: string, input: CreateUpdatePurchaseOrderDto) =>
        this.restService.request<any, PurchaseOrderDto>({
            method: 'PUT',
            url: `/api/app/purchase-order/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    delete = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/purchase-order/${id}`,
        },
            { apiName: this.apiName });

    get = (id: string) =>
        this.restService.request<any, PurchaseOrderDto>({
            method: 'GET',
            url: `/api/app/purchase-order/${id}`,
        },
            { apiName: this.apiName });

    getList = (input: PagedAndSortedResultRequestDto) =>
        this.restService.request<any, PagedResultDto<PurchaseOrderDto>>({
            method: 'GET',
            url: '/api/app/purchase-order',
            params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
        },
            { apiName: this.apiName });

    confirmOrder = (id: string) =>
        this.restService.request<any, PurchaseOrderDto>({
            method: 'POST',
            url: `/api/app/purchase-order/${id}/confirm`,
        },
            { apiName: this.apiName });

    cancelOrder = (id: string) =>
        this.restService.request<any, PurchaseOrderDto>({
            method: 'POST',
            url: `/api/app/purchase-order/${id}/cancel`,
        },
            { apiName: this.apiName });

    constructor(private restService: RestService) { }
}
