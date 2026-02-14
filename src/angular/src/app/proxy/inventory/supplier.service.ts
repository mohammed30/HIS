import { RestService } from '@abp/ng.core';
import { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SupplierDto, CreateUpdateSupplierDto } from './dtos';

@Injectable({
    providedIn: 'root',
})
export class SupplierService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    create = (input: CreateUpdateSupplierDto) =>
        this.restService.request<any, SupplierDto>({
            method: 'POST',
            url: '/api/app/supplier',
            body: input,
        },
            { apiName: this.apiName });

    update = (id: string, input: CreateUpdateSupplierDto) =>
        this.restService.request<any, SupplierDto>({
            method: 'PUT',
            url: `/api/app/supplier/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    delete = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/supplier/${id}`,
        },
            { apiName: this.apiName });

    get = (id: string) =>
        this.restService.request<any, SupplierDto>({
            method: 'GET',
            url: `/api/app/supplier/${id}`,
        },
            { apiName: this.apiName });

    getList = (input: PagedAndSortedResultRequestDto) =>
        this.restService.request<any, PagedResultDto<SupplierDto>>({
            method: 'GET',
            url: '/api/app/supplier',
            params: input,
        },
            { apiName: this.apiName });
}
