import type { CreateUpdatePatientInsuranceDto, GetPatientInsurancesInput, PatientInsuranceDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PatientInsuranceService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdatePatientInsuranceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientInsuranceDto>({
      method: 'POST',
      url: '/api/app/patient-insurance',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/patient-insurance/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientInsuranceDto>({
      method: 'GET',
      url: `/api/app/patient-insurance/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByPatient = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientInsuranceDto[]>({
      method: 'GET',
      url: `/api/app/patient-insurance/by-patient/${patientId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetPatientInsurancesInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PatientInsuranceDto>>({
      method: 'GET',
      url: '/api/app/patient-insurance',
      params: { patientId: input.patientId, insurancePlanId: input.insurancePlanId, status: input.status, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdatePatientInsuranceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientInsuranceDto>({
      method: 'PUT',
      url: `/api/app/patient-insurance/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}