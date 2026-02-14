import { RestService } from '@abp/ng.core';
import { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import {
    FluidBalanceDto, CreateFluidBalanceDto, FluidBalanceSummaryDto
} from './models';

@Injectable({
    providedIn: 'root',
})
export class FluidBalanceService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    getList = (patientId: string) =>
        this.restService.request<any, PagedResultDto<FluidBalanceDto>>({
            method: 'GET',
            url: `/api/app/fluid-balance/${patientId}`,
        },
            { apiName: this.apiName });

    create = (input: CreateFluidBalanceDto) =>
        this.restService.request<any, FluidBalanceDto>({
            method: 'POST',
            url: '/api/app/fluid-balance',
            body: input,
        },
            { apiName: this.apiName });

    getSummary = (patientId: string, date: string) =>
        this.restService.request<any, FluidBalanceSummaryDto>({
            method: 'GET',
            url: `/api/app/fluid-balance/summary/${patientId}`,
            params: { date },
        },
            { apiName: this.apiName });
}
