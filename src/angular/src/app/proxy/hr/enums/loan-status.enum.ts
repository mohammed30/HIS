import { mapEnumToOptions } from '@abp/ng.core';

export enum LoanStatus {
  Active = 1,
  PaidOff = 2,
  Cancelled = 3,
}

export const loanStatusOptions = mapEnumToOptions(LoanStatus);
