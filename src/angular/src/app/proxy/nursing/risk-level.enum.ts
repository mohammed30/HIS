import { mapEnumToOptions } from '@abp/ng.core';

export enum RiskLevel {
  Low = 0,
  Medium = 1,
  High = 2,
}

export const riskLevelOptions = mapEnumToOptions(RiskLevel);
