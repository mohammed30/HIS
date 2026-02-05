import { mapEnumToOptions } from '@abp/ng.core';

export enum PatientInsuranceStatus {
  Active = 0,
  Expired = 1,
  Cancelled = 2,
  Suspended = 3,
}

export const patientInsuranceStatusOptions = mapEnumToOptions(PatientInsuranceStatus);
