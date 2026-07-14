import type { EntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateNotificationDto {
  title: string;
  message: string;
  type?: string;
  url?: string | null;
  entityId?: string | null;
}

export interface GetNotificationsInput extends PagedAndSortedResultRequestDto {
  isRead?: boolean | null;
  type?: string | null;
}

export interface ModuleSubscriptionDto {
  moduleName?: string;
  displayName?: string;
  subscribedUserIds?: string[];
}

export interface NotificationDto extends EntityDto<string> {
  userId?: string;
  title?: string;
  message?: string;
  type?: string;
  url?: string | null;
  entityId?: string | null;
  isRead?: boolean;
  createdAt?: string;
  sentBy?: string | null;
}

export interface SetUserSilenceDto {
  isSilenced?: boolean;
  silencedUntil?: string | null;
}

export interface UpdateModuleSubscriptionDto {
  moduleName?: string;
  subscribedUserIds?: string[];
}

export interface UpdateNotificationSettingsDto {
  enabledTypes?: string[];
}

export interface UserNotificationSettingsDto {
  isEnabled?: boolean;
  globalSilence?: boolean;
  silencedUntil?: string | null;
  enabledTypes?: string[];
}

export interface UserNotificationSummaryDto {
  userId?: string;
  userName?: string;
  email?: string | null;
  isEnabled?: boolean;
  globalSilence?: boolean;
  silencedUntil?: string | null;
  unreadCount?: number;
  totalCount?: number;
}
