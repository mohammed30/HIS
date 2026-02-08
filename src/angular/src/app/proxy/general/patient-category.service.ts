import type { CreateUpdatePatientCategoryDto, PatientCategoryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PatientCategoryService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdatePatientCategoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientCategoryDto>({
      method: 'POST',
      url: '/api/app/patient-category',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/patient-category/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientCategoryDto>({
      method: 'GET',
      url: `/api/app/patient-category/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PatientCategoryDto>>({
      method: 'GET',
      url: '/api/app/patient-category',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdatePatientCategoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientCategoryDto>({
      method: 'PUT',
      url: `/api/app/patient-category/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}