import type { AdmissionStatus } from './admission-status.enum';
import type { ReservationStatus } from './reservation-status.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface AdmissionDto extends FullAuditedEntityDto<string> {
    patientId?: string;
    patientName?: string;
    patientFileNumber?: string;
    roomId?: string;
    roomNumber?: string;
    roomTypeName?: string;
    bedId?: string;
    bedNumber?: string;
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
    bedId?: string;
    numberOfDays?: number;
    paidAmount?: number;
    insuranceCeiling: number;
    companionName?: string;
    companionPhone?: string;
    companionAddress?: string;
    purpose?: string;
    pharmacyPercentage: number;
    isServicesStopped: boolean;
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
    roomTypeId?: number;
    fromDate?: string;
    toDate?: string;
}

export interface ReservationDto extends FullAuditedEntityDto<string> {
    patientId?: string;
    patientName?: string;
    roomId?: string;
    roomNumber?: string;
    bedId?: string;
    bedNumber?: string;
    startDate?: string;
    endDate?: string;
    status?: ReservationStatus;
    notes?: string;
}

export interface CreateUpdateReservationDto {
    patientId?: string;
    roomId?: string;
    bedId?: string;
    startDate?: string;
    endDate?: string;
    status?: ReservationStatus;
    notes?: string;
}

export interface GetReservationsInput extends PagedAndSortedResultRequestDto {
    patientId?: string;
    roomId?: string;
    fromDate?: string;
    toDate?: string;
    status?: ReservationStatus;
}
