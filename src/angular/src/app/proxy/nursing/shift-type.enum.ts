import { mapEnumToOptions } from '@abp/ng.core';

export enum ShiftType {
  Morning = 0,
  Evening = 1,
  Night = 2,
}

export const shiftTypeOptions = mapEnumToOptions(ShiftType);
