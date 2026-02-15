import { mapEnumToOptions } from '@abp/ng.core';

export enum BedStatus {
    Available = 0,
    Occupied = 1,
    Reserved = 2,
    Cleaning = 3,
    Maintenance = 4,
}

export const bedStatusOptions = mapEnumToOptions(BedStatus);
