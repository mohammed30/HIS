import { mapEnumToOptions } from '@abp/ng.core';

export enum ActivityLogLevel {
  Info = 0,
  Warning = 1,
  Critical = 2,
}

export const activityLogLevelOptions = mapEnumToOptions(ActivityLogLevel);
