import { mapEnumToOptions } from '@abp/ng.core';

export enum NoteType {
  Progress = 0,
  Consultation = 1,
  Discharge = 2,
  Referral = 3,
  Other = 99,
}

export const noteTypeOptions = mapEnumToOptions(NoteType);
