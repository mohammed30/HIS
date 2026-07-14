import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbPaginationModule, NgbModalModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationDto, NotificationType, NOTIFICATION_TYPES } from '../models/notification.model';
import { NotificationService } from '../services/notification.service';
import { NotificationHubService } from '../services/notification-hub.service';

@Component({
  selector: 'app-notifications-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, NgbPaginationModule, NgbModalModule],
  template: `
    <div class="notif-page container-fluid py-4" dir="rtl">

      <!-- Header Card -->
      <div class="notif-header-card mb-4">
        <div class="notif-header-gradient">
          <div class="notif-header-content">
            <div>
              <h4 class="mb-1">
                <i class="fas fa-bell me-2"></i>مركز التنبيهات
              </h4>
              <p class="mb-0 opacity-75">جميع تنبيهاتك في مكان واحد</p>
            </div>
            <div class="notif-header-actions">
              <button class="btn btn-light btn-sm me-2" (click)="markAllAsRead()" *ngIf="totalUnread > 0">
                <i class="fas fa-check-double me-1"></i>قراءة الكل ({{ totalUnread }})
              </button>
              <a routerLink="/notifications/settings" class="btn btn-outline-light btn-sm">
                <i class="fas fa-cog me-1"></i>الإعدادات
              </a>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="notif-filters mb-4">
        <div class="row g-3 align-items-center">
          <div class="col-auto">
            <label class="text-muted small">الحالة:</label>
          </div>
          <div class="col-auto">
            <div class="btn-group" role="group">
              <button class="btn btn-sm" [class.btn-primary]="filterRead === null"
                      [class.btn-outline-secondary]="filterRead !== null" (click)="setFilter(null)">الكل</button>
              <button class="btn btn-sm" [class.btn-warning]="filterRead === false"
                      [class.btn-outline-secondary]="filterRead !== false" (click)="setFilter(false)">غير مقروء</button>
              <button class="btn btn-sm" [class.btn-success]="filterRead === true"
                      [class.btn-outline-secondary]="filterRead !== true" (click)="setFilter(true)">مقروء</button>
            </div>
          </div>
          <div class="col-auto">
            <label class="text-muted small">النوع:</label>
          </div>
          <div class="col-auto">
            <select class="form-select form-select-sm" [(ngModel)]="filterType" (change)="loadData()">
              <option value="">الكل</option>
              <option *ngFor="let t of notifTypes" [value]="t.key">{{ t.label }}</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Notifications Grid -->
      <div class="notif-grid" *ngIf="items.length > 0; else empty">
        <div class="notif-card"
             *ngFor="let n of items"
             [class.unread]="!n.isRead"
             (click)="onCardClick(n)">
          <div class="notif-card-icon" [style.background]="getTypeColor(n.type)">
            <i [class]="getTypeIcon(n.type)"></i>
          </div>
          <div class="notif-card-body">
            <div class="notif-card-header">
              <span class="notif-card-title">{{ n.title }}</span>
              <div class="notif-card-meta">
                <span class="notif-type-badge" [style.background]="getTypeColor(n.type) + '22'"
                      [style.color]="getTypeColor(n.type)">{{ getTypeLabel(n.type) }}</span>
                <span class="notif-time text-muted small">
                  <i class="fas fa-clock me-1"></i>{{ formatTime(n.createdAt) }}
                </span>
              </div>
            </div>
            <p class="notif-card-msg">{{ n.message }}</p>
            <div class="notif-card-footer" *ngIf="n.sentBy">
              <i class="fas fa-user-shield me-1 text-muted"></i>
              <small class="text-muted">أرسله: {{ n.sentBy }}</small>
            </div>
          </div>
          <div class="notif-card-actions">
            <button class="btn btn-sm btn-outline-primary" *ngIf="!n.isRead" (click)="markRead($event, n)">
              <i class="fas fa-check"></i>
            </button>
            <button class="btn btn-sm btn-outline-danger" (click)="deleteNotif($event, n.id)">
              <i class="fas fa-trash"></i>
            </button>
          </div>
          <div class="notif-unread-indicator" *ngIf="!n.isRead"></div>
        </div>
      </div>

      <ng-template #empty>
        <div class="notif-empty-state">
          <i class="fas fa-bell-slash"></i>
          <h5>لا توجد تنبيهات</h5>
          <p class="text-muted">ستظهر التنبيهات هنا عند وصولها</p>
        </div>
      </ng-template>

      <!-- Pagination -->
      <div class="d-flex justify-content-between align-items-center mt-4" *ngIf="totalCount > pageSize">
        <ngb-pagination
          [(page)]="page"
          [pageSize]="pageSize"
          [collectionSize]="totalCount"
          (pageChange)="onPageChange($event)"
          [maxSize]="5"
          [boundaryLinks]="true">
        </ngb-pagination>
        <span class="text-muted small">الإجمالي: {{ totalCount }}</span>
      </div>

    </div>
  `,
  styles: [`
    :host { direction: rtl; }

    .notif-header-card { border-radius: 20px; overflow: hidden; }

    .notif-header-gradient {
      background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%);
      padding: 28px 32px;
    }

    .notif-header-content {
      display: flex;
      align-items: center;
      justify-content: space-between;
      color: #fff;
    }

    .notif-filters {
      background: var(--lpx-card-bg, #1e293b);
      border-radius: 14px;
      padding: 16px 20px;
      border: 1px solid rgba(255,255,255,0.06);
    }

    /* Notification Cards */
    .notif-grid { display: flex; flex-direction: column; gap: 12px; }

    .notif-card {
      display: flex;
      align-items: flex-start;
      gap: 16px;
      background: var(--lpx-card-bg, #1e293b);
      border-radius: 14px;
      padding: 16px 20px;
      border: 1px solid rgba(255,255,255,0.05);
      cursor: pointer;
      transition: all 0.2s ease;
      position: relative;
      overflow: hidden;
    }

    .notif-card:hover { transform: translateY(-1px); box-shadow: 0 8px 25px rgba(0,0,0,0.15); }

    .notif-card.unread {
      border-left: 3px solid #4f46e5;
      background: rgba(79,70,229,0.05);
    }

    .notif-unread-indicator {
      position: absolute;
      left: 0;
      top: 0;
      bottom: 0;
      width: 3px;
      background: linear-gradient(to bottom, #4f46e5, #7c3aed);
    }

    .notif-card-icon {
      width: 46px;
      height: 46px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #fff;
      font-size: 1rem;
      flex-shrink: 0;
    }

    .notif-card-body { flex: 1; min-width: 0; }

    .notif-card-header {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      gap: 12px;
      margin-bottom: 6px;
    }

    .notif-card-title {
      font-weight: 700;
      font-size: 0.9rem;
      color: var(--lpx-text, #f1f5f9);
    }

    .notif-card-meta { display: flex; align-items: center; gap: 10px; flex-shrink: 0; }

    .notif-type-badge {
      font-size: 0.7rem;
      font-weight: 600;
      padding: 2px 10px;
      border-radius: 99px;
    }

    .notif-card-msg {
      font-size: 0.83rem;
      color: #94a3b8;
      line-height: 1.5;
      margin-bottom: 6px;
    }

    .notif-card-footer { font-size: 0.75rem; }

    .notif-card-actions {
      display: flex;
      flex-direction: column;
      gap: 6px;
      flex-shrink: 0;
      opacity: 0;
      transition: opacity 0.2s;
    }

    .notif-card:hover .notif-card-actions { opacity: 1; }

    /* Empty State */
    .notif-empty-state {
      text-align: center;
      padding: 80px 20px;
      color: #64748b;
    }

    .notif-empty-state i { font-size: 4rem; margin-bottom: 20px; display: block; opacity: 0.4; }
    .notif-empty-state h5 { color: #94a3b8; margin-bottom: 8px; }
  `]
})
export class NotificationsPageComponent implements OnInit {
  private notifService = inject(NotificationService);
  private hubService   = inject(NotificationHubService);

