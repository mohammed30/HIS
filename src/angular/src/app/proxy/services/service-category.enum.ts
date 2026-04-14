import { mapEnumToOptions } from '@abp/ng.core';

export enum ServiceCategory {
  Consultation = 0,
  Procedure = 1,
  LabTest = 2,
  Radiology = 3,
  Surgery = 4,
  Other = 5,
  Pharmacy = 6,
  Consumable = 7,
  Inpatient = 8,
}

export const serviceCategoryOptions = mapEnumToOptions(ServiceCategory);
