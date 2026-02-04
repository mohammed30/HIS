import { mapEnumToOptions } from '@abp/ng.core';

export enum LabRequestStatus {
    Requested = 0,
    SampleCollected = 1,
    InProcess = 2,
    Completed = 3,
    Cancelled = 4,
}

export const labRequestStatusOptions = mapEnumToOptions(LabRequestStatus);
