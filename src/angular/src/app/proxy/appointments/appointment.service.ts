import type { AppointmentDto, BookClinicAppointmentDto, CreateAppointmentDto, CreateUpdateWaitingListDto, LookupDto, WaitingListDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AppointmentService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  addToWaitingList = (input: CreateUpdateWaitingListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, WaitingListDto>({
      method: 'POST',
      url: '/api/app/appointment/to-waiting-list',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  bookClinicAppointment = (input: BookClinicAppointmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppointmentDto>({
      method: 'POST',
      url: '/api/app/appointment/book-clinic-appointment',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  cancel = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/appointment/${id}/cancel`,
    },
    { apiName: this.apiName,...config });
  

  checkIn = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/appointment/${id}/check-in`,
    },
    { apiName: this.apiName,...config });
  

  completeConsultation = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/appointment/${id}/complete-consultation`,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateAppointmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppointmentDto>({
      method: 'POST',
      url: '/api/app/appointment',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deleteFromWaitingList = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/appointment/${id}/from-waiting-list`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppointmentDto>({
      method: 'GET',
      url: `/api/app/appointment/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getAvailableSlots = (doctorId: string, date: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string[]>({
      method: 'GET',
      url: `/api/app/appointment/available-slots/${doctorId}`,
      params: { date },
    },
    { apiName: this.apiName,...config });
  

  getClinicLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto<string>>({
      method: 'GET',
      url: '/api/app/appointment/clinic-lookup',
    },
    { apiName: this.apiName,...config });
  

  getDoctorLookup = (clinicId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto<string>>({
      method: 'GET',
      url: `/api/app/appointment/doctor-lookup/${clinicId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (doctorId: string, startDate: string, endDate: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppointmentDto[]>({
      method: 'GET',
      url: '/api/app/appointment',
      params: { doctorId, startDate, endDate },
    },
    { apiName: this.apiName,...config });
  

  getTicketPdf = (appointmentId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: '/api/app/appointment/ticket-pdf',
      params: { appointmentId },
    },
    { apiName: this.apiName,...config });
  

  getWaitingList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<WaitingListDto>>({
      method: 'GET',
      url: '/api/app/appointment/waiting-list',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  startConsultation = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/appointment/${id}/start-consultation`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateAppointmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AppointmentDto>({
      method: 'PUT',
      url: `/api/app/appointment/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateWaitingList = (id: string, input: CreateUpdateWaitingListDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, WaitingListDto>({
      method: 'PUT',
      url: `/api/app/appointment/${id}/waiting-list`,
      body: input,
    },
    { apiName: this.apiName,...config });
}