import type { OrderType } from './order-type.enum';
import type { AuditedEntityDto } from '@abp/ng.core';
import type { OrderStatus } from './order-status.enum';

export interface CreateUpdateMedicalOrderDto {
  patientId: string;
  serviceItemId: string;
  type?: OrderType;
  clinicalNotes?: string;
  details?: string;
}

export interface MedicalOrderDto extends AuditedEntityDto<string> {
  patientId?: string;
  doctorId?: string;
  type?: OrderType;
  status?: OrderStatus;
  serviceItemId?: string;
  serviceName?: string;
  price?: number;
  clinicalNotes?: string;
  details?: string;
}
