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
      { apiName: this.apiName, ...config });


  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ActivityLogDto>({
      method: 'GET',
      url: `/api/app/activity-log/${id}`,
    },
      { apiName: this.apiName, ...config });


  getByEntity = (entityType: string, entityId: string, skipCount?: number, maxResultCount: number = 20, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ActivityLogDto>>({
      method: 'GET',
      url: `/api/app/activity-log/by-entity/${entityId}`,
      params: { entityType, skipCount, maxResultCount },
    },
      { apiName: this.apiName, ...config });


  getByUser = (userId: string, skipCount?: number, maxResultCount: number = 20, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ActivityLogDto>>({
      method: 'GET',
      url: `/api/app/activity-log/by-user/${userId}`,
      params: { skipCount, maxResultCount },
    },
      { apiName: this.apiName, ...config });


  getList = (input: GetActivityLogsInput, config?: Partial<Rest.Config>) => {
    // Filter out undefined/null values to prevent API errors
    const params: any = {};
    if (input.userId) params.userId = input.userId;
    if (input.module) params.module = input.module;
    if (input.activityActionFilter !== undefined && input.activityActionFilter !== null) params.activityActionFilter = input.activityActionFilter;
    if (input.level !== undefined && input.level !== null) params.level = input.level;
    if (input.entityType) params.entityType = input.entityType;
    if (input.entityId) params.entityId = input.entityId;
    if (input.startDate) params.startDate = input.startDate;
    if (input.endDate) params.endDate = input.endDate;
    if (input.searchText) params.searchText = input.searchText;
    if (input.sorting) params.sorting = input.sorting;
    if (input.skipCount !== undefined) params.skipCount = input.skipCount;
    if (input.maxResultCount !== undefined) params.maxResultCount = input.maxResultCount;

    return this.restService.request<any, PagedResultDto<ActivityLogDto>>({
      method: 'GET',
      url: '/api/app/activity-log',
      params,
    },
      { apiName: this.apiName, ...config });
  }


  getModules = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, string[]>({
      method: 'GET',
      url: '/api/app/activity-log/modules',
    },
      { apiName: this.apiName, ...config });
}