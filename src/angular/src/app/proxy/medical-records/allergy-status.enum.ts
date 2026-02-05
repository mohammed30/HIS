import { mapEnumToOptions } from '@abp/ng.core';

export enum AllergyStatus {
  Active = 0,
  Resolved = 1,
}

export const allergyStatusOptions = mapEnumToOptions(AllergyStatus);
