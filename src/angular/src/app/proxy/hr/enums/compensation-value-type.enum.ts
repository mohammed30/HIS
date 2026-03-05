import { mapEnumToOptions } from '@abp/ng.core';

export enum CompensationValueType {
  Fixed = 1,
  Percentage = 2,
  Equation = 3,
}

export const compensationValueTypeOptions = mapEnumToOptions(CompensationValueType);
