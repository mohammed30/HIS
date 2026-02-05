import type { AllergyDto, CreateUpdateAllergyDto, CreateUpdateDiagnosisDto, CreateUpdateMedicalHistoryDto, CreateUpdatePatientNoteDto, CreateUpdateVitalSignDto, DiagnosisDto, MedicalHistoryDto, PatientMedicalSummaryDto, PatientNoteDto, VitalSignDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class MedicalRecordService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  createAllergy = (input: CreateUpdateAllergyDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AllergyDto>({
      method: 'POST',
      url: '/api/app/medical-records/allergies',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createDiagnosis = (input: CreateUpdateDiagnosisDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DiagnosisDto>({
      method: 'POST',
      url: '/api/app/medical-records/diagnoses',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createMedicalHistory = (input: CreateUpdateMedicalHistoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MedicalHistoryDto>({
      method: 'POST',
      url: '/api/app/medical-records/history',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createNote = (input: CreateUpdatePatientNoteDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientNoteDto>({
      method: 'POST',
      url: '/api/app/medical-records/notes',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createVitalSign = (input: CreateUpdateVitalSignDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, VitalSignDto>({
      method: 'POST',
      url: '/api/app/medical-records/vital-signs',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deleteAllergy = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/medical-records/allergies/${id}`,
    },
    { apiName: this.apiName,...config });
  

  deleteDiagnosis = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/medical-records/diagnoses/${id}`,
    },
    { apiName: this.apiName,...config });
  

  deleteMedicalHistory = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/medical-records/history/${id}`,
    },
    { apiName: this.apiName,...config });
  

  deleteNote = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/medical-records/notes/${id}`,
    },
    { apiName: this.apiName,...config });
  

  deleteVitalSign = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/medical-records/vital-signs/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getAllergies = (patientId: string, activeOnly?: boolean, skipCount?: number, maxResultCount: number = 20, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<AllergyDto>>({
      method: 'GET',
      url: `/api/app/medical-records/allergies/${patientId}`,
      params: { activeOnly, skipCount, maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getDiagnoses = (patientId: string, skipCount?: number, maxResultCount: number = 20, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<DiagnosisDto>>({
      method: 'GET',
      url: `/api/app/medical-records/diagnoses/${patientId}`,
      params: { skipCount, maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getLatestVitals = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, VitalSignDto>({
      method: 'GET',
      url: `/api/app/medical-records/vital-signs/${patientId}/latest`,
    },
    { apiName: this.apiName,...config });
  

  getMedicalHistory = (patientId: string, skipCount?: number, maxResultCount: number = 20, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<MedicalHistoryDto>>({
      method: 'GET',
      url: `/api/app/medical-records/history/${patientId}`,
      params: { skipCount, maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getNotes = (patientId: string, skipCount?: number, maxResultCount: number = 20, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PatientNoteDto>>({
      method: 'GET',
      url: `/api/app/medical-records/notes/${patientId}`,
      params: { skipCount, maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getSummary = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientMedicalSummaryDto>({
      method: 'GET',
      url: `/api/app/medical-records/summary/${patientId}`,
    },
    { apiName: this.apiName,...config });
  

  getVitalSigns = (patientId: string, skipCount?: number, maxResultCount: number = 20, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<VitalSignDto>>({
      method: 'GET',
      url: `/api/app/medical-records/vital-signs/${patientId}`,
      params: { skipCount, maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  updateAllergy = (id: string, input: CreateUpdateAllergyDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, AllergyDto>({
      method: 'PUT',
      url: `/api/app/medical-records/allergies/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateDiagnosis = (id: string, input: CreateUpdateDiagnosisDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DiagnosisDto>({
      method: 'PUT',
      url: `/api/app/medical-records/diagnoses/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateMedicalHistory = (id: string, input: CreateUpdateMedicalHistoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MedicalHistoryDto>({
      method: 'PUT',
      url: `/api/app/medical-records/history/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateNote = (id: string, input: CreateUpdatePatientNoteDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PatientNoteDto>({
      method: 'PUT',
      url: `/api/app/medical-records/notes/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}