import type { CreateUpdateRoomDto, GetRoomsInput, RoomDto, RoomLookupDto } from './models';
import type { RoomType } from './room-type.enum';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class RoomService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateUpdateRoomDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RoomDto>({
      method: 'POST',
      url: '/api/app/room',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/room/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RoomDto>({
      method: 'GET',
      url: `/api/app/room/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getAvailableRooms = (type?: RoomType, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RoomLookupDto[]>({
      method: 'GET',
      url: '/api/app/room/available-rooms',
      params: { type },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetRoomsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<RoomDto>>({
      method: 'GET',
      url: '/api/app/room',
      params: { searchText: input.searchText, type: input.type, status: input.status, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: CreateUpdateRoomDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RoomDto>({
      method: 'PUT',
      url: `/api/app/room/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}