import type { CreateFallRiskAssessmentDto, CreatePainAssessmentDto, CreatePatientRoundDto, CreateShiftHandoverDto, CreateWoundCareDto, FallRiskAssessmentDto, PainAssessmentDto, PatientRoundDto, ShiftHandoverDto, WoundCareDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto, PagedResultRequestDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PatientCareService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  createFallRiskAssessment = (input: CreateFallRiskAssessmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, FallRiskAssessmentDto>({
      method: 'POST',
      url: '/api/app/patient-care/fall-risk-assessment',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createPainAssessment = (input: CreatePainAssessmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PainAssessmentDto>({
      method: 'POST',
      url: '/api/app/patient-care/pain-assessment',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createPatientRound = (input: CreatePatientRoundDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientRoundDto>({
      method: 'POST',
      url: '/api/app/patient-care/patient-round',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createShiftHandover = (input: CreateShiftHandoverDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ShiftHandoverDto>({
      method: 'POST',
      url: '/api/app/patient-care/shift-handover',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createWoundCare = (input: CreateWoundCareDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, WoundCareDto>({
      method: 'POST',
      url: '/api/app/patient-care/wound-care',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getFallRiskAssessments = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<FallRiskAssessmentDto>>({
      method: 'GET',
      url: `/api/app/patient-care/fall-risk-assessments/${patientId}`,
    },
    { apiName: this.apiName,...config });
  

  getPainAssessments = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PainAssessmentDto>>({
      method: 'GET',
      url: `/api/app/patient-care/pain-assessments/${patientId}`,
    },
    { apiName: this.apiName,...config });
  

  getPatientRounds = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PatientRoundDto>>({
      method: 'GET',
      url: `/api/app/patient-care/patient-rounds/${patientId}`,
    },
    { apiName: this.apiName,...config });
  

  getShiftHandovers = (input: PagedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ShiftHandoverDto>>({
      method: 'GET',
      url: '/api/app/patient-care/shift-handovers',
      params: { skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getWoundCares = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<WoundCareDto>>({
      method: 'GET',
      url: `/api/app/patient-care/wound-cares/${patientId}`,
    },
    { apiName: this.apiName,...config });
}