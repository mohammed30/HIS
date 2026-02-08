import type { CreateLabAppointmentDto, CreateLabRequestDto, CreateUpdateLabTestDto, LabAppointmentDto, LabRequestDto, LabTestDto, UpdateLabAppointmentDto, UpdateLabResultDto } from './dtos/models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LabService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  cancelAppointment = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/lab/${id}/cancel-appointment`,
    },
    { apiName: this.apiName,...config });
  

  checkInAppointment = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LabAppointmentDto>({
      method: 'POST',
      url: `/api/app/lab/${id}/check-in-appointment`,
    },
    { apiName: this.apiName,...config });
  

  collectSample = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LabRequestDto>({
      method: 'POST',
      url: `/api/app/lab/collect-sample/${id}`,
    },
    { apiName: this.apiName,...config });
  

  completeAppointment = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LabAppointmentDto>({
      method: 'POST',
      url: `/api/app/lab/${id}/complete-appointment`,
    },
    { apiName: this.apiName,...config });
  

  completeRequest = (id: string, input: UpdateLabResultDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LabRequestDto>({
      method: 'POST',
      url: `/api/app/lab/complete-request/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  confirmAppointment = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LabAppointmentDto>({
      method: 'POST',
      url: `/api/app/lab/${id}/confirm-appointment`,
    },
    { apiName: this.apiName,...config });
  

  createAppointment = (input: CreateLabAppointmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LabAppointmentDto>({
      method: 'POST',
      url: '/api/app/lab/appointment',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createRequest = (input: CreateLabRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LabRequestDto>({
      method: 'POST',
      url: '/api/app/lab/request',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createTest = (input: CreateUpdateLabTestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LabTestDto>({
      method: 'POST',
      url: '/api/app/lab/test',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deleteTest = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/lab/${id}/test`,
    },
    { apiName: this.apiName,...config });
  

  getAppointment = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LabAppointmentDto>({
      method: 'GET',
      url: `/api/app/lab/${id}/appointment`,
    },
    { apiName: this.apiName,...config });
  

  getAppointmentPdf = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/lab/appointment-pdf/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getAppointments = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LabAppointmentDto>>({
      method: 'GET',
      url: '/api/app/lab/appointments',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getRequestOrderPdf = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/lab/request-order-pdf/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getRequests = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LabRequestDto>>({
      method: 'GET',
      url: '/api/app/lab/requests',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getResultPdf = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/lab/result-pdf/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getTests = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<LabTestDto>>({
      method: 'GET',
      url: '/api/app/lab/tests',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  updateAppointment = (id: string, input: UpdateLabAppointmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LabAppointmentDto>({
      method: 'PUT',
      url: `/api/app/lab/${id}/appointment`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateTest = (id: string, input: CreateUpdateLabTestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, LabTestDto>({
      method: 'PUT',
      url: `/api/app/lab/test/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}