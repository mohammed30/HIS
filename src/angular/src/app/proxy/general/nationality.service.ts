import type { CreateUpdateNationalityDto, NationalityDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NationalityService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateNationalityDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, NationalityDto>({
      method: 'POST',
      url: '/api/app/nationality',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/nationality/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, NationalityDto>({
      method: 'GET',
      url: `/api/app/nationality/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<NationalityDto>>({
      method: 'GET',
      url: '/api/app/nationality',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateNationalityDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, NationalityDto>({
      method: 'PUT',
      url: `/api/app/nationality/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}