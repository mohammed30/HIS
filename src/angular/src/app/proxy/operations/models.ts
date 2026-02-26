import type { OperationStatus } from './operation-status.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdateSurgicalOperationDto {
  patientId?: string;
  doctorId?: string;
  operationTypeId?: string;
  operationName?: string;
  operationDate?: string;
  details?: string;
  totalAmount?: number;
  companyShare?: number;
  patientShare?: number;
  status?: OperationStatus;
  admissionId?: string;
  surgeonFeePercentage?: number;
  surgeonFeeAmount?: number;
  anesthesiologistId?: string;
  anesthesiologistFeePercentage?: number;
  anesthesiologistFeeAmount?: number;
  hospitalShareAmount?: number;
  notes?: string;
}

export interface GetSurgicalOperationsInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  patientId?: string;
  doctorId?: string;
  status?: OperationStatus;
  specialtyId?: string;
  fromDate?: string;
  toDate?: string;
}

export interface SurgicalOperationDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  doctorId?: string;
  doctorName?: string;
  operationTypeId?: string;
  operationName?: string;
  specialtyName?: string;
  operationDate?: string;
  details?: string;
  totalAmount?: number;
  companyShare?: number;
  patientShare?: number;
  insuranceTotal?: number;
  status?: OperationStatus;
  invoiceId?: string;
  admissionId?: string;
  surgeonFeePercentage?: number;
  surgeonFeeAmount?: number;
  anesthesiologistId?: string;
  anesthesiologistFeePercentage?: number;
  anesthesiologistFeeAmount?: number;
  hospitalShareAmount?: number;
  notes?: string;
}
