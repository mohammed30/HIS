import { mapEnumToOptions } from '@abp/ng.core';

export enum InvoiceStatus {
  Draft = 0,
  Issued = 1,
  PartiallyPaid = 2,
  Paid = 3,
  Cancelled = 4,
  Deferred = 5,
  Refunded = 6,
}

export const invoiceStatusOptions = mapEnumToOptions(InvoiceStatus);
