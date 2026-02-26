import type { CreateFluidBalanceDto, FluidBalanceDto, FluidBalanceSummaryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FluidBalanceService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateFluidBalanceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, FluidBalanceDto>({
      method: 'POST',
      url: '/api/app/fluid-balance',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getList = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<FluidBalanceDto>>({
      method: 'GET',
      url: '/api/app/fluid-balance',
      params: { patientId },
    },
    { apiName: this.apiName,...config });
  

  getSummary = (patientId: string, date: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, FluidBalanceSummaryDto>({
      method: 'GET',
      url: `/api/app/fluid-balance/summary/${patientId}`,
      params: { date },
    },
    { apiName: this.apiName,...config });
}