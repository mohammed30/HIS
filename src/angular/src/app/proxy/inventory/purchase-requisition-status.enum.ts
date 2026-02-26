import { mapEnumToOptions } from '@abp/ng.core';

export enum PurchaseRequisitionStatus {
  Draft = 0,
  Pending = 1,
  Approved = 2,
  Rejected = 3,
  ConvertedToPO = 4,
}

export const purchaseRequisitionStatusOptions = mapEnumToOptions(PurchaseRequisitionStatus);
