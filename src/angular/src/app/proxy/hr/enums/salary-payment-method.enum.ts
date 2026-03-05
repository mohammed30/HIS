import { mapEnumToOptions } from '@abp/ng.core';

export enum SalaryPaymentMethod {
  Cash = 1,
  BankTransfer = 2,
  Check = 3,
}

export const salaryPaymentMethodOptions = mapEnumToOptions(SalaryPaymentMethod);
