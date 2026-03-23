import { mapEnumToOptions } from '@abp/ng.core';

export enum PurchaseInvoiceStatus {
  Draft = 0,
  Posted = 1,
  Cancelled = 2,
}

export const purchaseInvoiceStatusOptions = mapEnumToOptions(PurchaseInvoiceStatus);
