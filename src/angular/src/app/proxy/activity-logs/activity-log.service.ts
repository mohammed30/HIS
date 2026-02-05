import type { ActivityLogDto, GetActivityLogsInput } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ActivityLogService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  exportToCsv = (input: GetActivityLogsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, number[]>({
      method: 'POST',
      url: '/api/app/activity-log/export-to-csv',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ActivityLogDto>({
      method: 'GET',
      url: `/api/app/activity-log/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByEntity = (entityType: string, entityId: string, skipCount?: number, maxResultCount: number = 20, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ActivityLogDto>>({
      method: 'GET',
      url: `/api/app/activity-log/by-entity/${entityId}`,
      params: { entityType, skipCount, maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getByUser = (userId: string, skipCount?: number, maxResultCount: number = 20, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ActivityLogDto>>({
      method: 'GET',
      url: `/api/app/activity-log/by-user/${userId}`,
      params: { skipCount, maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getList = (input: GetActivityLogsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ActivityLogDto>>({
      method: 'GET',
      url: '/api/app/activity-log',
      params: { userId: input.userId, module: input.module, activityActionFilter: input.activityActionFilter, level: input.level, entityType: input.entityType, entityId: input.entityId, startDate: input.startDate, endDate: input.endDate, searchText: input.searchText, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getModules = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, string[]>({
      method: 'GET',
      url: '/api/app/activity-log/modules',
    },
    { apiName: this.apiName,...config });
}