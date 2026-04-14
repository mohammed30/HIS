import type { OrderType } from './order-type.enum';
import type { AuditedEntityDto } from '@abp/ng.core';
import type { OrderStatus } from './order-status.enum';

export interface CreateUpdateMedicalOrderDto {
  patientId: string;
  admissionId?: string | null;
  serviceItemId: string;
  type?: OrderType;
  clinicalNotes?: string;
  quantity?: number;
  details?: string;
}

export interface MedicalOrderDto extends AuditedEntityDto<string> {
  patientId?: string;
  admissionId?: string | null;
  doctorId?: string | null;
  type?: OrderType;
  status?: OrderStatus;
  serviceItemId?: string;
  serviceName?: string;
  price?: number;
  clinicalNotes?: string;
  details?: string;
}
