import { mapEnumToOptions } from '@abp/ng.core';

export enum DiagnosisType {
  Primary = 0,
  Secondary = 1,
  Differential = 2,
}

export const diagnosisTypeOptions = mapEnumToOptions(DiagnosisType);
