import type { AuditedEntityDto } from '@abp/ng.core';
import type { LabAppointmentStatus } from '../lab-appointment-status.enum';
import type { LabRequestStatus } from '../lab-request-status.enum';

export interface CreateLabAppointmentDto {
  patientId?: string;
  serviceItemId?: string;
  appointmentDate?: string;
  preferredTime?: string;
  notes?: string;
  isFasting?: boolean;
}

export interface CreateLabRequestDto {
  patientId?: string;
  doctorId?: string;
  serviceItemId?: string;
  notes?: string;
}

export interface CreateUpdateLabTestDto {
  code?: string;
  name: string;
  price?: number;
  instructions?: string;
  referenceRange?: string;
  unit?: string;
  isActive?: boolean;
}

export interface LabAppointmentDto extends AuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  serviceItemId?: string;
  testName?: string;
  testCode?: string;
  appointmentDate?: string;
  preferredTime?: string;
  status?: LabAppointmentStatus;
  notes?: string;
  preparationInstructions?: string;
  isFasting?: boolean;
}

export interface LabRequestDto extends AuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  doctorId?: string;
  doctorName?: string;
  serviceItemId?: string;
  testName?: string;
  testCode?: string;
  requestDate?: string;
  status?: LabRequestStatus;
  result?: string;
  notes?: string;
}

export interface LabTestDto extends AuditedEntityDto<string> {
  code?: string;
  name?: string;
  price?: number;
  instructions?: string;
  referenceRange?: string;
  unit?: string;
  isActive?: boolean;
}

export interface UpdateLabAppointmentDto {
  serviceItemId?: string;
  appointmentDate?: string;
  preferredTime?: string;
  notes?: string;
  isFasting?: boolean;
}

export interface UpdateLabResultDto {
  result?: string;
  notes?: string;
}
