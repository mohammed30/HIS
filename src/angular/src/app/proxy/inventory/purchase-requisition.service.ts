import type { CreateUpdatePurchaseRequisitionDto, GetPurchaseRequisitionsInput, PurchaseRequisitionDto } from './dtos/models';
import type { PurchaseRequisitionStatus } from './purchase-requisition-status.enum';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PurchaseRequisitionService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdatePurchaseRequisitionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseRequisitionDto>({
      method: 'POST',
      url: '/api/app/purchase-requisition',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/purchase-requisition/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseRequisitionDto>({
      method: 'GET',
      url: `/api/app/purchase-requisition/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetPurchaseRequisitionsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PurchaseRequisitionDto>>({
      method: 'GET',
      url: '/api/app/purchase-requisition',
      params: { filter: input.filter, status: input.status, departmentId: input.departmentId, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdatePurchaseRequisitionDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PurchaseRequisitionDto>({
      method: 'PUT',
      url: `/api/app/purchase-requisition/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateStatus = (id: string, status: PurchaseRequisitionStatus, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: `/api/app/purchase-requisition/${id}/status`,
      params: { status },
    },
    { apiName: this.apiName,...config });
}