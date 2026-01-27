import type { AppointmentDto, CreateAppointmentDto, LookupDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AppointmentService {
  private restService = inject(RestService);
  apiName = 'Default';


  cancel = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/appointment/${id}/cancel`,
    },
      { apiName: this.apiName, ...config });


  create = (input: CreateAppointmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppointmentDto>({
      method: 'POST',
      url: '/api/app/appointment',
      body: input,
    },
      { apiName: this.apiName, ...config });


  update = (id: string, input: CreateAppointmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppointmentDto>({
      method: 'PUT',
      url: `/api/app/appointment/${id}`,
      body: input,
    },
      { apiName: this.apiName, ...config });


  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppointmentDto>({
      method: 'GET',
      url: `/api/app/appointment/${id}`,
    },
      { apiName: this.apiName, ...config });


  getAvailableSlots = (doctorId: string, date: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string[]>({
      method: 'GET',
      url: `/api/app/appointment/available-slots/${doctorId}`,
      params: { date },
    },
      { apiName: this.apiName, ...config });


  getClinicLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto<string>>({
      method: 'GET',
      url: '/api/app/appointment/clinic-lookup',
    },
      { apiName: this.apiName, ...config });


  getDoctorLookup = (clinicId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto<string>>({
      method: 'GET',
      url: `/api/app/appointment/doctor-lookup/${clinicId}`,
    },
      { apiName: this.apiName, ...config });


  getList = (doctorId?: string, startDate?: string, endDate?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppointmentDto[]>({
      method: 'GET',
      url: '/api/app/appointment', // NOTE: Query string might need to be explicitly built if standard ABP mapping maps simple params
      params: { doctorId, startDate, endDate },
    },
      { apiName: this.apiName, ...config });
}