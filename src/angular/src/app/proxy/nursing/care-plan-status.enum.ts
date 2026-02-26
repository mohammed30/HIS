import { mapEnumToOptions } from '@abp/ng.core';

export enum CarePlanStatus {
  Active = 0,
  Resolved = 1,
  Discontinued = 2,
}

export const carePlanStatusOptions = mapEnumToOptions(CarePlanStatus);
