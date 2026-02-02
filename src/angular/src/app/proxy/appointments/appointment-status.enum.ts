import { mapEnumToOptions } from '@abp/ng.core';

export enum AppointmentStatus {
  Scheduled = 0,
  Confirmed = 1,
  Cancelled = 2,
  Completed = 3,
  NoShow = 4,
  CheckedIn = 5,
  InConsultation = 6,
}

export const appointmentStatusOptions = mapEnumToOptions(AppointmentStatus);
