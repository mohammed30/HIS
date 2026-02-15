import { mapEnumToOptions } from '@abp/ng.core';

export enum RoomStatus {
    Available = 0,
    Occupied = 1,
    Reserved = 2,
    Maintenance = 3,
    OutOfService = 4,
}

export const roomStatusOptions = mapEnumToOptions(RoomStatus);
