import type { AuditedEntityDto } from '@abp/ng.core';
import type { AppointmentStatus } from '../appointment-status.enum';
import type { AppointmentType } from '../appointment-type.enum';
import type { WaitingListPriority } from '../waiting-list-priority.enum';

export interface AppointmentDto extends AuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  doctorId?: string;
  doctorName?: string;
  clinicId?: string;
  clinicName?: string;
  appointmentDate?: string;
  status?: AppointmentStatus;
  type?: AppointmentType;
  isWalkIn?: boolean;
  consultationFee?: number;
  notes?: string;
}

export interface BookClinicAppointmentDto {
  patientId?: string;
  clinicId?: string;
  doctorId?: string;
  serviceItemId?: string | null;
  appointmentDate?: string;
  type?: AppointmentType;
  isWalkIn?: boolean;
  createInvoice?: boolean;
  paymentMethod?: string;
  paidAmount?: number | null;
  discount?: number | null;
  insurancePercentage?: number;
  patientInsuranceId?: string | null;
}

export interface CreateAppointmentDto {
  patientId?: string;
  doctorId?: string;
  clinicId?: string;
  appointmentDate?: string;
  type?: AppointmentType;
  isWalkIn?: boolean;
  consultationFee?: number;
  notes?: string;
}

export interface CreateUpdateWaitingListDto {
  patientId?: string;
  doctorId?: string | null;
  departmentId?: string;
  requestDate?: string;
  priority?: WaitingListPriority;
  notes?: string;
  isResolved?: boolean;
}

export interface LookupDto<TKey> {
  id?: TKey | null;
  name?: string;
}

export interface WaitingListDto extends AuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  doctorId?: string | null;
  doctorName?: string;
  departmentId?: string;
  departmentName?: string;
  requestDate?: string;
  priority?: WaitingListPriority;
  notes?: string;
  isResolved?: boolean;
}
