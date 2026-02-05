import { mapEnumToOptions } from '@abp/ng.core';

export enum OrderStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3,
  Reported = 4,
}

export const orderStatusOptions = mapEnumToOptions(OrderStatus);
