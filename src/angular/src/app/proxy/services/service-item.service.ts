import { RestService } from '@abp/ng.core';
import { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';

export enum ServiceCategory {
    Consultation = 1,
    Laboratory = 2,
    Radiology = 3,
    Procedure = 4,
    Surgery = 5,
    Other = 6,
}

export interface ServiceItemDto {
    id: string;
    code: string;
    name: string;
    category: ServiceCategory;
    departmentId?: string;
    isActive: boolean;
}

export interface CreateUpdateServiceItemDto {
    code: string;
    name: string;
    category: ServiceCategory;
    departmentId?: string;
    isActive: boolean;
}

export interface RadiologyItemDto extends ServiceItemDto {
    modality: string;
    bodyPart: string;
    instructions: string;
}

export interface CreateUpdateRadiologyItemDto extends CreateUpdateServiceItemDto {
    modality: string;
    bodyPart: string;
    instructions: string;
}

@Injectable({
    providedIn: 'root',
})
export class ServiceItemService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    getList = (input: PagedAndSortedResultRequestDto) =>
        this.restService.request<any, PagedResultDto<ServiceItemDto>>({
            method: 'GET',
            url: '/api/app/service-item',
            params: input,
        },
            { apiName: this.apiName });

    create = (input: CreateUpdateServiceItemDto) =>
        this.restService.request<any, ServiceItemDto>({
            method: 'POST',
            url: '/api/app/service-item',
            body: input,
        },
            { apiName: this.apiName });

    update = (id: string, input: CreateUpdateServiceItemDto) =>
        this.restService.request<any, ServiceItemDto>({
            method: 'PUT',
            url: `/api/app/service-item/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    delete = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/service-item/${id}`,
        },
            { apiName: this.apiName });

    // Radiology
    getRadiologyList = (input: PagedAndSortedResultRequestDto) =>
        this.restService.request<any, PagedResultDto<RadiologyItemDto>>({
            method: 'GET',
            url: '/api/app/service-item/radiology', // Assuming route constraint or different controller method name
            params: input,
        },
            { apiName: this.apiName });

    createRadiology = (input: CreateUpdateRadiologyItemDto) =>
        this.restService.request<any, RadiologyItemDto>({
            method: 'POST',
            url: '/api/app/service-item/radiology',
            body: input,
        },
            { apiName: this.apiName });

    updateRadiology = (id: string, input: CreateUpdateRadiologyItemDto) =>
        this.restService.request<any, RadiologyItemDto>({
            method: 'PUT',
            url: `/api/app/service-item/${id}/radiology`,
            body: input,
        },
            { apiName: this.apiName });
}
