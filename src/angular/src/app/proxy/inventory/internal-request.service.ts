import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { CreateUpdateInternalRequestDto, InternalRequestDto } from './dtos/models';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';

@Injectable({
  providedIn: 'root',
})
export class InternalRequestService {
  apiName = 'Default';

  create = (input: CreateUpdateInternalRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'POST',
      url: '/api/app/internal-request',
      body: input,
    },
    { apiName: this.apiName,...config });

  update = (id: string, input: CreateUpdateInternalRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'PUT',
      url: `/api/app/internal-request/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'GET',
      url: `/api/app/internal-request/${id}`,
    },
    { apiName: this.apiName,...config });

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InternalRequestDto>>({
      method: 'GET',
      url: '/api/app/internal-request',
      params: { 
        sorting: input.sorting, 
        skipCount: input.skipCount, 
        maxResultCount: input.maxResultCount 
      },
    },
    { apiName: this.apiName,...config });

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/internal-request/${id}`,
    },
    { apiName: this.apiName,...config });

  submitRequest = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'POST',
      url: `/api/app/internal-request/${id}/submit-request`,
    },
    { apiName: this.apiName,...config });

  approveAndFulfill = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'POST',
      url: `/api/app/internal-request/${id}/approve-and-fulfill`,
    },
    { apiName: this.apiName,...config });

  confirmReceipt = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'POST',
      url: `/api/app/internal-request/${id}/confirm-receipt`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
