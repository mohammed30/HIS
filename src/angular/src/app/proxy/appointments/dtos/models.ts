import type { AuditedEntityDto } from '@abp/ng.core';
import type { AppointmentStatus } from '../appointment-status.enum';
import type { AppointmentType } from '../appointment-type.enum';

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
  notes?: string;
}

export interface CreateAppointmentDto {
  doctorId: string;
  clinicId: string;
  appointmentDate: string;
  type: AppointmentType;
  notes?: string;
}

export interface LookupDto<TKey> {
  id?: TKey;
  name?: string;
}
