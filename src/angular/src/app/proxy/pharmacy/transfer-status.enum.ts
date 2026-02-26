import { mapEnumToOptions } from '@abp/ng.core';

export enum TransferStatus {
  Draft = 0,
  PendingApproval = 1,
  Approved = 2,
  Shipped = 3,
  Received = 4,
  Cancelled = 5,
}

export const transferStatusOptions = mapEnumToOptions(TransferStatus);
