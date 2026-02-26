import { mapEnumToOptions } from '@abp/ng.core';

export enum OrderType {
  Lab = 0,
  Radiology = 1,
  Medication = 2,
  Procedure = 3,
  Consumable = 4,
}

export const orderTypeOptions = mapEnumToOptions(OrderType);
