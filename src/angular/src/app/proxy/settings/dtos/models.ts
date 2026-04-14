import type { AuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdateJobTitleDto {
  nameAr: string;
  nameEn?: string;
  description?: string;
  departmentId?: string | null;
}

export interface JobTitleDto extends AuditedEntityDto<string> {
  nameAr?: string;
  nameEn?: string;
  description?: string;
  departmentId?: string | null;
  departmentName?: string;
}
