import type { AuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { LabRequestStatus } from '../lab-request-status.enum';
import type { LabAppointmentStatus } from '../lab-appointment-status.enum';

export interface CreateLabAppointmentDto {
  patientId?: string;
  serviceItemId?: string | null;
  appointmentDate?: string;
  preferredTime?: string | null;
  notes?: string | null;
  isFasting?: boolean;
}

export interface CreateLabRequestDto {
  patientId?: string;
  doctorId?: string | null;
  isExternalDoctor?: boolean;
  externalDoctorName?: string | null;
  serviceItemId?: string;
  notes?: string;
}

export interface CreateUpdateLabTestDto {
  code?: string | null;
  name: string;
  price?: number;
  instructions?: string | null;
  referenceRange?: string | null;
  unit?: string | null;
  categoryId?: string | null;
  isActive?: boolean;
  machine?: string | null;
  turnaroundTime?: string | null;
  normalRanges?: CreateLabTestNormalRangeDto[];
}

export interface CreateLabTestNormalRangeDto {
  targetGender?: number | null; // GenderType
  minAgeDays?: number | null;
  maxAgeDays?: number | null;
  resultType?: number; // LabResultType
  minValue?: number | null;
  maxValue?: number | null;
  normalStringValue?: string | null;
}

export interface GetLabRequestsInput extends PagedAndSortedResultRequestDto {
  fromDate?: string | null;
  toDate?: string | null;
  filter?: string | null;
  status?: LabRequestStatus | null;
}

export interface LabAppointmentDto extends AuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  serviceItemId?: string | null;
  testName?: string | null;
  testCode?: string | null;
  appointmentDate?: string;
  preferredTime?: string | null;
  status?: LabAppointmentStatus;
  notes?: string | null;
  preparationInstructions?: string | null;
  isFasting?: boolean;
}

export interface LabRequestDto extends AuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  doctorId?: string | null;
  doctorName?: string | null;
  isExternalDoctor?: boolean;
  externalDoctorName?: string | null;
  serviceItemId?: string;
  testName?: string;
  testCode?: string;
  requestDate?: string;
  status?: LabRequestStatus;
  sampleNumber?: string | null;
  result?: string;
  notes?: string;
  requestingDepartmentName?: string | null;
  admissionRoom?: string | null;
  referenceRange?: string | null;
  unit?: string | null;
}

export interface LabTestCategoryDto extends AuditedEntityDto<string> {
  code?: string;
  name?: string;
  parentId?: string | null;
  sortOrder?: number;
  isActive?: boolean;
  children?: LabTestCategoryDto[];
  tests?: LabTestDto[];
}

export interface LabTestDto extends AuditedEntityDto<string> {
  code?: string;
  name?: string;
  price?: number;
  instructions?: string | null;
  referenceRange?: string | null;
  unit?: string | null;
  categoryId?: string | null;
  categoryName?: string | null;
  isActive?: boolean;
  machine?: string | null;
  turnaroundTime?: string | null;
  normalRanges?: CreateLabTestNormalRangeDto[];
}

export interface UpdateLabAppointmentDto {
  serviceItemId?: string | null;
  appointmentDate?: string;
  preferredTime?: string | null;
  notes?: string | null;
  isFasting?: boolean;
}

export interface UpdateLabResultDto {
  result?: string;
  notes?: string;
}
