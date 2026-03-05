import { mapEnumToOptions } from '@abp/ng.core';

export enum CompensationNature {
  Allowance = 1,
  Deduction = 2,
}

export const compensationNatureOptions = mapEnumToOptions(CompensationNature);
