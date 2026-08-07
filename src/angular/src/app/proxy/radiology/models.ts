import type { RadiologyRequestStatus } from './radiology-request-status.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdateRadiologyRequestDto {
  patientId?: string;
  doctorId?: string | null;
  isExternalDoctor?: boolean;
  externalDoctorName?: string | null;
  radiologyItemId?: string;
  reportBody?: string;
  technicianNotes?: string;
  status?: RadiologyRequestStatus;
}

export interface GetRadiologyRequestInput extends PagedAndSortedResultRequestDto {
  filter?: string | null;
  status?: RadiologyRequestStatus | null;
}

export interface RadiologyRequestDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  doctorId?: string | null;
  doctorName?: string | null;
  isExternalDoctor?: boolean;
  externalDoctorName?: string | null;
  radiologyItemId?: string;
  radiologyItemName?: string;
  requestDate?: string;
  status?: RadiologyRequestStatus;
  reportBody?: string;
  technicianNotes?: string;
  reportDate?: string | null;
  requestNumber?: string;
  requestingDepartmentName?: string;
  admissionRoom?: string;
}
