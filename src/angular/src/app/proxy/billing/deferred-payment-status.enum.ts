import { mapEnumToOptions } from '@abp/ng.core';

export enum DeferredPaymentStatus {
  Active = 0,
  Settled = 1,
  Overdue = 2,
  Suspended = 3,
  Cancelled = 4,
}

export const deferredPaymentStatusOptions = mapEnumToOptions(DeferredPaymentStatus);
