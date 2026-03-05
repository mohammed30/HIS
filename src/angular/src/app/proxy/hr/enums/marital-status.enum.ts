import { mapEnumToOptions } from '@abp/ng.core';

export enum MaritalStatus {
  Single = 1,
  Married = 2,
  Divorced = 3,
  Widowed = 4,
}

export const maritalStatusOptions = mapEnumToOptions(MaritalStatus);
