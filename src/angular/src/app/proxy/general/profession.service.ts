import type { CreateUpdateProfessionDto, ProfessionDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProfessionService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateProfessionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProfessionDto>({
      method: 'POST',
      url: '/api/app/profession',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/profession/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProfessionDto>({
      method: 'GET',
      url: `/api/app/profession/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProfessionDto>>({
      method: 'GET',
      url: '/api/app/profession',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateProfessionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProfessionDto>({
      method: 'PUT',
      url: `/api/app/profession/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}