import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { AdmissionStatus } from './admission-status.enum';
import type { ReservationStatus } from './reservation-status.enum';

export interface AdmissionDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string | null;
  patientFileNumber?: string | null;
  roomId?: string;
  roomNumber?: string | null;
  roomTypeName?: string | null;
  bedId?: string | null;
  bedNumber?: string | null;
  admissionDate?: string;
  dischargeDate?: string | null;
  numberOfDays?: number;
  insuranceCeiling?: number;
  companionName?: string | null;
  companionPhone?: string | null;
  companionAddress?: string | null;
  purpose?: string | null;
  pharmacyPercentage?: number;
  isServicesStopped?: boolean;
  status?: AdmissionStatus;
  totalAmount?: number;
  paidAmount?: number;
  insuranceAmount?: number;
  dueAmount?: number;
  notes?: string | null;
  invoiceId?: string | null;
  patientInsuranceId?: string | null;
}

export interface AdmissionLookupDto {
  id?: string;
  displayName?: string;
}

export interface CreatePatientTransferDto {
  toRoomId?: string;
  toBedId?: string | null;
  reason?: string | null;
}

export interface CreateUpdateAdmissionDto {
  patientId?: string;
  roomId?: string;
  bedId?: string;
  insuranceCeiling?: number;
  companionName?: string | null;
  companionPhone?: string | null;
  companionAddress?: string | null;
  purpose?: string | null;
  numberOfDays?: number;
  paidAmount?: number;
  pharmacyPercentage?: number;
  isServicesStopped?: boolean;
  notes?: string | null;
  patientInsuranceId?: string | null;
}

export interface CreateUpdateReservationDto {
  patientId?: string;
  roomId?: string;
  bedId?: string | null;
  startDate?: string;
  endDate?: string;
  status?: ReservationStatus;
  notes?: string | null;
}

export interface DischargeAdmissionDto {
  dischargeDate?: string;
  notes?: string | null;
}

export interface GetAdmissionsInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  patientId?: string | null;
  status?: AdmissionStatus | null;
  roomId?: string | null;
  roomTypeId?: number | null;
  fromDate?: string | null;
  toDate?: string | null;
}

export interface GetReservationsInput extends PagedAndSortedResultRequestDto {
  patientId?: string | null;
  roomId?: string | null;
  fromDate?: string | null;
  toDate?: string | null;
  status?: ReservationStatus | null;
}

export interface PatientAdmissionStatusDto {
  isAdmitted?: boolean;
  admissionId?: string | null;
  isServicesStopped?: boolean;
  pharmacyPercentage?: number;
  insuranceCeiling?: number;
  paidAmount?: number;
  totalAmount?: number;
  availableBalance?: number;
}

export interface ReservationDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string | null;
  roomId?: string;
  roomNumber?: string | null;
  bedId?: string | null;
  bedNumber?: string | null;
  startDate?: string;
  endDate?: string;
  status?: ReservationStatus;
  notes?: string | null;
}
