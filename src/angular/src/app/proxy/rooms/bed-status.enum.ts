import { mapEnumToOptions } from '@abp/ng.core';

export enum BedStatus {
  Available = 0,
  Occupied = 1,
  Reserved = 2,
  Maintenance = 3,
  Cleaning = 4,
  OutOfService = 5,
}

export const bedStatusOptions = mapEnumToOptions(BedStatus);
