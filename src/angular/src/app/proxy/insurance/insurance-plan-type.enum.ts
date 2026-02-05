import { mapEnumToOptions } from '@abp/ng.core';

export enum InsurancePlanType {
  Individual = 0,
  Family = 1,
  Corporate = 2,
  Government = 3,
}

export const insurancePlanTypeOptions = mapEnumToOptions(InsurancePlanType);
