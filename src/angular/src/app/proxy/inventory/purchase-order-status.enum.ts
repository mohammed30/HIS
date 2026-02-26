import { mapEnumToOptions } from '@abp/ng.core';

export enum PurchaseOrderStatus {
  Draft = 0,
  Confirmed = 1,
  Received = 2,
  Cancelled = 3,
}

export const purchaseOrderStatusOptions = mapEnumToOptions(PurchaseOrderStatus);
