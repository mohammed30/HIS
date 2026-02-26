import { mapEnumToOptions } from '@abp/ng.core';

export enum FluidMetric {
  Oral = 0,
  IV = 1,
  TubeFeeding = 2,
  Urine = 10,
  Stool = 11,
  Vomit = 12,
  Drain = 13,
  Sweat = 14,
}

export const fluidMetricOptions = mapEnumToOptions(FluidMetric);
