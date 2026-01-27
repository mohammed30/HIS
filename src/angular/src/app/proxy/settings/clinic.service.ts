import type { ClinicDto, CreateUpdateClinicDto, GetClinicsInput, LookupDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ClinicService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateClinicDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ClinicDto>({
      method: 'POST',
      url: '/api/app/clinic',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/clinic/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ClinicDto>({
      method: 'GET',
      url: `/api/app/clinic/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByDepartment = (departmentId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ClinicDto[]>({
      method: 'GET',
      url: `/api/app/clinic/by-department/${departmentId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetClinicsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ClinicDto>>({
      method: 'GET',
      url: '/api/app/clinic',
      params: { searchText: input.searchText, departmentId: input.departmentId, isActive: input.isActive, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto[]>({
      method: 'GET',
      url: '/api/app/clinic/lookup',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateClinicDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ClinicDto>({
      method: 'PUT',
      url: `/api/app/clinic/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}