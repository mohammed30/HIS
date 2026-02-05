import { mapEnumToOptions } from '@abp/ng.core';

export enum EmergencySeverity {
  Resuscitation = 1,
  Emergent = 2,
  Urgent = 3,
  LessUrgent = 4,
  NonUrgent = 5,
}

export const emergencySeverityOptions = mapEnumToOptions(EmergencySeverity);
