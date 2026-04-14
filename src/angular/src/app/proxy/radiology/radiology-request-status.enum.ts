import { mapEnumToOptions } from '@abp/ng.core';

export enum RadiologyRequestStatus {
  Requested = 0,
  UnderProcedure = 1,
  Reported = 2,
  Cancelled = 3,
}

export const radiologyRequestStatusOptions = mapEnumToOptions(RadiologyRequestStatus);
