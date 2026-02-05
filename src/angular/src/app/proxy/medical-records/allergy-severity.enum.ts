import { mapEnumToOptions } from '@abp/ng.core';

export enum AllergySeverity {
  Mild = 0,
  Moderate = 1,
  Severe = 2,
  LifeThreatening = 3,
}

export const allergySeverityOptions = mapEnumToOptions(AllergySeverity);
