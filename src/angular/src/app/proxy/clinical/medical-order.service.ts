import type { CreateUpdateMedicalOrderDto, MedicalOrderDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class MedicalOrderService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateMedicalOrderDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MedicalOrderDto>({
      method: 'POST',
      url: '/api/app/medical-order',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/medical-order/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MedicalOrderDto>({
      method: 'GET',
      url: `/api/app/medical-order/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<MedicalOrderDto>>({
      method: 'GET',
      url: '/api/app/medical-order',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateMedicalOrderDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MedicalOrderDto>({
      method: 'PUT',
      url: `/api/app/medical-order/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}