import { mapEnumToOptions } from '@abp/ng.core';

export enum AppointmentType {
  FirstVisit = 0,
  FollowUp = 1,
  Emergency = 2,
  Consultation = 3,
}

export const appointmentTypeOptions = mapEnumToOptions(AppointmentType);
