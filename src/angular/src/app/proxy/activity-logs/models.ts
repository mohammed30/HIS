import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { ActivityAction } from './activity-action.enum';
import type { ActivityLogLevel } from './activity-log-level.enum';

export interface ActivityLogDto extends FullAuditedEntityDto<string> {
  userId?: string | null;
  userName?: string | null;
  module?: string;
  action?: ActivityAction;
  entityType?: string | null;
  entityId?: string | null;
  description?: string | null;
  oldValues?: string | null;
  newValues?: string | null;
  ipAddress?: string | null;
  userAgent?: string | null;
  timestamp?: string;
  level?: ActivityLogLevel;
  additionalData?: string | null;
  deviceType?: string | null;
  browserName?: string | null;
  browserVersion?: string | null;
  operatingSystem?: string | null;
  country?: string | null;
  city?: string | null;
}

export interface GetActivityLogsInput extends PagedAndSortedResultRequestDto {
  userId?: string | null;
  module?: string | null;
  activityActionFilter?: ActivityAction | null;
  level?: ActivityLogLevel | null;
  entityType?: string | null;
  entityId?: string | null;
  startDate?: string | null;
  endDate?: string | null;
  searchText?: string | null;
}
