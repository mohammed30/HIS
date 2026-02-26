import { mapEnumToOptions } from '@abp/ng.core';

export enum FluidType {
  Input = 0,
  Output = 1,
}

export const fluidTypeOptions = mapEnumToOptions(FluidType);
