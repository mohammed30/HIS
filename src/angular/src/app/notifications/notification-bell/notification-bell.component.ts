import {
  Component, OnInit, OnDestroy, inject, HostListener, ElementRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '@abp/ng.core';
import { NotificationDto, NOTIFICATION_TYPES } from '../models/notification.model';
import { NotificationService } from '../services/notification.service';
import { NotificationHubService } from '../services/notification-hub.service';

@Component({
  selector: 'app-notification-bell',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="notif-bell-wrapper" (clickOutside)="closeDropdown()">

      <!-- Bell Button -->
      <button
        id="notif-bell-btn"
        class="notif-bell-btn"
        [class.has-unread]="unreadCount > 0"
        (click)="toggleDropdown()"
        [attr.aria-label]="'التنبيهات (' + unreadCount + ' غير مقروء)'"
        title="التنبيهات">
        <i class="fas fa-bell" [class.fa-shake]="shaking"></i>
        <span class="notif-badge" *ngIf="unreadCount > 0">
          {{ unreadCount > 99 ? '99+' : unreadCount }}
        </span>
      </button>

      <!-- Dropdown -->
      <div class="notif-dropdown" *ngIf="isOpen" id="notif-dropdown">
        <!-- Header -->
        <div class="notif-dropdown-header">
          <span class="notif-title">
            <i class="fas fa-bell me-2"></i>التنبيهات
            <span class="badge bg-primary ms-2" *ngIf="unreadCount > 0">{{ unreadCount }}</span>
          </span>
          <div class="notif-header-actions">
            <button class="btn btn-link btn-sm" (click)="markAllAsRead()" *ngIf="unreadCount > 0" title="قراءة الكل">
              <i class="fas fa-check-double"></i>
            </button>
            <a routerLink="/notifications/settings" class="btn btn-link btn-sm" title="الإعدادات" (click)="closeDropdown()">
              <i class="fas fa-cog"></i>
            </a>
          </div>
        </div>

        <!-- Notifications List -->
        <div class="notif-list" *ngIf="notifications.length > 0; else emptyNotif">
          <div
            class="notif-item"
            *ngFor="let n of notifications"
            [class.unread]="!n.isRead"
            (click)="onNotifClick(n)">
            <div class="notif-icon" [style.background]="getTypeColor(n.type)">
              <i [class]="getTypeIcon(n.type)"></i>
            </div>
            <div class="notif-content">
              <div class="notif-item-title">{{ n.title }}</div>
              <div class="notif-item-msg">{{ n.message | slice:0:60 }}{{ n.message.length > 60 ? '...' : '' }}</div>
              <div class="notif-item-time">
                <i class="fas fa-clock me-1"></i>{{ formatTime(n.createdAt) }}
              </div>
            </div>
            <div class="notif-unread-dot" *ngIf="!n.isRead"></div>
          </div>
        </div>

        <ng-template #emptyNotif>
          <div class="notif-empty">
            <i class="fas fa-bell-slash fs-2 mb-2"></i>
            <p>لا توجد تنبيهات</p>
          </div>
        </ng-template>

        <!-- Footer -->
        <div class="notif-dropdown-footer">
          <a routerLink="/notifications" class="btn btn-link w-100" (click)="closeDropdown()">
            عرض جميع التنبيهات
            <i class="fas fa-arrow-left ms-1"></i>
          </a>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .notif-bell-wrapper {
      position: relative;
      display: inline-flex;
      align-items: center;
    }

    .notif-bell-btn {
      position: relative;
      background: transparent;
      border: none;
      cursor: pointer;
      padding: 8px 10px;
      border-radius: 10px;
      color: var(--lpx-text-muted, #94a3b8);
      font-size: 1.1rem;
      transition: all 0.2s ease;
      outline: none;
    }

    .notif-bell-btn:hover,
    .notif-bell-btn.has-unread {
      color: #4f46e5;
      background: rgba(79, 70, 229, 0.08);
    }

    .notif-badge {
      position: absolute;
      top: 2px;
      right: 2px;
      background: #ef4444;
      color: #fff;
      border-radius: 99px;
      font-size: 0.65rem;
      font-weight: 700;
      min-width: 18px;
      height: 18px;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 0 4px;
      animation: pop-in 0.3s ease;
    }

    @keyframes pop-in {
      from { transform: scale(0); }
      to   { transform: scale(1); }
    }

    /* Dropdown */
    .notif-dropdown {
      position: absolute;
      top: calc(100% + 10px);
      left: 0;
      width: 360px;
      background: var(--lpx-card-bg, #1e293b);
      border-radius: 16px;
      box-shadow: 0 25px 50px rgba(0,0,0,0.3);
      border: 1px solid rgba(255,255,255,0.06);
      z-index: 9999;
      overflow: hidden;
      animation: slide-down 0.25s cubic-bezier(.4,0,.2,1);
    }

    @keyframes slide-down {
      from { opacity: 0; transform: translateY(-12px); }
      to   { opacity: 1; transform: translateY(0); }
    }

    .notif-dropdown-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 14px 16px;
      border-bottom: 1px solid rgba(255,255,255,0.06);
    }

    .notif-title {
      font-weight: 700;
      font-size: 0.95rem;
      color: var(--lpx-text, #f1f5f9);
    }

    .notif-header-actions {
      display: flex;
      gap: 4px;
    }

    .notif-header-actions .btn-link {
      color: #94a3b8;
      padding: 4px 8px;
      border-radius: 8px;
      transition: all 0.2s;
    }

    .notif-header-actions .btn-link:hover {
      color: #4f46e5;
      background: rgba(79,70,229,0.1);
    }

    /* Notification Items */
    .notif-list {
      max-height: 340px;
      overflow-y: auto;
    }

    .notif-item {
      display: flex;
      align-items: flex-start;
      gap: 12px;
      padding: 12px 16px;
      cursor: pointer;
      transition: background 0.15s;
      position: relative;
      border-bottom: 1px solid rgba(255,255,255,0.04);
    }

    .notif-item:hover {
      background: rgba(255,255,255,0.04);
    }

    .notif-item.unread {
      background: rgba(79, 70, 229, 0.06);
    }

    .notif-icon {
      width: 38px;
      height: 38px;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #fff;
      font-size: 0.85rem;
      flex-shrink: 0;
      opacity: 0.85;
    }

    .notif-content {
      flex: 1;
      min-width: 0;
    }

    .notif-item-title {
      font-weight: 600;
      font-size: 0.85rem;
      color: var(--lpx-text, #f1f5f9);
      margin-bottom: 2px;
    }

    .notif-item-msg {
      font-size: 0.78rem;
      color: #94a3b8;
      line-height: 1.4;
    }

    .notif-item-time {
      font-size: 0.72rem;
      color: #64748b;
      margin-top: 4px;
    }

    .notif-unread-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: #4f46e5;
      flex-shrink: 0;
      margin-top: 6px;
    }

    /* Empty State */
    .notif-empty {
      text-align: center;
      padding: 32px 16px;
      color: #64748b;
    }

    /* Footer */
    .notif-dropdown-footer {
      padding: 8px;
      border-top: 1px solid rgba(255,255,255,0.06);
    }

    .notif-dropdown-footer .btn-link {
      color: #4f46e5;
      font-size: 0.85rem;
      font-weight: 600;
      text-decoration: none;
    }

    .notif-dropdown-footer .btn-link:hover {
      color: #6366f1;
    }
  `]
})
export class NotificationBellComponent implements OnInit, OnDestroy {
  private notifService   = inject(NotificationService);
  private hubService     = inject(NotificationHubService);
  private router         = inject(Router);
  private el             = inject(ElementRef);
  private destroy$       = new Subject<void>();

  notifications: NotificationDto[] = [];
  unreadCount  = 0;
  isOpen       = false;
  shaking      = false;

  private typeMap = new Map(
    NOTIFICATION_TYPES.map(t => [t.key, t])
  );

  ngOnInit() {
    // Ensure we are connected to SignalR when the bell component initializes
    this.hubService.connect();

    this.loadNotifications();

    // Listen for new real-time notifications
    this.hubService.notifications$
      .pipe(takeUntil(this.destroy$))
      .subscribe(notifs => {
        if (notifs.length > 0) {
          // Prepend new ones from hub to our list
          const newOnes = notifs.filter(n => !this.notifications.find(x => x.id === n.id));
          if (newOnes.length > 0) {
            this.notifications = [...newOnes, ...this.notifications].slice(0, 10);
            this.unreadCount += newOnes.length;
            this.triggerShake();
          }
        }
      });

    // Listen for read events
    this.hubService.notificationRead$
      .pipe(takeUntil(this.destroy$))
      .subscribe(id => {
        if (id) {
          const n = this.notifications.find(x => x.id === id);
          if (n && !n.isRead) {
            n.isRead = true;
            this.unreadCount = Math.max(0, this.unreadCount - 1);
          }
        }
      });

    this.hubService.allRead$
      .pipe(takeUntil(this.destroy$))
      .subscribe(all => {
        if (all) {
          this.notifications.forEach(n => n.isRead = true);
          this.unreadCount = 0;
        }
      });

    // Listen for deleted events
    this.hubService.notificationDeleted$
      .pipe(takeUntil(this.destroy$))
      .subscribe(id => {
        if (id) {
          const n = this.notifications.find(x => x.id === id);
          if (n) {
            this.notifications = this.notifications.filter(x => x.id !== id);
            if (!n.isRead) {
              this.unreadCount = Math.max(0, this.unreadCount - 1);
            }
          }
        }
      });
  }

  private loadNotifications() {
    this.notifService.getMyNotifications({ maxResultCount: 10 }).subscribe({
      next: res => {
        this.notifications = res.items;
        this.unreadCount = res.items.filter(n => !n.isRead).length;
      },
      error: () => {}
    });
  }

  toggleDropdown() {
    this.isOpen = !this.isOpen;
  }

  closeDropdown() {
    this.isOpen = false;
  }

  markAllAsRead() {
    this.notifService.markAllAsRead().subscribe({
      next: () => {
        this.notifications.forEach(n => n.isRead = true);
        this.unreadCount = 0;
      }
    });
  }

  onNotifClick(n: NotificationDto) {
    if (!n.isRead) {
      this.notifService.markAsRead(n.id).subscribe();
      n.isRead = true;
      this.unreadCount = Math.max(0, this.unreadCount - 1);
    }
    this.closeDropdown();
    if (n.url) {
      this.router.navigateByUrl(n.url);
    }
  }

  getTypeIcon(type: string): string {
    return this.typeMap.get(type as any)?.icon ?? 'fas fa-bell';
  }

  getTypeColor(type: string): string {
    return this.typeMap.get(type as any)?.color ?? '#6b7280';
  }

  formatTime(dateStr: string): string {
    if (!dateStr.endsWith('Z') && !dateStr.includes('+')) dateStr += 'Z';
    const date = new Date(dateStr);
    const diff = Date.now() - date.getTime();
    const min  = Math.floor(diff / 60000);
    if (min < 1)   return 'الآن';
    if (min < 60)  return `منذ ${min} دقيقة`;
    const hrs = Math.floor(min / 60);
    if (hrs < 24)  return `منذ ${hrs} ساعة`;
    return date.toLocaleDateString('ar-EG');
  }

  private triggerShake() {
    this.shaking = true;
    setTimeout(() => this.shaking = false, 1000);
  }

  @HostListener('document:click', ['$event'])
  onDocClick(event: Event) {
    if (!this.el.nativeElement.contains(event.target)) {
      this.isOpen = false;
    }
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
