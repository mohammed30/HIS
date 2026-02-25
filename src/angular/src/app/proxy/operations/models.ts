import type { OperationStatus } from './operation-status.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface SurgicalOperationDto extends FullAuditedEntityDto<string> {
    patientId?: string;
    patientName?: string;
    doctorId?: string;
    doctorName?: string;
    specialtyName?: string;
    operationTypeId?: string;
    operationName?: string;
    operationDate?: string;
    details?: string;
    totalAmount?: number;
    companyShare?: number;
    patientShare?: number;
    insuranceTotal?: number;
    status?: OperationStatus;
    invoiceId?: string;
    admissionId?: string;
    notes?: string;
}

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
    notes?: string;
}

export interface GetSurgicalOperationsInput extends PagedAndSortedResultRequestDto {
    searchText?: string;
    patientId?: string;
    doctorId?: string;
    specialtyId?: string;
    status?: OperationStatus;
    fromDate?: string;
    toDate?: string;
}
