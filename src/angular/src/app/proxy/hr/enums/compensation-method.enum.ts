import { mapEnumToOptions } from '@abp/ng.core';

export enum CompensationMethod {
  Credit = 1,
  Debit = 2,
}

export const compensationMethodOptions = mapEnumToOptions(CompensationMethod);
