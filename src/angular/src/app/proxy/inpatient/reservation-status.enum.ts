import { mapEnumToOptions } from '@abp/ng.core';

export enum ReservationStatus {
    Pending = 0,
    Confirmed = 1,
    CheckIn = 2,
    Cancelled = 3,
    NoShow = 4,
}

export const reservationStatusOptions = mapEnumToOptions(ReservationStatus);
