import type { CreateUpdateRadiologyItemDto, CreateUpdateServiceItemDto, RadiologyItemDto, ServiceItemDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ServiceItemService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateServiceItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceItemDto>({
      method: 'POST',
      url: '/api/app/service-item',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createRadiology = (input: CreateUpdateRadiologyItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RadiologyItemDto>({
      method: 'POST',
      url: '/api/app/service-item/radiology',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/service-item/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceItemDto>({
      method: 'GET',
      url: `/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ServiceItemDto>>({
      method: 'GET',
      url: '/api/app/service-item',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getRadiologyList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<RadiologyItemDto>>({
      method: 'GET',
      url: '/api/app/service-item/radiology',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateServiceItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ServiceItemDto>({
      method: 'PUT',
      url: `/api/app/service-item/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateRadiology = (id: string, input: CreateUpdateRadiologyItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RadiologyItemDto>({
      method: 'PUT',
      url: `/api/app/service-item/radiology/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}