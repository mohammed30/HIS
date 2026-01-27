import type { CreateUpdateLaboratoryDto, GetLaboratoriesInput, LaboratoryDto, LookupDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LaboratoryService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateLaboratoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LaboratoryDto>({
      method: 'POST',
      url: '/api/app/laboratory',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/laboratory/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LaboratoryDto>({
      method: 'GET',
      url: `/api/app/laboratory/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetLaboratoriesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LaboratoryDto>>({
      method: 'GET',
      url: '/api/app/laboratory',
      params: { searchText: input.searchText, isActive: input.isActive, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto[]>({
      method: 'GET',
      url: '/api/app/laboratory/lookup',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateLaboratoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LaboratoryDto>({
      method: 'PUT',
      url: `/api/app/laboratory/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}