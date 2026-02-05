import { mapEnumToOptions } from '@abp/ng.core';

export enum ServiceType {
  Consultation = 0,
  Medication = 1,
  Laboratory = 2,
  Radiology = 3,
  Procedure = 4,
  Inpatient = 5,
  Consumables = 6,
  Other = 99,
}

export const serviceTypeOptions = mapEnumToOptions(ServiceType);
