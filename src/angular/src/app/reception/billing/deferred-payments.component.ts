import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';

interface DeferredPayment {
  id: string;
  patientId: string;
  patientName?: string;
  invoiceId?: string;
  invoiceNumber?: string;
  deferredNumber: string;
  totalAmount: number;
  paidAmount: number;
  remainingAmount: number;
  createdDate: string;
  dueDate: string;
  numberOfInstallments: number;
  installmentAmount: number;
  status: number;
  reason?: string;
  contactPhone?: string;
  notes?: string;
}

interface Lookup {
  id: string;
  name: string;
}

const statusLabels: { [key: number]: string } = {
  0: 'نشط', 1: 'مسدد', 2: 'متأخر', 3: 'معلق', 4: 'ملغي'
};
const statusColors: { [key: number]: string } = {
  0: 'primary', 1: 'success', 2: 'danger', 3: 'warning', 4: 'secondary'
};

@Component({
  selector: 'app-deferred-payments',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-clock me-2"></i>
            المؤجلات - Deferred Payments
          </h5>
          <button class="btn btn-warning" (click)="showForm = true; resetForm()">
            <i class="fas fa-plus me-1"></i> تسجيل مؤجل
          </button>
        </div>
        <div class="card-body">
          <!-- Filters -->
          <div class="row mb-3">
            <div class="col-md-4">
              <div class="input-group">
                <span class="input-group-text"><i class="fas fa-search"></i></span>
                <input type="text" class="form-control" placeholder="بحث..." 
                       [(ngModel)]="searchText" (input)="search()">
              </div>
            </div>
            <div class="col-md-2">
              <select class="form-select" [(ngModel)]="filterStatus" (change)="search()">
                <option value="">كل الحالات</option>
                <option value="0">نشط</option>
                <option value="1">مسدد</option>
                <option value="2">متأخر</option>
              </select>
            </div>
          </div>

          <!-- Summary Cards -->
          <div class="row mb-3">
            <div class="col-md-4">
              <div class="card bg-danger text-white">
                <div class="card-body text-center">
                  <h6>إجمالي المؤجلات</h6>
                  <h4>{{ totalDeferred | number:'1.2-2' }} جنيه</h4>
                </div>
              </div>
            </div>
            <div class="col-md-4">
              <div class="card bg-success text-white">
                <div class="card-body text-center">
                  <h6>المبالغ المدفوعة</h6>
                  <h4>{{ totalPaid | number:'1.2-2' }} جنيه</h4>
                </div>
              </div>
            </div>
            <div class="col-md-4">
              <div class="card bg-warning text-dark">
                <div class="card-body text-center">
                  <h6>المتبقي</h6>
                  <h4>{{ totalRemaining | number:'1.2-2' }} جنيه</h4>
                </div>
              </div>
            </div>
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-striped table-hover">
              <thead class="table-dark">
                <tr>
                  <th>رقم المؤجل</th>
                  <th>المريض</th>
                  <th>المبلغ</th>
                  <th>المدفوع</th>
                  <th>المتبقي</th>
                  <th>تاريخ الاستحقاق</th>
                  <th>الحالة</th>
                  <th>الإجراءات</th>
                </tr>
              </thead>
              <tbody>
                @for (item of items; track item.id) {
                  <tr [class.table-danger]="isOverdue(item)">
                    <td><code>{{ item.deferredNumber }}</code></td>
                    <td>{{ item.patientName || '-' }}</td>
                    <td>{{ item.totalAmount | number:'1.2-2' }}</td>
                    <td class="text-success">{{ item.paidAmount | number:'1.2-2' }}</td>
                    <td class="text-danger fw-bold">{{ item.remainingAmount | number:'1.2-2' }}</td>
                    <td>{{ item.dueDate | date:'yyyy-MM-dd' }}</td>
                    <td>
                      <span [class]="'badge bg-' + getStatusColor(item.status)">
                        {{ getStatusLabel(item.status) }}
                      </span>
                    </td>
                    <td>
                      <button class="btn btn-sm btn-success me-1" (click)="recordPayment(item)" 
                              *ngIf="item.remainingAmount > 0" title="تسجيل دفعة">
                        <i class="fas fa-plus-circle"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-info" (click)="viewDetails(item)" title="التفاصيل">
                        <i class="fas fa-eye"></i>
                      </button>
                    </td>
                  </tr>
                } @empty {
                  <tr>
                    <td colspan="8" class="text-center text-muted py-4">لا توجد مؤجلات</td>
                  </tr>
                }
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
            <span class="text-muted">الإجمالي: {{ totalCount }}</span>
          </div>
        </div>
      </div>

      <!-- New Deferred Modal -->
      @if (showForm) {
        <div class="modal show d-block" style="background: rgba(0,0,0,0.5)">
          <div class="modal-dialog">
            <div class="modal-content">
              <div class="modal-header bg-warning">
                <h5 class="modal-title">تسجيل مؤجل جديد</h5>
                <button type="button" class="btn-close" (click)="showForm = false"></button>
              </div>
              <div class="modal-body">
                <div class="mb-3">
                  <label class="form-label">المريض *</label>
                  <select class="form-select" [(ngModel)]="formData.patientId" required>
                    <option value="">اختر المريض</option>
                    @for (p of patients; track p.id) {
                      <option [value]="p.id">{{ p.name }}</option>
                    }
                  </select>
                </div>
                
                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label class="form-label">المبلغ الإجمالي *</label>
                    <input type="number" class="form-control" [(ngModel)]="formData.totalAmount" min="0" required>
                  </div>
                  <div class="col-md-6 mb-3">
                    <label class="form-label">عدد الأقساط</label>
                    <input type="number" class="form-control" [(ngModel)]="formData.numberOfInstallments" min="1">
                  </div>
                </div>

                <div class="mb-3">
                  <label class="form-label">تاريخ الاستحقاق *</label>
                  <input type="date" class="form-control" [(ngModel)]="formData.dueDate" required>
                </div>

                <div class="mb-3">
                  <label class="form-label">السبب</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.reason">
                </div>

                <div class="mb-3">
                  <label class="form-label">رقم التواصل</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.contactPhone">
                </div>
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-secondary" (click)="showForm = false">إلغاء</button>
                <button type="button" class="btn btn-warning" (click)="save()">
                  <i class="fas fa-save me-1"></i> حفظ
                </button>
              </div>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`.modal { z-index: 1050; }`]
})
export class DeferredPaymentsComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url + '/api/app/deferred-payment';

  items: DeferredPayment[] = [];
  patients: Lookup[] = [];

  searchText = '';
  filterStatus = '';
  showForm = false;
  formData: any = this.getEmptyForm();

  totalDeferred = 0;
  totalPaid = 0;
  totalRemaining = 0;

  page = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit() {
    this.loadPatients();
    this.loadData();
  }

  getEmptyForm() {
    return { patientId: '', totalAmount: 0, numberOfInstallments: 1, dueDate: '', reason: '', contactPhone: '' };
  }

  resetForm() { this.formData = this.getEmptyForm(); }

  loadPatients() {
    this.http.get<any>(environment.apis.default.url + '/api/app/patient?maxResultCount=100').subscribe({
      next: (res) => this.patients = (res.items || []).map((p: any) => ({ id: p.id, name: p.firstNameAr + ' ' + p.lastNameAr })),
      error: (err) => console.error(err)
    });
  }

  loadData() {
    const skipCount = (this.page - 1) * this.pageSize;
    let url = `${this.apiUrl}?searchText=${this.searchText}&skipCount=${skipCount}&maxResultCount=${this.pageSize}`;
    if (this.filterStatus) url += `&status=${this.filterStatus}`;

    this.http.get<any>(url).subscribe({
      next: (res) => {
        this.items = res.items || [];
        this.totalCount = res.totalCount || 0;
        this.calculateTotals();
      },
      error: (err) => console.error(err)
    });
  }

  calculateTotals() {
    this.totalDeferred = this.items.reduce((sum, d) => sum + d.totalAmount, 0);
    this.totalPaid = this.items.reduce((sum, d) => sum + d.paidAmount, 0);
    this.totalRemaining = this.items.reduce((sum, d) => sum + d.remainingAmount, 0);
  }

  onPageChange(page: number) {
    this.page = page;
    this.loadData();
  }

  search() {
    this.page = 1;
    this.loadData();
  }

  isOverdue(item: DeferredPayment): boolean {
    return new Date(item.dueDate) < new Date() && item.remainingAmount > 0;
  }

  getStatusLabel(status: number): string { return statusLabels[status] || '-'; }
  getStatusColor(status: number): string { return statusColors[status] || 'secondary'; }

  viewDetails(item: DeferredPayment) {
    alert(`رقم المؤجل: ${item.deferredNumber}\nالمبلغ: ${item.totalAmount}\nالمتبقي: ${item.remainingAmount}\nالسبب: ${item.reason || 'لا يوجد'}`);
  }

  recordPayment(item: DeferredPayment) {
    const amount = prompt(`تسجيل دفعة للمؤجل ${item.deferredNumber}\nالمتبقي: ${item.remainingAmount}\nأدخل المبلغ:`);
    if (amount && parseFloat(amount) > 0) {
      this.http.post(`${this.apiUrl}/${item.id}/record-payment?amount=${amount}`, {}).subscribe({
        next: () => { this.loadData(); alert('تم تسجيل الدفعة بنجاح!'); },
        error: (err) => console.error(err)
      });
    }
  }

  save() {
    this.http.post(this.apiUrl, this.formData).subscribe({
      next: () => { this.showForm = false; this.loadData(); alert('تم تسجيل المؤجل بنجاح!'); },
      error: (err) => console.error(err)
    });
  }
}
