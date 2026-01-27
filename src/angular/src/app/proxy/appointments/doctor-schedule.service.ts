import type { CreateUpdateDoctorScheduleDto, DoctorScheduleDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DoctorScheduleService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateDoctorScheduleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DoctorScheduleDto>({
      method: 'POST',
      url: '/api/app/doctor-schedule',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/doctor-schedule/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DoctorScheduleDto>({
      method: 'GET',
      url: `/api/app/doctor-schedule/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DoctorScheduleDto>>({
      method: 'GET',
      url: '/api/app/doctor-schedule',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateDoctorScheduleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DoctorScheduleDto>({
      method: 'PUT',
      url: `/api/app/doctor-schedule/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}