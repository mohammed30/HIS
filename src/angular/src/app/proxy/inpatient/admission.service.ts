import type { AdmissionStatus } from './admission-status.enum';
import type { CreateUpdateAdmissionDto, DischargeAdmissionDto, GetAdmissionsInput, AdmissionDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class AdmissionService {
    private restService = inject(RestService);
    apiName = 'Default';

    create = (input: CreateUpdateAdmissionDto, config?: Partial<Rest.Config>) =>
        this.restService.request<any, AdmissionDto>({
            method: 'POST',
            url: '/api/app/admission',
            body: input,
        },
            { apiName: this.apiName, ...config });

    delete = (id: string, config?: Partial<Rest.Config>) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/admission/${id}`,
        },
            { apiName: this.apiName, ...config });

    get = (id: string, config?: Partial<Rest.Config>) =>
        this.restService.request<any, AdmissionDto>({
            method: 'GET',
            url: `/api/app/admission/${id}`,
        },
            { apiName: this.apiName, ...config });

    getList = (input: GetAdmissionsInput, config?: Partial<Rest.Config>) =>
        this.restService.request<any, PagedResultDto<AdmissionDto>>({
            method: 'GET',
            url: '/api/app/admission',
            params: { searchText: input.searchText, patientId: input.patientId, status: input.status, roomId: input.roomId, fromDate: input.fromDate, toDate: input.toDate, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
        },
            { apiName: this.apiName, ...config });

    update = (id: string, input: CreateUpdateAdmissionDto, config?: Partial<Rest.Config>) =>
        this.restService.request<any, AdmissionDto>({
            method: 'PUT',
            url: `/api/app/admission/${id}`,
            body: input,
        },
            { apiName: this.apiName, ...config });

    discharge = (id: string, input: DischargeAdmissionDto, config?: Partial<Rest.Config>) =>
        this.restService.request<any, AdmissionDto>({
            method: 'PUT',
            url: `/api/app/admission/${id}/discharge`,
            body: input,
        },
            { apiName: this.apiName, ...config });

    updateDays = (id: string, numberOfDays: number, config?: Partial<Rest.Config>) =>
        this.restService.request<any, AdmissionDto>({
            method: 'PUT',
            url: `/api/app/admission/${id}/days`,
            params: { numberOfDays },
        },
            { apiName: this.apiName, ...config });
}
