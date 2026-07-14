import type { CreateNotificationDto, GetNotificationsInput, ModuleSubscriptionDto, NotificationDto, SetUserSilenceDto, UpdateModuleSubscriptionDto, UpdateNotificationSettingsDto, UserNotificationSettingsDto, UserNotificationSummaryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { LookupDto } from '../appointments/dtos/models';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/notification/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getModuleSubscriptions = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ModuleSubscriptionDto[]>({
      method: 'GET',
      url: '/api/app/notification/module-subscriptions',
    },
    { apiName: this.apiName,...config });
  

  getMyNotifications = (input: GetNotificationsInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<NotificationDto>>({
      method: 'GET',
      url: '/api/app/notification/my-notifications',
      params: { isRead: input.isRead, type: input.type, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getMySettings = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserNotificationSettingsDto>({
      method: 'GET',
      url: '/api/app/notification/my-settings',
    },
    { apiName: this.apiName,...config });
  

  getUnreadCount = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'GET',
      url: '/api/app/notification/unread-count',
    },
    { apiName: this.apiName,...config });
  

  getUserLookup = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LookupDto<string>>({
      method: 'GET',
      url: '/api/app/notification/user-lookup',
    },
    { apiName: this.apiName,...config });
  

  getUsersNotificationStatus = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, UserNotificationSummaryDto[]>({
      method: 'GET',
      url: '/api/app/notification/users-notification-status',
    },
    { apiName: this.apiName,...config });
  

  markAllAsRead = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/notification/mark-all-as-read',
    },
    { apiName: this.apiName,...config });
  

  markAsRead = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/notification/${id}/mark-as-read`,
    },
    { apiName: this.apiName,...config });
  

  sendToAll = (input: CreateNotificationDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/notification/send-to-all',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  sendToUser = (userId: string, input: CreateNotificationDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/notification/send-to-user/${userId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  setUserSilence = (userId: string, input: SetUserSilenceDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/notification/set-user-silence/${userId}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateModuleSubscriptions = (input: UpdateModuleSubscriptionDto[], config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/notification/module-subscriptions',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateMySettings = (input: UpdateNotificationSettingsDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'PUT',
      url: '/api/app/notification/my-settings',
      body: input,
    },
    { apiName: this.apiName,...config });
}