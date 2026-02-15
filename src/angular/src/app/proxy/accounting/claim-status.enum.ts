import { mapEnumToOptions } from '@abp/ng.core';

export enum ClaimStatus {
    Pending = 0,
    Submitted = 1,
    Approved = 2,
    Rejected = 3,
    Paid = 4,
}

export const claimStatusOptions = mapEnumToOptions(ClaimStatus);
