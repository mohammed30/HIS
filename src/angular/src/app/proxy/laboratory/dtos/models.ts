import type { AuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { LabRequestStatus } from '../lab-request-status.enum';
import type { LabAppointmentStatus } from '../lab-appointment-status.enum';

export interface LabTestDto extends AuditedEntityDto<string> {
    code: string;
    name: string;
    price: number;
    instructions: string;
    referenceRange: string;
    unit: string;
    isActive: boolean;
}

export interface CreateUpdateLabTestDto {
    code: string;
    name: string;
    price: number;
    instructions: string;
    referenceRange: string;
    unit: string;
    isActive: boolean;
}

export interface LabRequestDto extends AuditedEntityDto<string> {
    patientId: string;
    patientName: string;
    doctorId: string;
    doctorName: string;
    testId: string;
    testName: string;
    testCode: string;
    requestDate: string;
    status: LabRequestStatus;
    result: string;
    notes: string;
}

export interface CreateLabRequestDto {
    patientId: string;
    doctorId: string;
    serviceItemId: string;
    notes: string;
}

export interface UpdateLabResultDto {
    result: string;
    notes: string;
}

// --- Lab Appointments ---

export interface LabAppointmentDto extends AuditedEntityDto<string> {
    patientId: string;
    patientName: string;
    serviceItemId?: string;
    testName?: string;
    testCode?: string;
    appointmentDate: string;
    preferredTime?: string;
    status: LabAppointmentStatus;
    notes?: string;
    preparationInstructions?: string;
    isFasting: boolean;
}

export interface CreateLabAppointmentDto {
    patientId: string;
    serviceItemId?: string;
    appointmentDate: string;
    preferredTime?: string;
    notes?: string;
    isFasting: boolean;
}

export interface UpdateLabAppointmentDto {
    serviceItemId?: string;
    appointmentDate: string;
    preferredTime?: string;
    notes?: string;
    isFasting: boolean;
}

