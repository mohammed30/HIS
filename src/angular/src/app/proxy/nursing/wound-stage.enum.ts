import { mapEnumToOptions } from '@abp/ng.core';

export enum WoundStage {
  Stage1 = 1,
  Stage2 = 2,
  Stage3 = 3,
  Stage4 = 4,
  Unstageable = 5,
}

export const woundStageOptions = mapEnumToOptions(WoundStage);
