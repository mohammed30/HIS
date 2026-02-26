import { mapEnumToOptions } from '@abp/ng.core';

export enum RoomType {
  Standard = 0,
  Private = 1,
  ICU = 2,
  Suite = 3,
  Isolation = 4,
}

export const roomTypeOptions = mapEnumToOptions(RoomType);
