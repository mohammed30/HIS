import { mapEnumToOptions } from '@abp/ng.core';

export enum IdentityType {
  NationalId = 0,
  Passport = 1,
  ResidencePermit = 2,
  DrivingLicense = 3,
}

export const identityTypeOptions = mapEnumToOptions(IdentityType);