  items: NotificationDto[] = [];
  totalCount   = 0;
  totalUnread  = 0;
  page         = 1;
  pageSize     = 15;
  filterRead: boolean | null = null;
  filterType   = '';

  notifTypes = NOTIFICATION_TYPES;
  private typeMap = new Map(NOTIFICATION_TYPES.map(t => [t.key, t]));

  ngOnInit() { this.loadData(); }

  loadData() {
    this.notifService.getMyNotifications({
      isRead: this.filterRead ?? undefined,
      type: (this.filterType || undefined) as any,
      skipCount: (this.page - 1) * this.pageSize,
      maxResultCount: this.pageSize,
    }).subscribe(res => {
      this.items = res.items;
      this.totalCount = res.totalCount;
      this.totalUnread = res.items.filter(n => !n.isRead).length;
    });
  }

  setFilter(val: boolean | null) {
    this.filterRead = val;
    this.page = 1;
    this.loadData();
  }

  onPageChange(p: number) { this.page = p; this.loadData(); }

  onCardClick(n: NotificationDto) {
    if (!n.isRead) this.markRead(null, n);
  }

  markRead(event: Event | null, n: NotificationDto) {
    event?.stopPropagation();
    this.notifService.markAsRead(n.id).subscribe(() => {
      n.isRead = true;
      this.totalUnread = Math.max(0, this.totalUnread - 1);
    });
  }

  markAllAsRead() {
    this.notifService.markAllAsRead().subscribe(() => {
      this.items.forEach(n => n.isRead = true);
      this.totalUnread = 0;
    });
  }

  deleteNotif(event: Event, id: string) {
    event.stopPropagation();
    this.notifService.delete(id).subscribe(() => {
      const n = this.items.find(x => x.id === id);
      this.items = this.items.filter(x => x.id !== id);
      this.totalCount--;
      if (n && !n.isRead) {
        this.totalUnread = Math.max(0, this.totalUnread - 1);
      }
      this.hubService.notificationDeleted$.next(id);
    });
  }

  getTypeIcon(type: string)  { return this.typeMap.get(type as NotificationType)?.icon  ?? 'fas fa-bell'; }
  getTypeColor(type: string) { return this.typeMap.get(type as NotificationType)?.color ?? '#6b7280'; }
  getTypeLabel(type: string) { return this.typeMap.get(type as NotificationType)?.label ?? type; }

  formatTime(dateStr: string): string {
    if (!dateStr.endsWith('Z') && !dateStr.includes('+')) dateStr += 'Z';
    const date = new Date(dateStr);
    const diff = Date.now() - date.getTime();
    const min  = Math.floor(diff / 60000);
    if (min < 1)   return 'الآن';
    if (min < 60)  return `منذ ${min} دقيقة`;
    const hrs = Math.floor(min / 60);
    if (hrs < 24)  return `منذ ${hrs} ساعة`;
    return date.toLocaleDateString('ar-EG', { day:'numeric', month:'short', year:'numeric' });
  }
}
