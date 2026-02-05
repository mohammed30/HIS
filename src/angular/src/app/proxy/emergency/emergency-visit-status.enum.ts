import { mapEnumToOptions } from '@abp/ng.core';

export enum EmergencyVisitStatus {
  Triaged = 0,
  TreatmentInProgress = 1,
  Admitted = 2,
  Discharged = 3,
  Transferred = 4,
  Deceased = 5,
}

export const emergencyVisitStatusOptions = mapEnumToOptions(EmergencyVisitStatus);
