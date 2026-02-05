import { mapEnumToOptions } from '@abp/ng.core';

export enum AppointmentType {
  NewVisit = 0,
  FollowUp = 1,
  Emergency = 2,
  Telemedicine = 3,
  Procedure = 4,
}

export const appointmentTypeOptions = mapEnumToOptions(AppointmentType);
