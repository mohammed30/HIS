import { mapEnumToOptions } from '@abp/ng.core';

export enum PatientCategory {
  Regular = 0,
  VIP = 1,
  Employee = 2,
  Retired = 3,
}

export const patientCategoryOptions = mapEnumToOptions(PatientCategory);
