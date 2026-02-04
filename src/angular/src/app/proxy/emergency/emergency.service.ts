import type { EmergencyVisitDto, CreateEmergencyVisitDto, TriageDto, UpdateStatusDto } from './dtos/models';
import { RestService, Rest, PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class EmergencyService {
    private restService = inject(RestService);
    apiName = 'Default';

    getActiveVisits = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
        this.restService.request<any, PagedResultDto<EmergencyVisitDto>>({
            method: 'GET',
            url: '/api/app/emergency/active-visits',
            params: input,
        },
            { apiName: this.apiName, ...config });

    register = (input: CreateEmergencyVisitDto, config?: Partial<Rest.Config>) =>
        this.restService.request<any, EmergencyVisitDto>({
            method: 'POST',
            url: '/api/app/emergency/register',
            body: input,
        },
            { apiName: this.apiName, ...config });

    performTriage = (id: string, input: TriageDto, config?: Partial<Rest.Config>) =>
        this.restService.request<any, EmergencyVisitDto>({
            method: 'POST',
            url: `/api/app/emergency/${id}/perform-triage`,
            body: input,
        },
            { apiName: this.apiName, ...config });

    updateStatus = (id: string, input: UpdateStatusDto, config?: Partial<Rest.Config>) =>
        this.restService.request<any, EmergencyVisitDto>({
            method: 'POST',
            url: `/api/app/emergency/${id}/update-status`,
            body: input,
        },
            { apiName: this.apiName, ...config });
}
