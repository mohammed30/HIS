import type { CreateUpdateDrugDto, DrugDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DrugService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateDrugDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DrugDto>({
      method: 'POST',
      url: '/api/app/drug',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/drug/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DrugDto>({
      method: 'GET',
      url: `/api/app/drug/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DrugDto>>({
      method: 'GET',
      url: '/api/app/drug',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateDrugDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DrugDto>({
      method: 'PUT',
      url: `/api/app/drug/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}