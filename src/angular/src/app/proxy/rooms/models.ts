import type { RoomType } from './room-type.enum';
import type { RoomStatus } from './room-status.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface RoomDto extends FullAuditedEntityDto<string> {
    roomNumber?: string;
    name?: string;
    type?: RoomType;
    bedCount?: number;
    availableBeds?: number;
    dailyRate?: number;
    floor?: string;
    status?: RoomStatus;
    notes?: string;
}

export interface CreateUpdateRoomDto {
    roomNumber?: string;
    name?: string;
    type?: RoomType;
    bedCount?: number;
    dailyRate?: number;
    floor?: string;
    status?: RoomStatus;
    notes?: string;
}

export interface GetRoomsInput extends PagedAndSortedResultRequestDto {
    searchText?: string;
    type?: RoomType;
    status?: RoomStatus;
}

export interface RoomLookupDto {
    id?: string;
    roomNumber?: string;
    name?: string;
    type?: RoomType;
    availableBeds?: number;
    dailyRate?: number;
}
