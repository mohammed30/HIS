import type { AdmissionStatus } from './admission-status.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface AdmissionDto extends FullAuditedEntityDto<string> {
    patientId?: string;
    patientName?: string;
    patientFileNumber?: string;
    roomId?: string;
    roomNumber?: string;
    roomTypeName?: string;
    admissionDate?: string;
    dischargeDate?: string;
    numberOfDays?: number;
    insuranceCeiling?: number;
    companionName?: string;
    companionPhone?: string;
    companionAddress?: string;
    purpose?: string;
    pharmacyPercentage?: number;
    isServicesStopped?: boolean;
    status?: AdmissionStatus;
    totalAmount?: number;
    paidAmount?: number;
    insuranceAmount?: number;
    dueAmount?: number;
    notes?: string;
    invoiceId?: string;
}

export interface CreateUpdateAdmissionDto {
    patientId?: string;
    roomId?: string;
    insuranceCeiling?: number;
    companionName?: string;
    companionPhone?: string;
    companionAddress?: string;
    purpose?: string;
    pharmacyPercentage?: number;
    isServicesStopped?: boolean;
    notes?: string;
}

export interface DischargeAdmissionDto {
    dischargeDate?: string;
    notes?: string;
}

export interface GetAdmissionsInput extends PagedAndSortedResultRequestDto {
    searchText?: string;
    patientId?: string;
    status?: AdmissionStatus;
    roomId?: string;
    fromDate?: string;
    toDate?: string;
}
