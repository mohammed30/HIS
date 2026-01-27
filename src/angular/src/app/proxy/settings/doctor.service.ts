import type { CreateUpdateDoctorDto, DoctorDto, GetDoctorsInput, LookupDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DoctorService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateDoctorDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DoctorDto>({
      method: 'POST',
      url: '/api/app/doctor',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/doctor/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DoctorDto>({
      method: 'GET',
      url: `/api/app/doctor/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByDepartment = (departmentId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DoctorDto[]>({
      method: 'GET',
      url: `/api/app/doctor/by-department/${departmentId}`,
    },
    { apiName: this.apiName,...config });
  

  getBySpecialty = (specialtyId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DoctorDto[]>({
      method: 'GET',
      url: `/api/app/doctor/by-specialty/${specialtyId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetDoctorsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DoctorDto>>({
      method: 'GET',
      url: '/api/app/doctor',
      params: { searchText: input.searchText, specialtyId: input.specialtyId, departmentId: input.departmentId, isActive: input.isActive, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto[]>({
      method: 'GET',
      url: '/api/app/doctor/lookup',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateDoctorDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DoctorDto>({
      method: 'PUT',
      url: `/api/app/doctor/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}