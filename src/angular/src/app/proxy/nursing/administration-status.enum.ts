import { mapEnumToOptions } from '@abp/ng.core';

export enum AdministrationStatus {
  Given = 0,
  Refused = 1,
  Skipped = 2,
  Late = 3,
}

export const administrationStatusOptions = mapEnumToOptions(AdministrationStatus);
