import type { CreateUpdateReferralSourceDto, ReferralSourceDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ReferralSourceService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateReferralSourceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ReferralSourceDto>({
      method: 'POST',
      url: '/api/app/referral-source',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/referral-source/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ReferralSourceDto>({
      method: 'GET',
      url: `/api/app/referral-source/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ReferralSourceDto>>({
      method: 'GET',
      url: '/api/app/referral-source',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateReferralSourceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ReferralSourceDto>({
      method: 'PUT',
      url: `/api/app/referral-source/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}