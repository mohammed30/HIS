import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { ActivityAction } from './activity-action.enum';
import type { ActivityLogLevel } from './activity-log-level.enum';

export interface ActivityLogDto extends FullAuditedEntityDto<string> {
  userId?: string;
  userName?: string;
  module?: string;
  action?: ActivityAction;
  entityType?: string;
  entityId?: string;
  description?: string;
  oldValues?: string;
  newValues?: string;
  ipAddress?: string;
  userAgent?: string;
  timestamp?: string;
  level?: ActivityLogLevel;
  additionalData?: string;
  // Device & Location Info
  deviceType?: string;
  browserName?: string;
  browserVersion?: string;
  operatingSystem?: string;
  country?: string;
  city?: string;
}

export interface GetActivityLogsInput extends PagedAndSortedResultRequestDto {
  userId?: string;
  module?: string;
  activityActionFilter?: ActivityAction;
  level?: ActivityLogLevel;
  entityType?: string;
  entityId?: string;
  startDate?: string;
  endDate?: string;
  searchText?: string;
}
