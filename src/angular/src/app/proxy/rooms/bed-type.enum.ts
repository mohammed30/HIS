import { mapEnumToOptions } from '@abp/ng.core';

export enum BedType {
    Standard = 0,
    Electric = 1,
    Hydraulic = 2,
    Bariatric = 3,
    Pediatric = 4,
}

export const bedTypeOptions = mapEnumToOptions(BedType);
