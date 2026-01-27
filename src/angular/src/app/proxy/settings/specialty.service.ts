import type { CreateUpdateSpecialtyDto, GetSpecialtiesInput, LookupDto, SpecialtyDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SpecialtyService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateSpecialtyDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SpecialtyDto>({
      method: 'POST',
      url: '/api/app/specialty',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/specialty/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SpecialtyDto>({
      method: 'GET',
      url: `/api/app/specialty/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetSpecialtiesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<SpecialtyDto>>({
      method: 'GET',
      url: '/api/app/specialty',
      params: { searchText: input.searchText, isActive: input.isActive, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto[]>({
      method: 'GET',
      url: '/api/app/specialty/lookup',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateSpecialtyDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SpecialtyDto>({
      method: 'PUT',
      url: `/api/app/specialty/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}