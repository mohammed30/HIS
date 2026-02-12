import type { EntityDto } from '@abp/ng.core';

export interface ContractDto extends EntityDto<string> {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
}

export interface CreateUpdateContractDto {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
}

export interface CreateUpdateNationalityDto {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
}

export interface CreateUpdatePatientCategoryDto {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
}

export interface CreateUpdateProfessionDto {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
}

export interface CreateUpdateReferralSourceDto {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
}

export interface NationalityDto extends EntityDto<string> {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
}

export interface PatientCategoryDto extends EntityDto<string> {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
}

export interface ProfessionDto extends EntityDto<string> {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
}

export interface ReferralSourceDto extends EntityDto<string> {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
}

export interface PaymentMethodDto extends EntityDto<string> {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
  isDefault?: boolean;
}

export interface CreateUpdatePaymentMethodDto {
  nameAr?: string;
  nameEn?: string;
  code?: string;
  isActive?: boolean;
  isDefault?: boolean;
}
