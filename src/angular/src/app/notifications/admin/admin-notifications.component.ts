import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModalModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import {
  CreateNotificationDto, NotificationType, NOTIFICATION_TYPES,
  UserNotificationSummaryDto, SetUserSilenceDto
} from '../models/notification.model';
import { NotificationService } from '../services/notification.service';

@Component({
  selector: 'app-admin-notifications',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbModalModule],
  template: `
    <div class="admin-notif-page container-fluid py-4" dir="rtl">

      <!-- Page Header -->
      <div class="admin-header-card mb-4">
        <div class="admin-header-gradient">
          <div class="admin-header-content">
            <div>
              <h4 class="mb-1">
                <i class="fas fa-shield-alt me-2"></i>إدارة التنبيهات
              </h4>
              <p class="mb-0 opacity-75">إرسال التنبيهات وإدارة صلاحيات المستخدمين</p>
            </div>
            <div class="admin-stats-row">
              <div class="admin-stat-pill">
                <i class="fas fa-users me-1"></i>{{ users.length }} مستخدم
              </div>
              <div class="admin-stat-pill warning" *ngIf="silencedCount > 0">
                <i class="fas fa-volume-mute me-1"></i>{{ silencedCount }} مكتوم
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="row g-4">

        <!-- LEFT: Send Notification Panel -->
        <div class="col-lg-4">
          <div class="admin-panel">
            <div class="admin-panel-header">
              <i class="fas fa-paper-plane me-2 text-primary"></i>
              <strong>إرسال تنبيه</strong>
            </div>
            <div class="admin-panel-body">

              <!-- Target -->
              <div class="mb-3">
                <label class="form-label">المستلم</label>
                <div class="btn-group w-100 mb-2">
                  <button class="btn btn-sm"
                          [class.btn-primary]="sendTarget === 'all'"
                          [class.btn-outline-secondary]="sendTarget !== 'all'"
                          (click)="sendTarget = 'all'">
                    <i class="fas fa-users me-1"></i>الكل
                  </button>
                  <button class="btn btn-sm"
                          [class.btn-primary]="sendTarget === 'user'"
                          [class.btn-outline-secondary]="sendTarget !== 'user'"
                          (click)="sendTarget = 'user'">
                    <i class="fas fa-user me-1"></i>مستخدم محدد
                  </button>
                </div>
                <select class="form-select form-select-sm" *ngIf="sendTarget === 'user'"
                        [(ngModel)]="selectedUserId">
                  <option value="">-- اختر مستخدماً --</option>
                  <option *ngFor="let u of users" [value]="u.userId">
                    {{ u.userName }} ({{ u.email }})
                  </option>
                </select>
              </div>

              <!-- Type -->
              <div class="mb-3">
                <label class="form-label">نوع التنبيه</label>
                <div class="type-chips">
                  <div class="type-chip"
                       *ngFor="let t of notifTypes"
                       [class.selected]="sendForm.type === t.key"
                       [style.--chip-color]="t.color"
                       (click)="sendForm.type = t.key">
                    <i [class]="t.icon"></i>
                    <span>{{ t.label }}</span>
                  </div>
                </div>
              </div>

              <!-- Title -->
              <div class="mb-3">
                <label class="form-label">العنوان <span class="text-danger">*</span></label>
                <input class="form-control form-control-sm" [(ngModel)]="sendForm.title"
                       placeholder="عنوان التنبيه...">
              </div>

              <!-- Message -->
              <div class="mb-3">
                <label class="form-label">الرسالة <span class="text-danger">*</span></label>
                <textarea class="form-control form-control-sm" rows="3" [(ngModel)]="sendForm.message"
                          placeholder="نص التنبيه..."></textarea>
              </div>

              <!-- URL (optional) -->
              <div class="mb-4">
                <label class="form-label">رابط (اختياري)</label>
                <input class="form-control form-control-sm" [(ngModel)]="sendForm.url"
                       placeholder="مثل: /notifications أو /laboratory/requests">
              </div>

              <button class="btn btn-primary w-100" (click)="sendNotification()"
                      [disabled]="sending || !sendForm.title || !sendForm.message">
                <span *ngIf="!sending">
                  <i class="fas fa-paper-plane me-2"></i>
                  {{ sendTarget === 'all' ? 'إرسال للجميع' : 'إرسال للمستخدم' }}
                </span>
                <span *ngIf="sending">
                  <span class="spinner-border spinner-border-sm me-2"></span>
                  جاري الإرسال...
                </span>
              </button>

              <div class="send-success mt-3" *ngIf="sendSuccess">
                <i class="fas fa-check-circle me-2"></i>تم الإرسال بنجاح!
              </div>
            </div>
          </div>
        </div>

        <!-- RIGHT: Users Status Table -->
        <div class="col-lg-8">
          <div class="admin-panel">
            <div class="admin-panel-header">
              <i class="fas fa-users me-2 text-primary"></i>
              <strong>حالة التنبيهات للمستخدمين</strong>
              <button class="btn btn-sm btn-outline-secondary ms-auto" (click)="loadUsers()">
                <i class="fas fa-sync-alt" [class.fa-spin]="loading"></i>
              </button>
            </div>

            <!-- Search -->
            <div class="p-3 border-bottom border-opacity-10">
              <input class="form-control form-control-sm" [(ngModel)]="searchText"
                     placeholder="🔍 بحث عن مستخدم...">
            </div>

            <div class="table-responsive">
              <table class="table table-hover align-middle mb-0" *ngIf="!loading">
                <thead>
                  <tr>
                    <th>المستخدم</th>
                    <th class="text-center">التنبيهات</th>
                    <th class="text-center">الحالة</th>
                    <th class="text-center">إجراءات</th>
                  </tr>
                </thead>
                <tbody>
                  <tr *ngFor="let u of filteredUsers">
                    <td>
                      <div class="user-info">
                        <div class="user-avatar">{{ u.userName[0].toUpperCase() }}</div>
                        <div>
                          <div class="fw-bold small">{{ u.userName }}</div>
                          <small class="text-muted">{{ u.email }}</small>
                        </div>
                      </div>
                    </td>
                    <td class="text-center">
                      <span class="badge bg-danger me-1" *ngIf="u.unreadCount > 0">{{ u.unreadCount }} جديد</span>
                      <span class="text-muted small">{{ u.totalCount }} إجمالي</span>
                    </td>
                    <td class="text-center">
                      <span class="status-badge active" *ngIf="!u.globalSilence && u.isEnabled">
                        <i class="fas fa-bell me-1"></i>فعّال
                      </span>
                      <span class="status-badge silenced" *ngIf="u.globalSilence || !u.isEnabled">
                        <i class="fas fa-bell-slash me-1"></i>
                        {{ u.globalSilence ? 'مكتوم' : 'معطل' }}
                        <small *ngIf="u.silencedUntil"> حتى {{ u.silencedUntil | date:'MM/dd HH:mm' }}</small>
                      </span>
                    </td>
                    <td class="text-center">
                      <div class="d-flex justify-content-center gap-2">
                        <!-- Toggle Silence -->
                        <button class="btn btn-sm"
                                [class.btn-warning]="!u.globalSilence"
                                [class.btn-success]="u.globalSilence"
                                (click)="toggleSilence(u)"
                                [title]="u.globalSilence ? 'إلغاء الصمت' : 'تفعيل الصمت'">
                          <i class="fas" [class.fa-volume-mute]="!u.globalSilence" [class.fa-volume-up]="u.globalSilence"></i>
                        </button>
                        <!-- Send direct notification -->
                        <button class="btn btn-sm btn-primary" (click)="quickSend(u)"
                                title="إرسال تنبيه">
                          <i class="fas fa-paper-plane"></i>
                        </button>
                      </div>
                    </td>
                  </tr>
                  <tr *ngIf="filteredUsers.length === 0">
                    <td colspan="4" class="text-center text-muted py-4">لا توجد نتائج</td>
                  </tr>
                </tbody>
              </table>

              <div class="text-center p-4" *ngIf="loading">
                <div class="spinner-border text-primary"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host { direction: rtl; display: block; }

    /* Header */
    .admin-header-card { border-radius: 20px; overflow: hidden; }
    .admin-header-gradient {
      background: linear-gradient(135deg, #1e1b4b 0%, #4f46e5 100%);
      padding: 28px 32px;
    }
    .admin-header-content {
      display: flex;
      align-items: center;
      justify-content: space-between;
      color: #fff;
    }
    .admin-stats-row { display: flex; gap: 10px; }
    .admin-stat-pill {
      background: rgba(255,255,255,0.15);
      border-radius: 99px;
      padding: 6px 16px;
      font-size: 0.82rem;
      font-weight: 600;
      color: #fff;
    }
    .admin-stat-pill.warning { background: rgba(251,191,36,0.25); }

    /* Panels */
    .admin-panel {
      background: var(--lpx-card-bg, #1e293b);
      border-radius: 18px;
      border: 1px solid rgba(255,255,255,0.06);
      overflow: hidden;
    }
    .admin-panel-header {
      display: flex;
      align-items: center;
      padding: 14px 20px;
      border-bottom: 1px solid rgba(255,255,255,0.06);
      font-size: 0.9rem;
      color: var(--lpx-text, #f1f5f9);
    }
    .admin-panel-body { padding: 20px; }

    /* Type Chips */
    .type-chips { display: flex; flex-wrap: wrap; gap: 8px; }
    .type-chip {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 6px 12px;
      border-radius: 99px;
      border: 1px solid rgba(255,255,255,0.1);
      cursor: pointer;
      font-size: 0.78rem;
      color: #94a3b8;
      transition: all 0.2s;
    }
    .type-chip:hover { border-color: var(--chip-color); color: var(--chip-color); }
    .type-chip.selected {
      background: rgba(var(--chip-color), 0.1);
      border-color: var(--chip-color, #4f46e5);
      color: var(--chip-color, #4f46e5);
    }
    .type-chip.selected { background: rgba(79,70,229,0.1); border-color: #4f46e5; color: #4f46e5; }

    /* Users Table */
    .user-info { display: flex; align-items: center; gap: 10px; }
    .user-avatar {
      width: 36px;
      height: 36px;
      border-radius: 10px;
      background: linear-gradient(135deg, #4f46e5, #7c3aed);
      display: flex;
      align-items: center;
      justify-content: center;
      color: #fff;
      font-weight: 700;
      font-size: 0.85rem;
      flex-shrink: 0;
    }

    .status-badge {
      display: inline-flex;
      align-items: center;
      padding: 4px 12px;
      border-radius: 99px;
      font-size: 0.75rem;
      font-weight: 600;
    }
    .status-badge.active  { background: rgba(16,185,129,0.1); color: #10b981; }
    .status-badge.silenced { background: rgba(239,68,68,0.1); color: #ef4444; }

    /* Send Success */
    .send-success {
      padding: 10px 16px;
      background: rgba(16,185,129,0.1);
      border: 1px solid rgba(16,185,129,0.3);
      border-radius: 10px;
      color: #10b981;
      font-weight: 600;
      font-size: 0.85rem;
      animation: fadeIn 0.3s ease;
    }
    @keyframes fadeIn { from { opacity:0; } to { opacity:1; } }

    /* Table */
    .table { color: var(--lpx-text, #f1f5f9); }
    .table thead th { color: #64748b; font-size: 0.78rem; font-weight: 600; text-transform: uppercase; }
  `]
})
export class AdminNotificationsComponent implements OnInit {
  private notifService = inject(NotificationService);

