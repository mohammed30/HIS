import type { ContractDto, CreateUpdateContractDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ContractService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateContractDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ContractDto>({
      method: 'POST',
      url: '/api/app/contract',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/contract/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ContractDto>({
      method: 'GET',
      url: `/api/app/contract/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ContractDto>>({
      method: 'GET',
      url: '/api/app/contract',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateContractDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ContractDto>({
      method: 'PUT',
      url: `/api/app/contract/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}