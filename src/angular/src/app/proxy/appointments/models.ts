import type { AuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdateDoctorScheduleDto {
  doctorId: string;
  dayOfWeek: any;
  startTime: string;
  endTime: string;
  slotDuration?: number;
  isActive?: boolean;
}

export interface DoctorScheduleDto extends AuditedEntityDto<string> {
  doctorId?: string;
  doctorName?: string;
  dayOfWeek?: any;
  startTime?: string;
  endTime?: string;
  slotDuration?: number;
  isActive?: boolean;
}
