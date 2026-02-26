import type { CarePlanDto, CreateCarePlanDto, CreateMedicationAdministrationDto, DueMedicationDto, MedicationAdministrationDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NursingService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  createCarePlan = (input: CreateCarePlanDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarePlanDto>({
      method: 'POST',
      url: '/api/app/nursing/care-plan',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createMedicationAdministration = (input: CreateMedicationAdministrationDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MedicationAdministrationDto>({
      method: 'POST',
      url: '/api/app/nursing/medication-administration',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deleteCarePlan = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/nursing/${id}/care-plan`,
    },
    { apiName: this.apiName,...config });
  

  getCarePlans = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarePlanDto[]>({
      method: 'GET',
      url: `/api/app/nursing/care-plans/${patientId}`,
    },
    { apiName: this.apiName,...config });
  

  getDueMedications = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DueMedicationDto[]>({
      method: 'GET',
      url: `/api/app/nursing/due-medications/${patientId}`,
    },
    { apiName: this.apiName,...config });
  

  getMedicationAdministrations = (patientId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, MedicationAdministrationDto[]>({
      method: 'GET',
      url: `/api/app/nursing/medication-administrations/${patientId}`,
    },
    { apiName: this.apiName,...config });
  

  updateCarePlan = (id: string, input: CreateCarePlanDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CarePlanDto>({
      method: 'PUT',
      url: `/api/app/nursing/${id}/care-plan`,
      body: input,
    },
    { apiName: this.apiName,...config });
}