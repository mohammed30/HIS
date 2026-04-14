import type { AuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdatePriceListDto {
  name?: string;
  isDefault?: boolean;
  effectiveFrom?: string;
  effectiveTo?: string | null;
}

export interface CreateUpdateServicePriceDto {
  priceListId?: string;
  serviceItemId?: string;
  amount?: number;
  coPayAmount?: number;
}

export interface PriceListDto extends AuditedEntityDto<string> {
  name?: string;
  isDefault?: boolean;
  effectiveFrom?: string;
  effectiveTo?: string | null;
}

export interface ServicePriceDto extends AuditedEntityDto<string> {
  priceListId?: string;
  serviceItemId?: string;
  serviceItemName?: string;
  amount?: number;
  coPayAmount?: number;
}
