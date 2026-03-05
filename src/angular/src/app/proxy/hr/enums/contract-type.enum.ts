import { mapEnumToOptions } from '@abp/ng.core';

export enum ContractType {
  Permanent = 1,
  Temporary = 2,
  PartTime = 3,
  Probation = 4,
}

export const contractTypeOptions = mapEnumToOptions(ContractType);
