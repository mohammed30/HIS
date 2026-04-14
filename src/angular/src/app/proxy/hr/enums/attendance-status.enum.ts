import { mapEnumToOptions } from '@abp/ng.core';

export enum AttendanceStatus {
  Present = 0,
  Absent = 1,
  Late = 2,
  EarlyLeave = 3,
  OnLeave = 4,
}

export const attendanceStatusOptions = mapEnumToOptions(AttendanceStatus);
