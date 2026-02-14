import { RestService } from '@abp/ng.core';
import { PagedResultDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import {
    PatientRoundDto, CreatePatientRoundDto,
    PainAssessmentDto, CreatePainAssessmentDto,
    FallRiskAssessmentDto, CreateFallRiskAssessmentDto,
    WoundCareDto, CreateWoundCareDto,
    ShiftHandoverDto, CreateShiftHandoverDto
} from './models';

@Injectable({
    providedIn: 'root',
})
export class PatientCareService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    // Rounds
    getPatientRounds = (patientId: string) =>
        this.restService.request<any, PagedResultDto<PatientRoundDto>>({
            method: 'GET',
            url: `/api/app/patient-care/patient-rounds/${patientId}`,
        },
            { apiName: this.apiName });

    createPatientRound = (input: CreatePatientRoundDto) =>
        this.restService.request<any, PatientRoundDto>({
            method: 'POST',
            url: '/api/app/patient-care/patient-round',
            body: input,
        },
            { apiName: this.apiName });

    // Pain
    getPainAssessments = (patientId: string) =>
        this.restService.request<any, PagedResultDto<PainAssessmentDto>>({
            method: 'GET',
            url: `/api/app/patient-care/pain-assessments/${patientId}`,
        },
            { apiName: this.apiName });

    createPainAssessment = (input: CreatePainAssessmentDto) =>
        this.restService.request<any, PainAssessmentDto>({
            method: 'POST',
            url: '/api/app/patient-care/pain-assessment',
            body: input,
        },
            { apiName: this.apiName });

    // Fall Risk
    getFallRiskAssessments = (patientId: string) =>
        this.restService.request<any, PagedResultDto<FallRiskAssessmentDto>>({
            method: 'GET',
            url: `/api/app/patient-care/fall-risk-assessments/${patientId}`,
        },
            { apiName: this.apiName });

    createFallRiskAssessment = (input: CreateFallRiskAssessmentDto) =>
        this.restService.request<any, FallRiskAssessmentDto>({
            method: 'POST',
            url: '/api/app/patient-care/fall-risk-assessment',
            body: input,
        },
            { apiName: this.apiName });

    // Wound Care
    getWoundCares = (patientId: string) =>
        this.restService.request<any, PagedResultDto<WoundCareDto>>({
            method: 'GET',
            url: `/api/app/patient-care/wound-cares/${patientId}`,
        },
            { apiName: this.apiName });

    createWoundCare = (input: CreateWoundCareDto) =>
        this.restService.request<any, WoundCareDto>({
            method: 'POST',
            url: '/api/app/patient-care/wound-care',
            body: input,
        },
            { apiName: this.apiName });

    // Handover
    getShiftHandovers = (input: any) =>
        this.restService.request<any, PagedResultDto<ShiftHandoverDto>>({
            method: 'GET',
            url: '/api/app/patient-care/shift-handovers',
            params: input,
        },
            { apiName: this.apiName });

    createShiftHandover = (input: CreateShiftHandoverDto) =>
        this.restService.request<any, ShiftHandoverDto>({
            method: 'POST',
            url: '/api/app/patient-care/shift-handover',
            body: input,
        },
            { apiName: this.apiName });
}
