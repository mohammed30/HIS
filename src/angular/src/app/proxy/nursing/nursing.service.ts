import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { CarePlanDto, CreateCarePlanDto, CreateMedicationAdministrationDto, MedicationAdministrationDto, DueMedicationDto } from './models';

@Injectable({
    providedIn: 'root',
})
export class NursingService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    createMedicationAdministration = (input: CreateMedicationAdministrationDto) =>
        this.restService.request<any, MedicationAdministrationDto>({
            method: 'POST',
            url: '/api/app/nursing/medication-administration',
            body: input,
        },
            { apiName: this.apiName });

    getMedicationAdministrations = (patientId: string) =>
        this.restService.request<any, MedicationAdministrationDto[]>({
            method: 'GET',
            url: `/api/app/nursing/medication-administrations?patientId=${patientId}`,
        },
            { apiName: this.apiName });

    getDueMedications = (patientId: string) =>
        this.restService.request<any, DueMedicationDto[]>({
            method: 'GET',
            url: `/api/app/nursing/due-medications?patientId=${patientId}`,
        },
            { apiName: this.apiName });

    createCarePlan = (input: CreateCarePlanDto) =>
        this.restService.request<any, CarePlanDto>({
            method: 'POST',
            url: '/api/app/nursing/care-plan',
            body: input,
        },
            { apiName: this.apiName });

    getCarePlans = (patientId: string) =>
        this.restService.request<any, CarePlanDto[]>({
            method: 'GET',
            url: `/api/app/nursing/care-plans?patientId=${patientId}`,
        },
            { apiName: this.apiName });

    updateCarePlan = (id: string, input: CreateCarePlanDto) =>
        this.restService.request<any, CarePlanDto>({
            method: 'PUT',
            url: `/api/app/nursing/care-plan/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    deleteCarePlan = (id: string) =>
        this.restService.request<void, void>({
            method: 'DELETE',
            url: `/api/app/nursing/care-plan/${id}`,
        },
            { apiName: this.apiName });
}
