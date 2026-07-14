import { Injectable, OnDestroy, inject } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Subject } from 'rxjs';
import { AuthService, ConfigStateService } from '@abp/ng.core';
import { NotificationDto } from '../models/notification.model';
import { OAuthService } from 'angular-oauth2-oidc';
import { ToasterService } from '@abp/ng.theme.shared';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class NotificationHubService implements OnDestroy {
  private connection: HubConnection | null = null;
  private oauthService = inject(OAuthService);
  private config = inject(ConfigStateService);
  private toaster = inject(ToasterService);

  /** All notifications received since connection */
  notifications$ = new BehaviorSubject<NotificationDto[]>([]);

  /** Fired when a notification is marked read */
  notificationRead$ = new BehaviorSubject<string | null>(null);

  /** Fired when a notification is deleted from UI */
  notificationDeleted$ = new Subject<string>();

  /** Fired when all notifications were marked read */
  allRead$ = new BehaviorSubject<boolean>(false);

  /** Connection state */
  isConnected$ = new BehaviorSubject<boolean>(false);

  async connect(): Promise<void> {
    if (this.connection && this.connection.state === HubConnectionState.Connected) {
      return;
    }

    const apiUrl = environment.apis.default.url || '';

    this.connection = new HubConnectionBuilder()
      .withUrl(`${apiUrl}/signalr-hubs/notifications`, {
        accessTokenFactory: () => this.oauthService.getAccessToken() || '',
        transport: signalR.HttpTransportType.LongPolling
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(LogLevel.Warning)
      .build();

    // Handle incoming notifications
    this.connection.on('ReceiveNotification', (notification: NotificationDto) => {
      const current = this.notifications$.getValue();
      this.notifications$.next([notification, ...current]);
      
      const showToast = localStorage.getItem('Notifications.ShowToast') !== 'false';
      console.log('[NotificationHub] Received:', notification, 'ShowToast:', showToast);
      if (showToast) {
        this.toaster.success(notification.message, notification.title);
      }
    });

    // Handle single read event (from another tab)
    this.connection.on('NotificationRead', (notificationId: string) => {
      this.notificationRead$.next(notificationId);
    });

    // Handle all-read event
    this.connection.on('AllNotificationsRead', () => {
      this.allRead$.next(true);
    });

    this.connection.onclose(() => this.isConnected$.next(false));
    this.connection.onreconnected(() => this.isConnected$.next(true));

    try {
      await this.connection.start();
      this.isConnected$.next(true);
      console.log('[NotificationHub] Connected');
    } catch (err) {
      console.error('[NotificationHub] Connection failed:', err);
    }
  }

  async disconnect(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      this.isConnected$.next(false);
    }
  }

  ngOnDestroy(): void {
    this.disconnect();
  }
}
