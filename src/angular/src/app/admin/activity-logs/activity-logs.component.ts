import { Component, OnInit, inject, ViewChild, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbPaginationModule, NgbModalModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ActivityLogService } from '../../proxy/activity-logs/activity-log.service';

@Component({
  selector: 'app-activity-logs',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule, NgbModalModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card shadow-sm">
        <div class="card-header bg-gradient text-white" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);">
          <h5 class="mb-0">
            <i class="fas fa-history me-2"></i>
            سجل النشاطات - Activity Log
          </h5>
        </div>
        <div class="card-body">
          <!-- Filters -->
          <div class="row mb-4 g-2">
            <div class="col-md-3">
              <div class="input-group">
                <span class="input-group-text"><i class="fas fa-search"></i></span>
                <input type="text" class="form-control" placeholder="بحث..." 
                       [(ngModel)]="filter.searchText" (input)="search()">
              </div>
            </div>
            <div class="col-md-2">
              <select class="form-select" [(ngModel)]="filter.module" (change)="search()">
                <option value="">كل الوحدات</option>
                <option *ngFor="let m of modules" [value]="m">{{ m }}</option>
              </select>
            </div>
            <div class="col-md-2">
              <input type="date" class="form-control" placeholder="من" [(ngModel)]="filter.startDate" (change)="search()">
            </div>
            <div class="col-md-2">
              <input type="date" class="form-control" placeholder="إلى" [(ngModel)]="filter.endDate" (change)="search()">
            </div>
            <div class="col-md-2">
              <button class="btn btn-outline-secondary w-100" (click)="resetFilters()">
                <i class="fas fa-redo me-1"></i> إعادة تعيين
              </button>
            </div>
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-hover align-middle">
              <thead class="table-light">
                <tr>
                  <th style="width: 140px;">التاريخ</th>
                  <th>المستخدم</th>
                  <th>الإجراء</th>
                  <th>الوحدة</th>
                  <th>الوصف</th>
                  <th>الجهاز</th>
                  <th>الموقع</th>
                  <th style="width: 80px;">التفاصيل</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let item of items" class="activity-row">
                  <td>
                    <small class="text-muted">{{ item.timestamp | date:'yyyy-MM-dd' }}</small><br>
                    <small class="fw-bold">{{ item.timestamp | date:'HH:mm:ss' }}</small>
                  </td>
                  <td>
                    <span class="fw-bold">{{ item.userName || '-' }}</span>
                    <br><small class="text-muted">{{ item.ipAddress || '-' }}</small>
                  </td>
                  <td>
                    <span [class]="'badge ' + getActionClass(item.action)">
                      <i [class]="getActionIcon(item.action) + ' me-1'"></i>
                      {{ getActionLabel(item.action) }}
                    </span>
                  </td>
                  <td><span class="badge bg-secondary">{{ item.module }}</span></td>
                  <td>
                    <span [title]="item.description">{{ item.description | slice:0:40 }}{{ item.description?.length > 40 ? '...' : '' }}</span>
                  </td>
                  <td>
                    <div class="d-flex align-items-center">
                      <i [class]="getDeviceIcon(item.deviceType) + ' me-2 fs-5'"></i>
                      <div>
                        <small class="d-block">{{ item.browserName || '-' }} {{ item.browserVersion }}</small>
                        <small class="text-muted">{{ item.operatingSystem || '-' }}</small>
                      </div>
                    </div>
                  </td>
                  <td>
                    <span *ngIf="item.country">
                      <i class="fas fa-map-marker-alt me-1 text-danger"></i>
                      {{ item.city ? item.city + ', ' : '' }}{{ item.country }}
                    </span>
                    <span *ngIf="!item.country" class="text-muted">-</span>
                  </td>
                  <td>
                    <button class="btn btn-sm btn-outline-primary" (click)="showDetails(item)" 
                            *ngIf="item.oldValues || item.newValues">
                      <i class="fas fa-eye"></i>
                    </button>
                  </td>
                </tr>
                <tr *ngIf="items.length === 0">
                  <td colspan="8" class="text-center text-muted py-5">
                    <i class="fas fa-folder-open fs-1 d-block mb-3"></i>
                    لا توجد سجلات
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Pagination -->
          <div class="d-flex justify-content-between align-items-center mt-3" *ngIf="totalCount > 0">
            <ngb-pagination
              [(page)]="page"
              [pageSize]="pageSize"
              [collectionSize]="totalCount"
              (pageChange)="onPageChange($event)"
              [maxSize]="5"
              [boundaryLinks]="true">
            </ngb-pagination>
            <span class="badge bg-secondary fs-6">الإجمالي: {{ totalCount }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Details Modal -->
    <ng-template #detailsModal let-modal>
      <div class="modal-header bg-primary text-white">
        <h5 class="modal-title"><i class="fas fa-info-circle me-2"></i>تفاصيل النشاط</h5>
        <button type="button" class="btn-close btn-close-white" (click)="modal.dismiss()"></button>
      </div>
      <div class="modal-body" *ngIf="selectedItem">
        <div class="row mb-3">
          <div class="col-6">
            <strong>المستخدم:</strong> {{ selectedItem.userName }}
          </div>
          <div class="col-6">
            <strong>التاريخ:</strong> {{ selectedItem.timestamp | date:'yyyy-MM-dd HH:mm:ss' }}
          </div>
        </div>
        
        <div class="row mb-3" *ngIf="selectedItem.oldValues">
          <div class="col-12">
            <div class="card border-danger">
              <div class="card-header bg-danger text-white py-2">
                <i class="fas fa-minus-circle me-2"></i>القيم القديمة
              </div>
              <div class="card-body">
                <pre class="mb-0" style="white-space: pre-wrap; max-height: 200px; overflow: auto;">{{ formatJson(selectedItem.oldValues) }}</pre>
              </div>
            </div>
          </div>
        </div>
        
        <div class="row" *ngIf="selectedItem.newValues">
          <div class="col-12">
            <div class="card border-success">
              <div class="card-header bg-success text-white py-2">
                <i class="fas fa-plus-circle me-2"></i>القيم الجديدة
              </div>
              <div class="card-body">
                <pre class="mb-0" style="white-space: pre-wrap; max-height: 200px; overflow: auto;">{{ formatJson(selectedItem.newValues) }}</pre>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" (click)="modal.close()">إغلاق</button>
      </div>
    </ng-template>
  `,
  styles: [`
    .activity-row:hover { background-color: #f8f9fa; }
    .bg-gradient { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); }
  `]
})
export class ActivityLogsComponent implements OnInit {
  private activityLogService = inject(ActivityLogService);
  private modalService = inject(NgbModal);

  items: any[] = [];
  modules: string[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 15;
  selectedItem: any = null;
  @ViewChild('detailsModal') detailsModalRef!: TemplateRef<any>;

  filter: any = {
    searchText: '',
    module: '',
    startDate: '',
    endDate: ''
  };

  ngOnInit() {
    this.loadModules();
    this.loadData();
  }

  loadModules() {
    this.activityLogService.getModules().subscribe({
      next: (res) => this.modules = res,
      error: (err) => console.error(err)
    });
  }

  loadData() {
    const skipCount = (this.page - 1) * this.pageSize;
    this.activityLogService.getList({
      searchText: this.filter.searchText,
      module: this.filter.module || undefined,
      startDate: this.filter.startDate || undefined,
      endDate: this.filter.endDate || undefined,
      skipCount,
      maxResultCount: this.pageSize
    }).subscribe({
      next: (res) => {
        this.items = res.items || [];
        this.totalCount = res.totalCount || 0;
      },
      error: (err) => console.error(err)
    });
  }

  search() {
    this.page = 1;
    this.loadData();
  }

  onPageChange(page: number) {
    this.page = page;
    this.loadData();
  }

  resetFilters() {
    this.filter = { searchText: '', module: '', startDate: '', endDate: '' };
    this.search();
  }

  showDetails(item: any) {
    this.selectedItem = item;
    this.modalService.open(this.detailsModalRef, { size: 'lg' });
  }

  formatJson(jsonString: string): string {
    try {
      return JSON.stringify(JSON.parse(jsonString), null, 2);
    } catch {
      return jsonString;
    }
  }

  getActionLabel(action: number): string {
    const actions: { [key: number]: string } = {
      0: 'دخول', 1: 'خروج', 2: 'إنشاء', 3: 'تعديل', 4: 'حذف',
      5: 'عرض', 6: 'تصدير', 7: 'استيراد', 8: 'رفض الوصول', 9: 'فشل الدخول'
    };
    return actions[action] || action.toString();
  }

  getActionClass(action: number): string {
    const classes: { [key: number]: string } = {
      0: 'bg-success', 1: 'bg-secondary', 2: 'bg-success', 3: 'bg-primary',
      4: 'bg-danger', 5: 'bg-info', 6: 'bg-warning text-dark', 8: 'bg-danger'
    };
    return classes[action] || 'bg-secondary';
  }

  getActionIcon(action: number): string {
    const icons: { [key: number]: string } = {
      0: 'fas fa-sign-in-alt', 1: 'fas fa-sign-out-alt', 2: 'fas fa-plus',
      3: 'fas fa-edit', 4: 'fas fa-trash', 5: 'fas fa-eye', 6: 'fas fa-download'
    };
    return icons[action] || 'fas fa-circle';
  }

  getDeviceIcon(deviceType: string): string {
    if (!deviceType) return 'fas fa-desktop text-muted';
    const dt = deviceType.toLowerCase();
    if (dt === 'mobile') return 'fas fa-mobile-alt text-primary';
    if (dt === 'tablet') return 'fas fa-tablet-alt text-info';
    return 'fas fa-desktop text-secondary';
  }

  getLevelLabel(level: number): string {
    const levels: { [key: number]: string } = {
      0: 'معلومات', 1: 'تحذير', 2: 'خطأ', 3: 'حرج', 4: 'تدقيق'
    };
    return levels[level] || level.toString();
  }
}
