import { mapEnumToOptions } from '@abp/ng.core';

export enum ActivityAction {
  Login = 0,
  Logout = 1,
  Create = 2,
  Update = 3,
  Delete = 4,
  View = 5,
  Export = 6,
  Import = 7,
  AccessDenied = 8,
  FailedLogin = 9,
}

export const activityActionOptions = mapEnumToOptions(ActivityAction);
