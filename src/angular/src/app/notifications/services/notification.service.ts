import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import {
  NotificationDto,
  UserNotificationSettingsDto,
  UpdateNotificationSettingsDto,
  UserNotificationSummaryDto,
  CreateNotificationDto,
  SetUserSilenceDto,
  GetNotificationsInput,
} from '../models/notification.model';

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private rest = inject(RestService);

  // ── User endpoints ──────────────────────────────────────────────────────

  getMyNotifications(input: GetNotificationsInput = {}): Observable<PagedResult<NotificationDto>> {
    return this.rest.request<void, PagedResult<NotificationDto>>({
      method: 'GET',
      url: '/api/app/notification/my-notifications',
      params: input,
    });
  }

  markAsRead(id: string): Observable<void> {
    return this.rest.request<void, void>({
      method: 'POST',
      url: `/api/app/notification/${id}/mark-as-read`,
    });
  }

  markAllAsRead(): Observable<void> {
    return this.rest.request<void, void>({
      method: 'POST',
      url: '/api/app/notification/mark-all-as-read',
    });
  }

  delete(id: string): Observable<void> {
    return this.rest.request<void, void>({
      method: 'DELETE',
      url: `/api/app/notification/${id}`,
    });
  }

  getUnreadCount(): Observable<number> {
    return this.rest.request<void, number>({
      method: 'GET',
      url: '/api/app/notification/unread-count',
    });
  }

  getMySettings(): Observable<UserNotificationSettingsDto> {
    return this.rest.request<void, UserNotificationSettingsDto>({
      method: 'GET',
      url: '/api/app/notification/my-settings',
    });
  }

  updateMySettings(input: UpdateNotificationSettingsDto): Observable<void> {
    return this.rest.request<UpdateNotificationSettingsDto, void>({
      method: 'PUT',
      url: '/api/app/notification/my-settings',
      body: input,
    });
  }

  // ── Admin endpoints ──────────────────────────────────────────────────────

  sendToUser(userId: string, input: CreateNotificationDto): Observable<void> {
    return this.rest.request<CreateNotificationDto, void>({
      method: 'POST',
      url: `/api/app/notification/send-to-user/${userId}`,
      body: input,
    });
  }

  sendToAll(input: CreateNotificationDto): Observable<void> {
    return this.rest.request<CreateNotificationDto, void>({
      method: 'POST',
      url: '/api/app/notification/send-to-all',
      body: input,
    });
  }

  setUserSilence(userId: string, input: SetUserSilenceDto): Observable<void> {
    return this.rest.request<SetUserSilenceDto, void>({
      method: 'POST',
      url: `/api/app/notification/set-user-silence/${userId}`,
      body: input,
    });
  }

  getUsersNotificationStatus(): Observable<UserNotificationSummaryDto[]> {
    return this.rest.request<void, UserNotificationSummaryDto[]>({
      method: 'GET',
      url: '/api/app/notification/users-notification-status',
    });
  }
}
