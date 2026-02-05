import { mapEnumToOptions } from '@abp/ng.core';

export enum AllergenType {
  Drug = 0,
  Food = 1,
  Environmental = 2,
  Other = 99,
}

export const allergenTypeOptions = mapEnumToOptions(AllergenType);
