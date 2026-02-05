import { mapEnumToOptions } from '@abp/ng.core';

export enum PaymentMethod {
  Cash = 0,
  CreditCard = 1,
  DebitCard = 2,
  BankTransfer = 3,
  Check = 4,
  Insurance = 5,
  Other = 99,
}

export const paymentMethodOptions = mapEnumToOptions(PaymentMethod);
