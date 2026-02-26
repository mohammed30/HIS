import type { CreateUpdateJobTitleDto, JobTitleDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class JobTitleService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateJobTitleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, JobTitleDto>({
      method: 'POST',
      url: '/api/app/job-title',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/job-title/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, JobTitleDto>({
      method: 'GET',
      url: `/api/app/job-title/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<JobTitleDto>>({
      method: 'GET',
      url: '/api/app/job-title',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateJobTitleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, JobTitleDto>({
      method: 'PUT',
      url: `/api/app/job-title/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}