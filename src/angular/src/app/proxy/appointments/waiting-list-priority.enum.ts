import { mapEnumToOptions } from '@abp/ng.core';

export enum WaitingListPriority {
  Normal = 0,
  High = 1,
  Urgent = 2,
}

export const waitingListPriorityOptions = mapEnumToOptions(WaitingListPriority);
