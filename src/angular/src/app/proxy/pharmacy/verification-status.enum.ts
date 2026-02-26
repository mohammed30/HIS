import { mapEnumToOptions } from '@abp/ng.core';

export enum VerificationStatus {
  Pending = 0,
  Verified = 1,
  Rejected = 2,
  RequiresClarification = 3,
}

export const verificationStatusOptions = mapEnumToOptions(VerificationStatus);
