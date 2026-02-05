import { mapEnumToOptions } from '@abp/ng.core';

export enum DiagnosisStatus {
  Active = 0,
  Resolved = 1,
  Chronic = 2,
}

export const diagnosisStatusOptions = mapEnumToOptions(DiagnosisStatus);
