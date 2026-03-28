import { mapEnumToOptions } from '@abp/ng.core';

export enum InventoryCountStatus {
  Draft = 0,
  Completed = 1,
  Canceled = 2,
}

export const inventoryCountStatusOptions = mapEnumToOptions(InventoryCountStatus);
