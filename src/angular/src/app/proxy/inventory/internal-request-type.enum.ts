import { mapEnumToOptions } from '@abp/ng.core';

export enum InternalRequestType {
  Medication = 0,
  Consumable = 1,
  Laboratory = 2,
  Radiology = 3,
  Other = 4,
}

export const internalRequestTypeOptions = mapEnumToOptions(InternalRequestType);
