import type { AuditedEntityDto } from '@abp/ng.core';
import type { EmergencySeverity, EmergencyVisitStatus } from '../enums';

export interface EmergencyVisitDto extends AuditedEntityDto<string> {
    patientId: string;
    patientName: string;
    arrivalTime: string;
    severity: EmergencySeverity;
    status: EmergencyVisitStatus;
    chiefComplaint: string;
    bloodPressure: string;
    heartRate: number;
    temperature: number;
    respiratoryRate: number;
    oxygenSaturation: number;
    notes: string;
}

export interface CreateEmergencyVisitDto {
    patientId: string;
    chiefComplaint: string;
}

export interface TriageDto {
    severity: EmergencySeverity;
    bloodPressure: string;
    heartRate: number;
    temperature: number;
    respiratoryRate: number;
    oxygenSaturation: number;
    notes: string;
}

export interface UpdateStatusDto {
    status: EmergencyVisitStatus;
    notes: string;
}
