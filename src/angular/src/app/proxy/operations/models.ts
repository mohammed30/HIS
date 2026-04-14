import type { OperationStatus } from './operation-status.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdateSurgicalOperationDto {
  patientId?: string;
  doctorId?: string | null;
  operationTypeId?: string | null;
  operationName?: string;
  operationDate?: string;
  details?: string | null;
  totalAmount?: number;
  companyShare?: number;
  patientShare?: number;
  status?: OperationStatus;
  admissionId?: string | null;
  surgeonFeePercentage?: number;
  surgeonFeeAmount?: number;
  anesthesiologistId?: string | null;
  anesthesiologistFeePercentage?: number;
  anesthesiologistFeeAmount?: number;
  hospitalShareAmount?: number;
  notes?: string | null;
}

export interface GetSurgicalOperationsInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  patientId?: string | null;
  doctorId?: string | null;
  status?: OperationStatus | null;
  specialtyId?: string | null;
  fromDate?: string | null;
  toDate?: string | null;
}

export interface SurgicalOperationDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string | null;
  doctorId?: string | null;
  doctorName?: string | null;
  operationTypeId?: string | null;
  operationName?: string;
  specialtyName?: string | null;
  operationDate?: string;
  details?: string | null;
  totalAmount?: number;
  companyShare?: number;
  patientShare?: number;
  insuranceTotal?: number;
  status?: OperationStatus;
  invoiceId?: string | null;
  admissionId?: string | null;
  surgeonFeePercentage?: number;
  surgeonFeeAmount?: number;
  anesthesiologistId?: string | null;
  anesthesiologistFeePercentage?: number;
  anesthesiologistFeeAmount?: number;
  hospitalShareAmount?: number;
  notes?: string | null;
}
