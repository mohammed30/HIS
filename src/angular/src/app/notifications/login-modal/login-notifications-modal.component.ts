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
      --lm-bg: #ffffff;
      --lm-border: #f1f5f9;
      --lm-text-title: #1e293b;
      --lm-text-msg: #64748b;
      --lm-text-time: #94a3b8;
      --lm-hover: #f8fafc;
      --lm-footer-bg: #fcfcfd;
      --lm-btn-border: #e2e8f0;
      --lm-btn-text: #475569;
      --lm-btn-hover-bg: #f1f5f9;
      --lm-btn-hover-text: #1e293b;
      --lm-shadow: rgba(0,0,0,0.12);

      width: 480px;
      max-width: 95vw;
      background: var(--lm-bg);
      border-radius: 24px;
      overflow: hidden;
      box-shadow: 0 25px 50px -12px var(--lm-shadow);
      border: 1px solid var(--lm-border);
      animation: slideUp 0.4s cubic-bezier(0.16, 1, 0.3, 1);
      transition: background 0.3s ease, border-color 0.3s ease;
    }

    :host-context([data-theme="dark"]) .login-modal-panel,
    :host-context(.dark) .login-modal-panel,
    :host-context(.lpx-theme-dark) .login-modal-panel {
      --lm-bg: #151923;
      --lm-border: #2a3143;
      --lm-text-title: #f1f5f9;
      --lm-text-msg: #94a3b8;
      --lm-text-time: #64748b;
      --lm-hover: #1e2332;
      --lm-footer-bg: #11141c;
      --lm-btn-border: #334155;
      --lm-btn-text: #cbd5e1;
      --lm-btn-hover-bg: #2a3143;
      --lm-btn-hover-text: #f8fafc;
      --lm-shadow: rgba(0,0,0,0.5);
    }

    @keyframes slideUp {
      from { opacity:0; transform:translateY(30px) scale(0.96); }
      to   { opacity:1; transform:translateY(0) scale(1); }
    }

    /* Header */
    .lm-header {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 24px 28px;
      background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
      color: #fff;
      position: relative;
      overflow: hidden;
    }

    .lm-header::after {
      content: '';
      position: absolute;
      top: -50%;
      right: -50%;
      width: 200%;
      height: 200%;
      background: radial-gradient(circle, rgba(255,255,255,0.1) 0%, transparent 60%);
      animation: rotateBg 15s linear infinite;
      pointer-events: none;
    }

    @keyframes rotateBg {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .lm-header-icon {
      position: relative;
      width: 52px;
      height: 52px;
      border-radius: 16px;
      background: rgba(255,255,255,0.2);
      backdrop-filter: blur(8px);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.4rem;
      flex-shrink: 0;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
    }

    .lm-badge {
      position: absolute;
      top: -6px;
      right: -6px;
      background: #ef4444;
      color: #fff;
      border-radius: 99px;
      font-size: 0.7rem;
      font-weight: 700;
      min-width: 20px;
      height: 20px;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 0 5px;
      box-shadow: 0 2px 6px rgba(239, 68, 68, 0.4);
      animation: pulseBadge 2s infinite;
    }

    @keyframes pulseBadge {
      0% { transform: scale(1); }
      50% { transform: scale(1.1); }
      100% { transform: scale(1); }
    }

    .lm-close-btn {
      margin-right: auto;
      background: rgba(255,255,255,0.15);
      border: none;
      color: #fff;
      width: 36px;
      height: 36px;
      border-radius: 10px;
      cursor: pointer;
      transition: all 0.2s ease;
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 2;
    }

    .lm-close-btn:hover { 
      background: rgba(255,255,255,0.25);
      transform: scale(1.05);
    }
    
    .lm-close-btn:active {
      transform: scale(0.95);
    }

    /* Body */
    .lm-body {
      max-height: 400px;
      overflow-y: auto;
      padding: 16px 0;
    }

    /* Scrollbar styling */
    .lm-body::-webkit-scrollbar { width: 6px; }
    .lm-body::-webkit-scrollbar-track { background: transparent; }
    .lm-body::-webkit-scrollbar-thumb { background: var(--lm-border); border-radius: 10px; }
    .lm-body::-webkit-scrollbar-thumb:hover { background: var(--lm-text-time); }

    .lm-notif-item {
      display: flex;
      align-items: flex-start;
      gap: 16px;
      padding: 16px 20px;
      margin: 4px 16px;
      border-radius: 16px;
      cursor: pointer;
      transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
      border: 1px solid transparent;
    }

    .lm-notif-item:hover { 
      background: var(--lm-hover);
      border-color: var(--lm-border);
      transform: translateY(-2px);
      box-shadow: 0 4px 12px var(--lm-shadow);
    }

    .lm-notif-icon {
      width: 42px;
      height: 42px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #fff;
      font-size: 0.95rem;
      flex-shrink: 0;
      transition: transform 0.3s ease;
    }

    .lm-notif-item:hover .lm-notif-icon {
      transform: scale(1.1) rotate(-5deg);
    }

    .lm-notif-content {
      flex: 1;
    }

    .lm-notif-title {
      font-weight: 700;
      font-size: 0.95rem;
      color: var(--lm-text-title);
      margin-bottom: 4px;
      transition: color 0.3s ease;
    }

    .lm-notif-msg {
      font-size: 0.85rem;
      color: var(--lm-text-msg);
      line-height: 1.5;
      transition: color 0.3s ease;
    }

    .lm-notif-time { 
      font-size: 0.75rem; 
      margin-top: 6px; 
      color: var(--lm-text-time);
      display: flex;
      align-items: center;
      gap: 4px;
      transition: color 0.3s ease;
    }

    .lm-more {
      text-align: center;
      padding: 12px;
      color: var(--lm-text-msg);
      font-size: 0.85rem;
      font-weight: 500;
      transition: color 0.3s ease;
    }

    /* Footer */
    .lm-footer {
      display: flex;
      flex-direction: column;
      gap: 10px;
      padding: 20px 28px;
      border-top: 1px solid var(--lm-border);
      background: var(--lm-footer-bg);
      transition: background 0.3s ease, border-color 0.3s ease;
    }

    .lm-footer .btn { 
      border-radius: 12px; 
      font-weight: 600; 
      padding: 10px 16px;
      transition: all 0.2s ease;
    }
    
    .lm-footer .btn-primary {
      background: linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%);
      border: none;
      box-shadow: 0 4px 12px rgba(99, 102, 241, 0.25);
      color: #fff;
    }
    
    .lm-footer .btn-primary:hover {
      box-shadow: 0 6px 16px rgba(99, 102, 241, 0.4);
      transform: translateY(-1px);
    }

    .lm-footer .btn-primary:active {
      transform: translateY(1px);
    }

    .lm-footer .btn-outline-secondary { 
      border: 1px solid var(--lm-btn-border); 
      color: var(--lm-btn-text); 
      background: transparent;
    }
    
    .lm-footer .btn-outline-secondary:hover { 
      background: var(--lm-btn-hover-bg); 
      color: var(--lm-btn-hover-text); 
      border-color: var(--lm-btn-hover-text);
    }

    .lm-footer .btn-link {
      color: var(--lm-text-time);
      text-decoration: none;
    }

    .lm-footer .btn-link:hover {
      color: var(--lm-text-title);
      text-decoration: underline;
    }
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
