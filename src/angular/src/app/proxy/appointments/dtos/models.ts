import type { AuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { AppointmentStatus } from '../appointment-status.enum';
import type { AppointmentType } from '../appointment-type.enum';

export interface AppointmentDto extends AuditedEntityDto<string> {
  patientId: string;
  patientName?: string;
  doctorId: string;
  doctorName?: string;
  clinicId: string;
  clinicName?: string;
  appointmentDate: string;
  status: AppointmentStatus;
  type: AppointmentType;
  isWalkIn: boolean;
  notes?: string;
}

export interface CreateAppointmentDto {
  patientId: string;
  doctorId: string;
  clinicId: string;
  appointmentDate: string;
  type: AppointmentType;
  isWalkIn: boolean;
  notes?: string;
}

export interface WaitingListDto extends AuditedEntityDto<string> {
  patientId: string;
  patientName?: string;
  doctorId?: string;
  doctorName?: string;
  departmentId: string;
  departmentName?: string;
  requestDate: string;
  priority: number;
  notes?: string;
  isResolved: boolean;
}

export interface CreateUpdateWaitingListDto {
  patientId: string;
  doctorId?: string;
  departmentId: string;
  requestDate: string;
  priority: number;
  notes?: string;
  isResolved: boolean;
}

export interface DoctorScheduleDto extends AuditedEntityDto<string> {
  doctorId: string;
  doctorName?: string;
  dayOfWeek: number;
  startTime: string; // TimeSpan usually maps to string in Proxy unless configured
  endTime: string;
  slotDuration: number;
  isActive: boolean;
}

export interface CreateUpdateDoctorScheduleDto {
  doctorId: string;
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  slotDuration: number;
  isActive: boolean;
}

export interface LookupDto<TKey> {
  id?: TKey;
  name?: string;
}
