import type { CreateEmergencyVisitDto, EmergencyVisitDto, TriageDto, UpdateStatusDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
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
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  performTriage = (id: string, input: TriageDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmergencyVisitDto>({
      method: 'POST',
      url: `/api/app/emergency/${id}/perform-triage`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  register = (input: CreateEmergencyVisitDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmergencyVisitDto>({
      method: 'POST',
      url: '/api/app/emergency/register',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateStatus = (id: string, input: UpdateStatusDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, EmergencyVisitDto>({
      method: 'PUT',
      url: `/api/app/emergency/${id}/status`,
      body: input,
    },
    { apiName: this.apiName,...config });
}