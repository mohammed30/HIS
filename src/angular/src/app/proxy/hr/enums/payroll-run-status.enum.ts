import { mapEnumToOptions } from '@abp/ng.core';

export enum PayrollRunStatus {
  Draft = 1,
  Processed = 2,
  Posted = 3,
}

export const payrollRunStatusOptions = mapEnumToOptions(PayrollRunStatus);
