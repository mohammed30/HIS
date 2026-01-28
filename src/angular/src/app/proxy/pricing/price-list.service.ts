import { RestService } from '@abp/ng.core';
import { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

export interface PriceListDto {
    id: string;
    name: string;
    isDefault: boolean;
    effectiveFrom: string;
    effectiveTo?: string;
}

export interface CreateUpdatePriceListDto {
    name: string;
    isDefault: boolean;
    effectiveFrom: string;
    effectiveTo?: string;
}

export interface ServicePriceDto {
    id: string;
    priceListId: string;
    serviceItemId: string;
    serviceItemName: string;
    amount: number;
    coPayAmount: number;
}

export interface CreateUpdateServicePriceDto {
    priceListId: string;
    serviceItemId: string;
    amount: number;
    coPayAmount: number;
}

@Injectable({
    providedIn: 'root',
})
export class PriceListService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    getList = (input: PagedAndSortedResultRequestDto) =>
        this.restService.request<any, PagedResultDto<PriceListDto>>({
            method: 'GET',
            url: '/api/app/price-list',
            params: input,
        },
            { apiName: this.apiName });

    create = (input: CreateUpdatePriceListDto) =>
        this.restService.request<any, PriceListDto>({
            method: 'POST',
            url: '/api/app/price-list',
            body: input,
        },
            { apiName: this.apiName });

    update = (id: string, input: CreateUpdatePriceListDto) =>
        this.restService.request<any, PriceListDto>({
            method: 'PUT',
            url: `/api/app/price-list/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    delete = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/price-list/${id}`,
        },
            { apiName: this.apiName });

    // Prices
    getPrices = (priceListId: string, input: PagedAndSortedResultRequestDto) =>
        this.restService.request<any, PagedResultDto<ServicePriceDto>>({
            method: 'GET',
            url: `/api/app/price-list/${priceListId}/prices`,
            params: input,
        },
            { apiName: this.apiName });

    setPrice = (input: CreateUpdateServicePriceDto) =>
        this.restService.request<any, ServicePriceDto>({
            method: 'POST',
            url: '/api/app/price-list/price',
            body: input,
        },
            { apiName: this.apiName });
}
