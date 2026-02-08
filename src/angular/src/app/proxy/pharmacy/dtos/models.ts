import type { AuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdateDrugDto {
  barcode: string;
  brandName: string;
  scientificName: string;
  strength?: string;
  form?: string;
  manufacturer?: string;
  batchNumberPrefix?: string;
  price?: number;
}

export interface DrugDto extends AuditedEntityDto<string> {
  barcode?: string;
  brandName?: string;
  scientificName?: string;
  strength?: string;
  form?: string;
  manufacturer?: string;
  batchNumberPrefix?: string;
  serviceItemId?: string;
  serviceItemName?: string;
}
