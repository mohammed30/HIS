import { mapEnumToOptions } from '@abp/ng.core';

export enum OperationStatus {
  Scheduled = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3,
}

export const operationStatusOptions = mapEnumToOptions(OperationStatus);
