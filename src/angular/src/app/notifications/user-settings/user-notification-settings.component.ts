import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NotificationType, NOTIFICATION_TYPES, UserNotificationSettingsDto } from '../models/notification.model';
import { NotificationService } from '../services/notification.service';

@Component({
  selector: 'app-user-notification-settings',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  template: `
    <div class="settings-page container-fluid py-4" dir="rtl">
      <div class="settings-card">
        <!-- Header -->
        <div class="settings-header">
          <div class="settings-header-icon">
            <i class="fas fa-bell-slash"></i>
          </div>
          <div>
            <h5 class="mb-1">إعدادات التنبيهات</h5>
            <p class="mb-0 text-muted small">اختر أنواع التنبيهات التي تريد استلامها</p>
          </div>
        </div>

        <div class="settings-body" *ngIf="settings">
          <!-- Admin Silence Notice -->
          <div class="alert alert-warning d-flex align-items-center gap-3 mb-4"
               *ngIf="settings.globalSilence">
            <i class="fas fa-volume-mute fa-2x text-warning"></i>
            <div>
              <strong>وضع الصمت مفعّل</strong><br>
              <small>تم تعطيل التنبيهات من قبل المسؤول
                <span *ngIf="settings.silencedUntil">
                  حتى {{ settings.silencedUntil | date:'yyyy-MM-dd HH:mm' }}
                </span>
              </small>
            </div>
          </div>

          <!-- Notification Types -->
          <h6 class="section-title">أنواع التنبيهات</h6>
          <div class="types-grid">
            <div class="type-card"
                 *ngFor="let t of notifTypes"
                 [class.active]="isTypeEnabled(t.key)"
                 (click)="toggleType(t.key)">
              <div class="type-icon" [style.background]="t.color + '22'">
                <i [class]="t.icon" [style.color]="t.color"></i>
              </div>
              <div class="type-label">{{ t.label }}</div>
              <div class="type-toggle">
                <div class="toggle-switch" [class.on]="isTypeEnabled(t.key)">
                  <div class="toggle-thumb"></div>
                </div>
              </div>
            </div>
          </div>

          <!-- Actions -->
          <div class="settings-actions">
            <button class="btn btn-primary px-5" (click)="save()" [disabled]="saving">
              <i class="fas fa-save me-2"></i>
              {{ saving ? 'جاري الحفظ...' : 'حفظ الإعدادات' }}
            </button>
            <button class="btn btn-outline-secondary" (click)="enableAll()">
              <i class="fas fa-check-double me-1"></i>تفعيل الكل
            </button>
            <button class="btn btn-outline-secondary" (click)="disableAll()">
              <i class="fas fa-times me-1"></i>تعطيل الكل
            </button>
          </div>

          <!-- Success Toast -->
          <div class="success-toast" *ngIf="saved">
            <i class="fas fa-check-circle me-2"></i>تم حفظ الإعدادات بنجاح
          </div>
        </div>

        <div class="text-center p-5" *ngIf="!settings">
          <div class="spinner-border text-primary"></div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host { direction: rtl; display: block; }

    .settings-card {
      background: var(--lpx-card-bg, #1e293b);
      border-radius: 20px;
      border: 1px solid rgba(255,255,255,0.06);
      overflow: hidden;
      max-width: 700px;
      margin: 0 auto;
    }

    .settings-header {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 24px 28px;
      background: linear-gradient(135deg, #4f46e5 0%, #7c3aed 100%);
      color: #fff;
    }

    .settings-header-icon {
      width: 52px;
      height: 52px;
      border-radius: 14px;
      background: rgba(255,255,255,0.15);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.4rem;
    }

    .settings-body { padding: 28px; }

    .section-title {
      font-weight: 700;
      color: var(--lpx-text, #f1f5f9);
      margin-bottom: 16px;
      padding-bottom: 8px;
      border-bottom: 1px solid rgba(255,255,255,0.06);
    }

    /* Types Grid */
    .types-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
      gap: 12px;
      margin-bottom: 28px;
    }

    .type-card {
      background: rgba(255,255,255,0.03);
      border: 1px solid rgba(255,255,255,0.06);
      border-radius: 14px;
      padding: 16px;
      cursor: pointer;
      transition: all 0.2s ease;
      text-align: center;
    }

    .type-card:hover { background: rgba(79,70,229,0.08); border-color: rgba(79,70,229,0.3); }

    .type-card.active {
      background: rgba(79,70,229,0.1);
      border-color: #4f46e5;
    }

    .type-icon {
      width: 44px;
      height: 44px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      margin: 0 auto 10px;
      font-size: 1.1rem;
      transition: transform 0.2s;
    }

    .type-card:hover .type-icon { transform: scale(1.1); }

    .type-label {
      font-size: 0.82rem;
      font-weight: 600;
      color: var(--lpx-text, #f1f5f9);
      margin-bottom: 10px;
    }

    /* Toggle Switch */
    .toggle-switch {
      width: 38px;
      height: 20px;
      border-radius: 99px;
      background: #374151;
      position: relative;
      margin: 0 auto;
      transition: background 0.2s;
      cursor: pointer;
    }

    .toggle-switch.on { background: #4f46e5; }

    .toggle-thumb {
      position: absolute;
      top: 2px;
      right: 2px;
      width: 16px;
      height: 16px;
      border-radius: 50%;
      background: #fff;
      transition: transform 0.2s;
    }

    .toggle-switch.on .toggle-thumb { transform: translateX(-18px); }

    /* Actions */
    .settings-actions {
      display: flex;
      gap: 12px;
      flex-wrap: wrap;
      align-items: center;
    }

    /* Success Toast */
    .success-toast {
      margin-top: 16px;
      padding: 12px 20px;
      background: rgba(16,185,129,0.1);
      border: 1px solid rgba(16,185,129,0.3);
      border-radius: 10px;
      color: #10b981;
      font-weight: 600;
      animation: fadeIn 0.3s ease;
    }

    @keyframes fadeIn { from { opacity:0; transform:translateY(4px); } to { opacity:1; transform:translateY(0); } }
  `]
})
export class UserNotificationSettingsComponent implements OnInit {
  private notifService = inject(NotificationService);

  settings: UserNotificationSettingsDto | null = null;
  enabledTypes = new Set<NotificationType>();
  notifTypes = NOTIFICATION_TYPES;
  saving = false;
  saved  = false;

  ngOnInit() { this.load(); }

  load() {
    this.notifService.getMySettings().subscribe(s => {
      this.settings = s;
      this.enabledTypes = new Set(s.enabledTypes);
    });
  }

  isTypeEnabled(key: NotificationType) { return this.enabledTypes.has(key); }

  toggleType(key: NotificationType) {
    if (this.enabledTypes.has(key)) this.enabledTypes.delete(key);
    else this.enabledTypes.add(key);
  }

  enableAll()  { this.notifTypes.forEach(t => this.enabledTypes.add(t.key)); }
  disableAll() { this.enabledTypes.clear(); }

  save() {
    this.saving = true;
    this.notifService.updateMySettings({ enabledTypes: [...this.enabledTypes] }).subscribe({
      next: () => {
        this.saving = false;
        this.saved  = true;
        setTimeout(() => this.saved = false, 3000);
      },
      error: () => { this.saving = false; }
    });
  }
}
