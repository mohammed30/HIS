import { mapEnumToOptions } from '@abp/ng.core';

export enum DepositStatus {
  Active = 0,
  Consumed = 1,
  Refunded = 2,
}

export const depositStatusOptions = mapEnumToOptions(DepositStatus);