  users: UserNotificationSummaryDto[] = [];
  notifTypes = NOTIFICATION_TYPES;
  loading   = false;
  sending   = false;
  sendSuccess = false;
  searchText  = '';

  sendTarget    = 'all';
  selectedUserId = '';

  sendForm: CreateNotificationDto = {
    title: '',
    message: '',
    type: 'system',
    url: undefined,
  };

  get silencedCount() { return this.users.filter(u => u.globalSilence).length; }

  get filteredUsers() {
    if (!this.searchText) return this.users;
    const q = this.searchText.toLowerCase();
    return this.users.filter(u =>
      u.userName.toLowerCase().includes(q) ||
      (u.email?.toLowerCase().includes(q) ?? false)
    );
  }

  ngOnInit() { this.loadUsers(); }

  loadUsers() {
    this.loading = true;
    this.notifService.getUsersNotificationStatus().subscribe({
      next: users => { this.users = users; this.loading = false; },
      error: () => { this.loading = false; }
    });
  }

  sendNotification() {
    if (!this.sendForm.title || !this.sendForm.message) return;
    this.sending = true;

    const obs = this.sendTarget === 'all'
      ? this.notifService.sendToAll(this.sendForm)
      : this.notifService.sendToUser(this.selectedUserId, this.sendForm);

    obs.subscribe({
      next: () => {
        this.sending = false;
        this.sendSuccess = true;
        this.sendForm = { title: '', message: '', type: 'system' };
        setTimeout(() => this.sendSuccess = false, 3000);
        this.loadUsers(); // refresh counts
      },
      error: () => { this.sending = false; }
    });
  }

  toggleSilence(u: UserNotificationSummaryDto) {
    const dto: SetUserSilenceDto = { isSilenced: !u.globalSilence };
    this.notifService.setUserSilence(u.userId, dto).subscribe(() => {
      u.globalSilence = !u.globalSilence;
    });
  }

  quickSend(u: UserNotificationSummaryDto) {
    this.sendTarget = 'user';
    this.selectedUserId = u.userId;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
