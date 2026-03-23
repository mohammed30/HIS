import { mapEnumToOptions } from '@abp/ng.core';

export enum InternalRequestStatus {
  Draft = 0,
  Submitted = 1,
  Approved = 2,
  Received = 3,
  Rejected = 4,
}

export const internalRequestStatusOptions = mapEnumToOptions(InternalRequestStatus);
