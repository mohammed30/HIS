import type { ContractClaimDto, CreateUpdateContractClaimDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ContractClaimService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateContractClaimDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ContractClaimDto>({
      method: 'POST',
      url: '/api/app/contract-claim',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/contract-claim/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ContractClaimDto>({
      method: 'GET',
      url: `/api/app/contract-claim/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ContractClaimDto>>({
      method: 'GET',
      url: '/api/app/contract-claim',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateContractClaimDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ContractClaimDto>({
      method: 'PUT',
      url: `/api/app/contract-claim/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}