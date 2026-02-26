import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { BedType } from './bed-type.enum';
import type { BedStatus } from './bed-status.enum';
import type { RoomType } from './room-type.enum';
import type { RoomStatus } from './room-status.enum';

export interface BedDto extends FullAuditedEntityDto<string> {
  roomId?: string;
  bedNumber?: string;
  type?: BedType;
  status?: BedStatus;
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
  amenities?: string;
}

export interface GetRoomsInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  type?: RoomType;
  status?: RoomStatus;
}

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
  amenities?: string;
  beds?: BedDto[];
}

export interface RoomLookupDto {
  id?: string;
  roomNumber?: string;
  name?: string;
  type?: RoomType;
  availableBeds?: number;
  dailyRate?: number;
}
