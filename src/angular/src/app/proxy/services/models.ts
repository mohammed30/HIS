import type { ServiceCategory } from './service-category.enum';
import type { AuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdateRadiologyItemDto extends CreateUpdateServiceItemDto {
  modality?: string;
  bodyPart?: string;
  instructions?: string;
}

export interface CreateUpdateServiceItemDto {
  code?: string;
  name?: string;
  category?: ServiceCategory;
  departmentId?: string;
  isActive?: boolean;
  price?: number;
  unit?: string;
  referenceRange?: string;
  instructions?: string;
}

export interface RadiologyItemDto extends ServiceItemDto {
  modality?: string;
  bodyPart?: string;
  instructions?: string;
}

export interface ServiceItemDto extends AuditedEntityDto<string> {
  code?: string;
  name?: string;
  category?: ServiceCategory;
  departmentId?: string;
  isActive?: boolean;
  price?: number;
  unit?: string;
  referenceRange?: string;
  instructions?: string;
}
