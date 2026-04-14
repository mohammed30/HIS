import type { FullAuditedEntityDto } from '@abp/ng.core';

export interface CreateUpdatePaymentMethodDto {
  nameAr: string;
  nameEn: string;
  code?: string | null;
  isActive?: boolean;
  isDefault?: boolean;
}

export interface PaymentMethodDto extends FullAuditedEntityDto<string> {
  nameAr?: string;
  nameEn?: string;
  code?: string | null;
  isActive?: boolean;
  isDefault?: boolean;
}
