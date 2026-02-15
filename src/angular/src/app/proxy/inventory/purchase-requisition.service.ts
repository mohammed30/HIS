import { RestService } from '@abp/ng.core';
import { PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { PurchaseRequisitionDto, CreateUpdatePurchaseRequisitionDto } from './dtos';

@Injectable({
    providedIn: 'root',
})
export class PurchaseRequisitionService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    create = (input: CreateUpdatePurchaseRequisitionDto) =>
        this.restService.request<any, PurchaseRequisitionDto>({
            method: 'POST',
            url: '/api/app/purchase-requisition',
            body: input,
        },
            { apiName: this.apiName });

    update = (id: string, input: CreateUpdatePurchaseRequisitionDto) =>
        this.restService.request<any, PurchaseRequisitionDto>({
            method: 'PUT',
            url: `/api/app/purchase-requisition/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    get = (id: string) =>
        this.restService.request<any, PurchaseRequisitionDto>({
            method: 'GET',
            url: `/api/app/purchase-requisition/${id}`,
        },
            { apiName: this.apiName });

    getList = (params: any) =>
        this.restService.request<any, PagedResultDto<PurchaseRequisitionDto>>({
            method: 'GET',
            url: '/api/app/purchase-requisition',
            params: params,
        },
            { apiName: this.apiName });

    updateStatus = (id: string, status: number) =>
        this.restService.request<any, void>({
            method: 'POST',
            url: `/api/app/purchase-requisition/${id}/status`,
            params: { status }
        },
            { apiName: this.apiName });
}
