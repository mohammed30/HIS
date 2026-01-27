import type { CreateUpdatePatientDto, GetPatientsInput, PatientDto, PatientLookupDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PatientService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdatePatientDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientDto>({
      method: 'POST',
      url: '/api/app/patient',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/patient/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientDto>({
      method: 'GET',
      url: `/api/app/patient/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByIdentityNumber = (identityNumber: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientDto>({
      method: 'GET',
      url: '/api/app/patient/by-identity-number',
      params: { identityNumber },
    },
    { apiName: this.apiName,...config });
  

  getByMRN = (mrn: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientDto>({
      method: 'GET',
      url: '/api/app/patient/by-mRN',
      params: { mrn },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetPatientsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PatientDto>>({
      method: 'GET',
      url: '/api/app/patient',
      params: { searchText: input.searchText, mrn: input.mrn, identityNumber: input.identityNumber, mobileNumber: input.mobileNumber, gender: input.gender, category: input.category, isActive: input.isActive, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  search = (searchText: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientLookupDto[]>({
      method: 'POST',
      url: '/api/app/patient/search',
      params: { searchText },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdatePatientDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientDto>({
      method: 'PUT',
      url: `/api/app/patient/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}