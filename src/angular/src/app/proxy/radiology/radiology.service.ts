import type { CreateUpdateRadiologyRequestDto, GetRadiologyRequestInput, RadiologyRequestDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class RadiologyService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateRadiologyRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RadiologyRequestDto>({
      method: 'POST',
      url: '/api/app/radiology',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/radiology/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RadiologyRequestDto>({
      method: 'GET',
      url: `/api/app/radiology/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetRadiologyRequestInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<RadiologyRequestDto>>({
      method: 'GET',
      url: '/api/app/radiology',
      params: { filter: input.filter, status: input.status, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getPatientResults = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RadiologyRequestDto[]>({
      method: 'GET',
      url: '/patient-results',
      params: { patientId },
    },
    { apiName: this.apiName,...config });
  

  getRadiologyResultPdf = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, Blob>({
      method: 'GET',
      responseType: 'blob',
      url: `/result-pdf/${id}`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateRadiologyRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RadiologyRequestDto>({
      method: 'PUT',
      url: `/api/app/radiology/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}