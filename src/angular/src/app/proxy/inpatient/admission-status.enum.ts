import { mapEnumToOptions } from '@abp/ng.core';

export enum AdmissionStatus {
  Active = 0,
  Discharged = 1,
  Transferred = 2,
  Cancelled = 3,
}

export const admissionStatusOptions = mapEnumToOptions(AdmissionStatus);
