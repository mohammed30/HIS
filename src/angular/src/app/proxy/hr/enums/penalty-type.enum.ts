import { mapEnumToOptions } from '@abp/ng.core';

export enum PenaltyType {
  Warning = 1,
  SalaryDeduction = 2,
  Suspension = 3,
  Termination = 4,
}

export const penaltyTypeOptions = mapEnumToOptions(PenaltyType);
