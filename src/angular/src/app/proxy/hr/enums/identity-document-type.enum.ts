import { mapEnumToOptions } from '@abp/ng.core';

export enum IdentityDocumentType {
  NationalId = 1,
  Passport = 2,
  ResidencyPermit = 3,
  DrivingLicense = 4,
  NationalityCard = 5,
}

export const identityDocumentTypeOptions = mapEnumToOptions(IdentityDocumentType);
