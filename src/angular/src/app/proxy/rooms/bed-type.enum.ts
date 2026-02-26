import { mapEnumToOptions } from '@abp/ng.core';

export enum BedType {
  Standard = 0,
  Electric = 1,
  ICU = 2,
  Incubator = 3,
  Labor = 4,
  Emergency = 5,
  DialysisChair = 6,
}

export const bedTypeOptions = mapEnumToOptions(BedType);
