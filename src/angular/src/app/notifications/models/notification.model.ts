export interface NotificationDto {
  id: string;
  userId: string;
  title: string;
  message: string;
  type: NotificationType;
  url?: string;
  entityId?: string;
  isRead: boolean;
  createdAt: string;
  sentBy?: string;
}

export type NotificationType =
  | 'appointment'
  | 'lab'
  | 'pharmacy'
  | 'radiology'
  | 'inventory'
  | 'billing'
  | 'emergency'
  | 'system';

export interface NotificationTypeConfig {
  key: NotificationType;
  label: string;
  icon: string;
  color: string;
}

export const NOTIFICATION_TYPES: NotificationTypeConfig[] = [
  { key: 'appointment', label: 'المواعيد',     icon: 'fas fa-calendar-check', color: '#4f46e5' },
  { key: 'lab',         label: 'المختبر',      icon: 'fas fa-flask',          color: '#0891b2' },
  { key: 'pharmacy',    label: 'الصيدلية',     icon: 'fas fa-pills',          color: '#059669' },
  { key: 'radiology',   label: 'الأشعة',       icon: 'fas fa-x-ray',          color: '#7c3aed' },
  { key: 'inventory',   label: 'المخزون',      icon: 'fas fa-boxes',          color: '#d97706' },
  { key: 'billing',     label: 'الفواتير',     icon: 'fas fa-file-invoice',   color: '#dc2626' },
  { key: 'emergency',   label: 'الطوارئ',      icon: 'fas fa-ambulance',      color: '#ef4444' },
  { key: 'system',      label: 'النظام',       icon: 'fas fa-bell',           color: '#6b7280' },
];

export interface UserNotificationSettingsDto {
  isEnabled: boolean;
  globalSilence: boolean;
  silencedUntil?: string;
  enabledTypes: NotificationType[];
}

export interface UpdateNotificationSettingsDto {
  enabledTypes: NotificationType[];
}

export interface UserNotificationSummaryDto {
  userId: string;
  userName: string;
  email?: string;
  isEnabled: boolean;
  globalSilence: boolean;
  silencedUntil?: string;
  unreadCount: number;
  totalCount: number;
}

export interface CreateNotificationDto {
  title: string;
  message: string;
  type: NotificationType;
  url?: string;
  entityId?: string;
}

export interface SetUserSilenceDto {
  isSilenced: boolean;
  silencedUntil?: string;
}

export interface GetNotificationsInput {
  isRead?: boolean;
  type?: NotificationType;
  skipCount?: number;
  maxResultCount?: number;
}
