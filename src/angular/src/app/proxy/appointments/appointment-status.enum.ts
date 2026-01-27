import { mapEnumToOptions } from '@abp/ng.core';

export enum AppointmentStatus {
  Pending = 0,
  Confirmed = 1,
  Cancelled = 2,
  Completed = 3,
  NoShow = 4,
}

export const appointmentStatusOptions = mapEnumToOptions(AppointmentStatus);
