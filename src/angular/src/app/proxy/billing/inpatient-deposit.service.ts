import type { CreateInpatientDepositDto, GetInpatientDepositsInput, InpatientDepositDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InpatientDepositService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateInpatientDepositDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InpatientDepositDto>({
      method: 'POST',
      url: '/api/app/inpatient-deposit',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/inpatient-deposit/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InpatientDepositDto>({
      method: 'GET',
      url: `/api/app/inpatient-deposit/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetInpatientDepositsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InpatientDepositDto>>({
      method: 'GET',
      url: '/api/app/inpatient-deposit',
      params: { patientId: input.patientId, admissionId: input.admissionId, status: input.status, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateInpatientDepositDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InpatientDepositDto>({
      method: 'PUT',
      url: `/api/app/inpatient-deposit/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}