import type { CreateUpdateDepartmentDto, DepartmentDto, GetDepartmentsInput, LookupDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DepartmentService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateDepartmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DepartmentDto>({
      method: 'POST',
      url: '/api/app/department',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/department/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DepartmentDto>({
      method: 'GET',
      url: `/api/app/department/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetDepartmentsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DepartmentDto>>({
      method: 'GET',
      url: '/api/app/department',
      params: { searchText: input.searchText, isActive: input.isActive, isMedical: input.isMedical, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto[]>({
      method: 'GET',
      url: '/api/app/department/lookup',
    },
    { apiName: this.apiName,...config });
  

  getMedicalDepartmentsLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto[]>({
      method: 'GET',
      url: '/api/app/department/medical-departments-lookup',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateDepartmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DepartmentDto>({
      method: 'PUT',
      url: `/api/app/department/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}