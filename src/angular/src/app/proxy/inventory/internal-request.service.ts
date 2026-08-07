import type { CreateUpdateInternalRequestDto, InternalRequestDto, InternalRequestGetListInput, ReturnInternalRequestDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InternalRequestService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  approveAndFulfill = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'POST',
      url: `/api/app/internal-request/${id}/approve-and-fulfill`,
    },
    { apiName: this.apiName,...config });
  

  approveReturn = (requestId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'POST',
      url: `/api/app/internal-request/approve-return/${requestId}`,
    },
    { apiName: this.apiName,...config });
  

  cancelRequest = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'POST',
      url: `/api/app/internal-request/${id}/cancel-request`,
    },
    { apiName: this.apiName,...config });
  

  confirmReceipt = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'POST',
      url: `/api/app/internal-request/${id}/confirm-receipt`,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateUpdateInternalRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'POST',
      url: '/api/app/internal-request',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/internal-request/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'GET',
      url: `/api/app/internal-request/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: InternalRequestGetListInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InternalRequestDto>>({
      method: 'GET',
      url: '/api/app/internal-request',
      params: { fromDate: input.fromDate, toDate: input.toDate, filterText: input.filterText, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getPendingReturns = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InternalRequestDto>>({
      method: 'GET',
      url: '/api/app/internal-request/pending-returns',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  returnItems = (input: ReturnInternalRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'POST',
      url: '/api/app/internal-request/return-items',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  submitRequest = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'POST',
      url: `/api/app/internal-request/${id}/submit-request`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateInternalRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InternalRequestDto>({
      method: 'PUT',
      url: `/api/app/internal-request/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}