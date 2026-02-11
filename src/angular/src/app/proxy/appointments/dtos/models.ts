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
  notes?: string;
}

export interface CreateAppointmentDto {
  patientId?: string;
  doctorId?: string;
  clinicId?: string;
  appointmentDate?: string;
  type?: AppointmentType;
  isWalkIn?: boolean;
  notes?: string;
}

export interface BookClinicAppointmentDto {
  patientId?: string;
  clinicId?: string;
  doctorId?: string;
  serviceItemId?: string;
  appointmentDate?: string;
  type?: AppointmentType;
  createInvoice?: boolean;
  paymentMethod?: string;
  paidAmount?: number;
  discount?: number;
}

export interface CreateUpdateWaitingListDto {
  patientId?: string;
  doctorId?: string;
  departmentId?: string;
  requestDate?: string;
  priority?: WaitingListPriority;
  notes?: string;
  isResolved?: boolean;
}

export interface LookupDto<TKey> {
  id?: TKey;
  name?: string;
}

export interface WaitingListDto extends AuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  doctorId?: string;
  doctorName?: string;
  departmentId?: string;
  departmentName?: string;
  requestDate?: string;
  priority?: WaitingListPriority;
  notes?: string;
  isResolved?: boolean;
}
