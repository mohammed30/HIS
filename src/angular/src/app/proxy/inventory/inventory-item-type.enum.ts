import { mapEnumToOptions } from '@abp/ng.core';

export enum InventoryItemType {
  Medication = 0,
  Consumable = 1,
  Asset = 2,
  Reagent = 3,
  Other = 4,
}

export const inventoryItemTypeOptions = mapEnumToOptions(InventoryItemType);
