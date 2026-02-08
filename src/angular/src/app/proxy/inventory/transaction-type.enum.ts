import { mapEnumToOptions } from '@abp/ng.core';

export enum TransactionType {
  Receipt = 0,
  Issue = 1,
  Transfer = 2,
  Adjustment = 3,
  Dispensing = 4,
}

export const transactionTypeOptions = mapEnumToOptions(TransactionType);
