import { mapEnumToOptions } from '@abp/ng.core';

export enum PaymentStatus {
  Pending = 0,
  Completed = 1,
  Rejected = 2,
  Refunded = 3,
  Cancelled = 4,
}

export const paymentStatusOptions = mapEnumToOptions(PaymentStatus);
