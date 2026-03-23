import { mapEnumToOptions } from '@abp/ng.core';

export enum InsurancePlanClass {
  ClassA = 0,
  ClassB = 1,
  ClassC = 2,
}

export const insurancePlanClassOptions = mapEnumToOptions(InsurancePlanClass);
