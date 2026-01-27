import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';

interface Payment {
  id: string;
  invoiceId?: string;
  invoiceNumber?: string;
  patientId: string;
  patientName?: string;
  paymentNumber: string;
  paymentDate: string;
  amount: number;
  paymentMethod: number;
  referenceNumber?: string;
  status: number;
  receivedBy?: string;
  notes?: string;
}

interface Lookup {
  id: string;
  name: string;
}

const methodLabels: { [key: number]: string } = {
  0: 'نقدي', 1: 'بطاقة ائتمان', 2: 'مدى', 3: 'تحويل بنكي', 4: 'شيك', 5: 'تأمين', 99: 'أخرى'
};
const statusLabels: { [key: number]: string } = {
  0: 'معلق', 1: 'مكتمل', 2: 'مرفوض', 3: 'مسترد', 4: 'ملغي'
};
const statusColors: { [key: number]: string } = {
  0: 'warning', 1: 'success', 2: 'danger', 3: 'info', 4: 'secondary'
};

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-money-bill-wave me-2"></i>
            المدفوعات - Payments
          </h5>
          <button class="btn btn-primary" (click)="showForm = true; resetForm()">
            <i class="fas fa-plus me-1"></i> تسجيل دفعة
          </button>
        </div>
        <div class="card-body">
          <!-- Filters -->
          <div class="row mb-3">
            <div class="col-md-3">
              <div class="input-group">
                <span class="input-group-text"><i class="fas fa-search"></i></span>
                <input type="text" class="form-control" placeholder="بحث..." 
                       [(ngModel)]="searchText" (input)="search()">
              </div>
            </div>
            <div class="col-md-2">
              <select class="form-select" [(ngModel)]="filterMethod" (change)="search()">
                <option value="">كل الطرق</option>
                <option value="0">نقدي</option>
                <option value="1">بطاقة ائتمان</option>
                <option value="2">مدى</option>
                <option value="3">تحويل بنكي</option>
              </select>
            </div>
            <div class="col-md-2">
              <input type="date" class="form-control" [(ngModel)]="filterFromDate" (change)="search()">
            </div>
            <div class="col-md-2">
              <input type="date" class="form-control" [(ngModel)]="filterToDate" (change)="search()">
            </div>
          </div>

          <!-- Summary -->
          <div class="alert alert-info mb-3">
            <strong>إجمالي المدفوعات:</strong> {{ totalAmount | number:'1.2-2' }} جنيه
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-striped table-hover">
              <thead class="table-dark">
                <tr>
                  <th>رقم الدفعة</th>
                  <th>التاريخ</th>
                  <th>المريض</th>
                  <th>المبلغ</th>
                  <th>طريقة الدفع</th>
                  <th>المرجع</th>
                  <th>الحالة</th>
                  <th>الإجراءات</th>
                </tr>
              </thead>
              <tbody>
                @for (item of items; track item.id) {
                  <tr>
                    <td><code>{{ item.paymentNumber }}</code></td>
                    <td>{{ item.paymentDate | date:'yyyy-MM-dd HH:mm' }}</td>
                    <td>{{ item.patientName || '-' }}</td>
                    <td><strong>{{ item.amount | number:'1.2-2' }}</strong></td>
                    <td>{{ getMethodLabel(item.paymentMethod) }}</td>
                    <td>{{ item.referenceNumber || '-' }}</td>
                    <td>
                      <span [class]="'badge bg-' + getStatusColor(item.status)">
                        {{ getStatusLabel(item.status) }}
                      </span>
                    </td>
                    <td>
                      <button class="btn btn-sm btn-outline-info me-1" title="طباعة">
                        <i class="fas fa-print"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-secondary" (click)="viewDetails(item)">
                        <i class="fas fa-eye"></i>
                      </button>
                    </td>
                  </tr>
                } @empty {
                  <tr>
                    <td colspan="8" class="text-center text-muted py-4">لا توجد مدفوعات</td>
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

      <!-- New Payment Modal -->
      @if (showForm) {
        <div class="modal show d-block" style="background: rgba(0,0,0,0.5)">
          <div class="modal-dialog">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">تسجيل دفعة جديدة</h5>
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
                    <label class="form-label">المبلغ *</label>
                    <input type="number" class="form-control" [(ngModel)]="formData.amount" min="0" required>
                  </div>
                  <div class="col-md-6 mb-3">
                    <label class="form-label">طريقة الدفع</label>
                    <select class="form-select" [(ngModel)]="formData.paymentMethod">
                      <option [value]="0">نقدي</option>
                      <option [value]="1">بطاقة ائتمان</option>
                      <option [value]="2">مدى</option>
                      <option [value]="3">تحويل بنكي</option>
                    </select>
                  </div>
                </div>

                <div class="mb-3" *ngIf="formData.paymentMethod !== 0">
                  <label class="form-label">رقم المرجع</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.referenceNumber">
                </div>

                <div class="mb-3">
                  <label class="form-label">ملاحظات</label>
                  <textarea class="form-control" rows="2" [(ngModel)]="formData.notes"></textarea>
                </div>
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-secondary" (click)="showForm = false">إلغاء</button>
                <button type="button" class="btn btn-success" (click)="save()">
                  <i class="fas fa-check me-1"></i> تسجيل الدفعة
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
export class PaymentsComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url + '/api/app/payment';

  items: Payment[] = [];
  patients: Lookup[] = [];

  searchText = '';
  filterMethod = '';
  filterFromDate = '';
  filterToDate = '';
  showForm = false;
  formData: any = this.getEmptyForm();
  totalAmount = 0;

  page = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit() {
    this.loadPatients();
    this.loadData();
  }

  getEmptyForm() {
    return { patientId: '', amount: 0, paymentMethod: 0, referenceNumber: '', notes: '' };
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
    if (this.filterMethod) url += `&paymentMethod=${this.filterMethod}`;
    if (this.filterFromDate) url += `&fromDate=${this.filterFromDate}`;
    if (this.filterToDate) url += `&toDate=${this.filterToDate}`;

    this.http.get<any>(url).subscribe({
      next: (res) => {
        this.items = res.items || [];
        this.totalCount = res.totalCount || 0;
        this.totalAmount = this.items.reduce((sum, p) => sum + p.amount, 0);
      },
      error: (err) => console.error(err)
    });
  }

  onPageChange(page: number) {
    this.page = page;
    this.loadData();
  }

  search() {
    this.page = 1;
    this.loadData();
  }

  getMethodLabel(method: number): string { return methodLabels[method] || '-'; }
  getStatusLabel(status: number): string { return statusLabels[status] || '-'; }
  getStatusColor(status: number): string { return statusColors[status] || 'secondary'; }

  viewDetails(item: Payment) {
    alert(`رقم الدفعة: ${item.paymentNumber}\nالمبلغ: ${item.amount}\nالملاحظات: ${item.notes || 'لا يوجد'}`);
  }

  save() {
    this.http.post(this.apiUrl, this.formData).subscribe({
      next: () => { this.showForm = false; this.loadData(); alert('تم تسجيل الدفعة بنجاح!'); },
      error: (err) => console.error(err)
    });
  }
}
