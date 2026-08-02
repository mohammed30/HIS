import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '@abp/ng.core';
import { NotificationDto, NOTIFICATION_TYPES } from '../models/notification.model';
import { NotificationService } from '../services/notification.service';

@Component({
  selector: 'app-login-notifications-modal',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <!-- Backdrop -->
    <div class="login-modal-backdrop" *ngIf="isVisible" (click)="dismiss()">
      <div class="login-modal-panel" (click)="$event.stopPropagation()">

        <!-- Header -->
        <div class="lm-header">
          <div class="lm-header-icon">
            <i class="fas fa-bell"></i>
            <span class="lm-badge">{{ notifications.length }}</span>
          </div>
          <div>
            <h5 class="mb-0">مرحباً بعودتك! 👋</h5>
            <p class="mb-0 opacity-75 small">
              لديك <strong>{{ notifications.length }}</strong> تنبيه جديد منذ آخر دخول
            </p>
          </div>
          <button class="lm-close-btn" (click)="dismiss()">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <!-- Notifications -->
        <div class="lm-body">
          <div class="lm-notif-item"
               *ngFor="let n of notifications.slice(0, 5)"
               (click)="navigate(n)">
            <div class="lm-notif-icon" [style.background]="getTypeColor(n.type)">
              <i [class]="getTypeIcon(n.type)"></i>
            </div>
            <div class="lm-notif-content">
              <div class="lm-notif-title">{{ n.title }}</div>
              <div class="lm-notif-msg">{{ n.message | slice:0:80 }}{{ n.message.length > 80 ? '...' : '' }}</div>
              <div class="lm-notif-time text-muted">
                <i class="fas fa-clock me-1"></i>{{ formatTime(n.createdAt) }}
              </div>
            </div>
          </div>

          <div *ngIf="notifications.length > 5" class="lm-more">
            <i class="fas fa-ellipsis-h me-1"></i>
            و {{ notifications.length - 5 }} تنبيه آخر...
          </div>
        </div>

        <!-- Footer -->
        <div class="lm-footer">
          <a routerLink="/notifications" class="btn btn-primary" (click)="dismiss()">
            <i class="fas fa-list me-2"></i>عرض جميع التنبيهات
          </a>
          <button class="btn btn-outline-secondary" (click)="markAllAndDismiss()">
            <i class="fas fa-check-double me-2"></i>قراءة الكل وإغلاق
          </button>
          <button class="btn btn-link text-muted small" (click)="dismissForToday()">
            لا تُظهر اليوم مجدداً
          </button>
        </div>

      </div>
    </div>
  `,
  styles: [`
    .login-modal-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(0,0,0,0.4);
      backdrop-filter: blur(4px);
      z-index: 10000;
      display: flex;
      align-items: center;
      justify-content: center;
      animation: fadeIn 0.3s ease;
    }

    @keyframes fadeIn { from { opacity:0; } to { opacity:1; } }

    .login-modal-panel {
      width: 460px;
      max-width: 95vw;
      background: #ffffff;
      border-radius: 24px;
      overflow: hidden;
      box-shadow: 0 20px 40px rgba(0,0,0,0.1);
      border: 1px solid #f1f5f9;
      animation: slideUp 0.35s cubic-bezier(.4,0,.2,1);
    }

    @keyframes slideUp {
      from { opacity:0; transform:translateY(30px) scale(0.96); }
      to   { opacity:1; transform:translateY(0) scale(1); }
    }

    /* Header */
    .lm-header {
      display: flex;
      align-items: center;
      gap: 14px;
      padding: 22px 24px;
      background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%);
      color: #fff;
    }

    .lm-header-icon {
      position: relative;
      width: 48px;
      height: 48px;
      border-radius: 14px;
      background: rgba(255,255,255,0.15);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.3rem;
      flex-shrink: 0;
    }

    .lm-badge {
      position: absolute;
      top: -4px;
      right: -4px;
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
    }

    .lm-close-btn {
      margin-right: auto;
      background: rgba(255,255,255,0.15);
      border: none;
      color: #fff;
      width: 32px;
      height: 32px;
      border-radius: 8px;
      cursor: pointer;
      transition: background 0.2s;
    }

    .lm-close-btn:hover { background: rgba(255,255,255,0.25); }

    /* Body */
    .lm-body {
      max-height: 360px;
      overflow-y: auto;
      padding: 12px 0;
    }

    .lm-notif-item {
      display: flex;
      align-items: flex-start;
      gap: 12px;
      padding: 12px 22px;
      cursor: pointer;
      transition: background 0.15s;
    }

    .lm-notif-item:hover { background: #f8fafc; }

    .lm-notif-icon {
      width: 38px;
      height: 38px;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #fff;
      font-size: 0.85rem;
      flex-shrink: 0;
    }

    .lm-notif-title {
      font-weight: 700;
      font-size: 0.9rem;
      color: #1e293b;
      margin-bottom: 2px;
    }

    .lm-notif-msg {
      font-size: 0.8rem;
      color: #64748b;
      line-height: 1.4;
    }

    .lm-notif-time { font-size: 0.72rem; margin-top: 4px; color: #94a3b8; }

    .lm-more {
      text-align: center;
      padding: 8px;
      color: #64748b;
      font-size: 0.8rem;
    }

    /* Footer */
    .lm-footer {
      display: flex;
      flex-direction: column;
      gap: 8px;
      padding: 16px 22px;
      border-top: 1px solid #f1f5f9;
      background: #ffffff;
    }

    .lm-footer .btn { border-radius: 10px; font-weight: 600; }
    .lm-footer .btn-outline-secondary { border-color: #e2e8f0; color: #475569; }
    .lm-footer .btn-outline-secondary:hover { background: #f8fafc; color: #1e293b; }
  `]
})
export class LoginNotificationsModalComponent implements OnInit {
  private notifService = inject(NotificationService);
  private router       = inject(Router);

  isVisible     = false;
  notifications: NotificationDto[] = [];

  private readonly DISMISS_KEY = 'his-notif-dismissed-date';
  private typeMap = new Map(NOTIFICATION_TYPES.map(t => [t.key, t]));

  ngOnInit() {
    // Don't show if dismissed today
    const lastDismiss = localStorage.getItem(this.DISMISS_KEY);
    if (lastDismiss === new Date().toDateString()) return;

    // Load unread notifications
    this.notifService.getMyNotifications({ isRead: false, maxResultCount: 20 }).subscribe(res => {
      if (res.totalCount > 0) {
        this.notifications = res.items;
        this.isVisible = true;
      }
    });
  }

  dismiss() { this.isVisible = false; }

  dismissForToday() {
    localStorage.setItem(this.DISMISS_KEY, new Date().toDateString());
    this.isVisible = false;
  }

  markAllAndDismiss() {
    this.notifService.markAllAsRead().subscribe();
    this.isVisible = false;
  }

  navigate(n: NotificationDto) {
    this.dismiss();
    if (n.url) this.router.navigateByUrl(n.url);
  }

  getTypeIcon(type: string)  { return this.typeMap.get(type as any)?.icon  ?? 'fas fa-bell'; }
  getTypeColor(type: string) { return this.typeMap.get(type as any)?.color ?? '#6b7280'; }

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
}
