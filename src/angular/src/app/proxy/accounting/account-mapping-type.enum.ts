import { mapEnumToOptions } from '@abp/ng.core';

export enum AccountMappingType {
  SalesRevenue = 0,
  CashAccount = 1,
  VATOutput = 2,
  VATInput = 3,
  Inventory = 4,
  COGS = 5,
  PatientsReceivable = 6,
  InsuranceReceivable = 7,
  InsuranceDiscounts = 8,
  InventoryAdjustment = 9,
  AccruedInventory = 10,
  CardPaymentBank = 11,
  PatientDeposits = 12,
  Purchases = 13,
}

export const accountMappingTypeOptions = mapEnumToOptions(AccountMappingType);
