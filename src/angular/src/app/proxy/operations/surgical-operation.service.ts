import type { CreateUpdateSurgicalOperationDto, GetSurgicalOperationsInput, SurgicalOperationDto } from './models';
import type { OperationStatus } from './operation-status.enum';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SurgicalOperationService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateSurgicalOperationDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SurgicalOperationDto>({
      method: 'POST',
      url: '/api/app/surgical-operation',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/surgical-operation/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SurgicalOperationDto>({
      method: 'GET',
      url: `/api/app/surgical-operation/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetSurgicalOperationsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<SurgicalOperationDto>>({
      method: 'GET',
      url: '/api/app/surgical-operation',
      params: { searchText: input.searchText, patientId: input.patientId, doctorId: input.doctorId, status: input.status, specialtyId: input.specialtyId, fromDate: input.fromDate, toDate: input.toDate, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getOperationTicketPdf = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/api/app/surgical-operation/ticket-pdf/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getOperationsReportPdf = (input: GetSurgicalOperationsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: '/api/app/surgical-operation/report-pdf',
      params: { searchText: input.searchText, patientId: input.patientId, doctorId: input.doctorId, status: input.status, specialtyId: input.specialtyId, fromDate: input.fromDate, toDate: input.toDate, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateSurgicalOperationDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SurgicalOperationDto>({
      method: 'PUT',
      url: `/api/app/surgical-operation/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateStatus = (id: string, status: OperationStatus, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SurgicalOperationDto>({
      method: 'PUT',
      url: `/api/app/surgical-operation/${id}/status`,
      params: { status },
    },
    { apiName: this.apiName,...config });
}