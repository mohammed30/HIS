import { Component, OnInit, inject } from '@angular/core';
import { ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';

interface Invoice {
  id: string;
  patientId: string;
  patientName?: string;
  invoiceNumber: string;
  invoiceDate: string;
  dueDate?: string;
  totalAmount: number;
  discountAmount: number;
  taxPercentage: number;
  taxAmount: number;
  netAmount: number;
  paidAmount: number;
  dueAmount: number;
  insuranceCoverage: number;
  coPaymentAmount: number;
  status: number;
  notes?: string;
}

const statusLabels: { [key: number]: string } = {
  0: 'مسودة', 1: 'صادرة', 2: 'مدفوعة جزئياً', 3: 'مدفوعة', 4: 'ملغية', 5: 'مؤجلة'
};
const statusColors: { [key: number]: string } = {
  0: 'secondary', 1: 'primary', 2: 'warning', 3: 'success', 4: 'danger', 5: 'info'
};

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-file-invoice me-2"></i>
            الفواتير - Invoices
          </h5>
          <button class="btn btn-primary" (click)="showForm = true; resetForm()">
            <i class="fas fa-plus me-1"></i> فاتورة جديدة
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
              <select class="form-select" [(ngModel)]="filterStatus" (change)="search()">
                <option value="">كل الحالات</option>
                <option value="0">مسودة</option>
                <option value="1">صادرة</option>
                <option value="2">جزئي</option>
                <option value="3">مدفوعة</option>
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
          <div class="row mb-3">
            <div class="col-md-3">
              <div class="card bg-primary text-white">
                <div class="card-body py-2 text-center">
                  <small>إجمالي الفواتير</small>
                  <h5 class="mb-0">{{ summaryTotal | number:'1.2-2' }} ج.م</h5>
                </div>
              </div>
            </div>
            <div class="col-md-3">
              <div class="card bg-success text-white">
                <div class="card-body py-2 text-center">
                  <small>المدفوع</small>
                  <h5 class="mb-0">{{ summaryPaid | number:'1.2-2' }} ج.م</h5>
                </div>
              </div>
            </div>
            <div class="col-md-3">
              <div class="card bg-warning text-dark">
                <div class="card-body py-2 text-center">
                  <small>المتبقي</small>
                  <h5 class="mb-0">{{ summaryDue | number:'1.2-2' }} ج.م</h5>
                </div>
              </div>
            </div>
            <div class="col-md-3">
              <div class="card bg-info text-white">
                <div class="card-body py-2 text-center">
                  <small>التأمين</small>
                  <h5 class="mb-0">{{ summaryInsurance | number:'1.2-2' }} ج.م</h5>
                </div>
              </div>
            </div>
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-striped table-hover">
              <thead class="table-dark">
                <tr>
                  <th>رقم الفاتورة</th>
                  <th>التاريخ</th>
                  <th>المريض</th>
                  <th>المصدر</th>
                  <th>الإجمالي</th>
                  <th>الخصم</th>
                  <th>الصافي</th>
                  <th>المدفوع</th>
                  <th>المتبقي</th>
                  <th>الحالة</th>
                  <th>الإجراءات</th>
                </tr>
              </thead>
              <tbody>
                @for (item of items; track item.id) {
                  <tr>
                    <td><code>{{ item.invoiceNumber }}</code></td>
                    <td>{{ item.invoiceDate | date:'yyyy-MM-dd' }}</td>
                    <td>{{ item.patientName || getPatientName(item.patientId) }}</td>
                    <td>
                      <span class="badge" [ngClass]="getInvoiceSourceClass(item.invoiceNumber)">
                        {{ getInvoiceSource(item.invoiceNumber) }}
                      </span>
                    </td>
                    <td>{{ item.totalAmount | number:'1.2-2' }}</td>
                    <td class="text-muted">{{ item.discountAmount | number:'1.2-2' }}</td>
                    <td><strong>{{ item.netAmount | number:'1.2-2' }}</strong></td>
                    <td class="text-success">{{ item.paidAmount | number:'1.2-2' }}</td>
                    <td class="text-danger fw-bold">{{ item.dueAmount | number:'1.2-2' }}</td>
                    <td>
                      <span [class]="'badge bg-' + getStatusColor(item.status)">
                        {{ getStatusLabel(item.status) }}
                      </span>
                    </td>
                    <td>
                      <button class="btn btn-sm btn-outline-success me-1" *ngIf="item.dueAmount > 0" 
                              (click)="payInvoice(item)" title="دفع">
                        <i class="fas fa-money-bill"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-info me-1" (click)="printInvoice(item)" title="طباعة">
                        <i class="fas fa-print"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-danger" *ngIf="item.status !== 4" 
                              (click)="cancelInvoice(item)" title="إلغاء">
                        <i class="fas fa-ban"></i>
                      </button>

                    </td>
                  </tr>
                } @empty {
                  <tr>
                    <td colspan="11" class="text-center text-muted py-4">لا توجد فواتير</td>
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

      <!-- New Invoice Modal -->
      @if (showForm) {
        <div class="modal show d-block" style="background: rgba(0,0,0,0.5)">
          <div class="modal-dialog modal-lg">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">فاتورة جديدة</h5>
                <button type="button" class="btn-close" (click)="showForm = false"></button>
              </div>
              <div class="modal-body">
                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label class="form-label">المريض *</label>
                    <select class="form-select" [(ngModel)]="formData.patientId" required>
                      <option value="">اختر المريض</option>
                      @for (p of patients; track p.id) {
                        <option [value]="p.id">{{ p.name }}</option>
                      }
                    </select>
                  </div>
                  <div class="col-md-6 mb-3">
                    <label class="form-label">تاريخ الاستحقاق</label>
                    <input type="date" class="form-control" [(ngModel)]="formData.dueDate">
                  </div>
                </div>

                <!-- Invoice Items -->
                <h6 class="mt-3">بنود الفاتورة</h6>
                <div class="table-responsive">
                  <table class="table table-sm">
                    <thead>
                      <tr>
                        <th>الوصف</th>
                        <th style="width:80px">الكمية</th>
                        <th style="width:100px">السعر</th>
                        <th style="width:80px"></th>
                      </tr>
                    </thead>
                    <tbody>
                      @for (item of formData.items; track $index; let i = $index) {
                        <tr>
                          <td><input type="text" class="form-control form-control-sm" [(ngModel)]="item.description"></td>
                          <td><input type="number" class="form-control form-control-sm" [(ngModel)]="item.quantity" min="1"></td>
                          <td><input type="number" class="form-control form-control-sm" [(ngModel)]="item.unitPrice" min="0"></td>
                          <td><button class="btn btn-sm btn-outline-danger" (click)="removeItem(i)"><i class="fas fa-times"></i></button></td>
                        </tr>
                      }
                    </tbody>
                  </table>
                </div>
                <button class="btn btn-sm btn-outline-primary" (click)="addItem()">
                  <i class="fas fa-plus me-1"></i> إضافة بند
                </button>

                <div class="row mt-3">
                  <div class="col-md-4 mb-3">
                    <label class="form-label">الخصم</label>
                    <input type="number" class="form-control" [(ngModel)]="formData.discountAmount" min="0">
                  </div>
                  <div class="col-md-4 mb-3">
                    <label class="form-label">نسبة الضريبة (%)</label>
                    <input type="number" class="form-control" [(ngModel)]="formData.taxPercentage" min="0" max="100">
                  </div>
                </div>

                <div class="mb-3">
                  <label class="form-label">ملاحظات</label>
                  <textarea class="form-control" rows="2" [(ngModel)]="formData.notes"></textarea>
                </div>
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-secondary" (click)="showForm = false">إلغاء</button>
                <button type="button" class="btn btn-primary" (click)="save()">
                  <i class="fas fa-save me-1"></i> حفظ الفاتورة
                </button>
              </div>
            </div>
          </div>
        </div>
      }

      <!-- Payment Modal -->
      @if (showPaymentModal && selectedInvoice) {
        <div class="modal show d-block" style="background: rgba(0,0,0,0.5)">
          <div class="modal-dialog modal-sm modal-dialog-centered">
            <div class="modal-content border-0 shadow-lg">
              <div class="modal-header bg-success text-white">
                <h5 class="modal-title">
                  <i class="fas fa-money-bill-wave me-2"></i>
                  دفع فاتورة
                </h5>
                <button type="button" class="btn-close btn-close-white" (click)="showPaymentModal = false"></button>
              </div>
              <div class="modal-body p-4">
                <div class="text-center mb-3">
                  <span class="text-muted d-block small mb-1">رقم الفاتورة</span>
                  <code class="fs-5">{{ selectedInvoice.invoiceNumber }}</code>
                </div>
                
          <div class="alert border-0 text-center mb-3">
                  <small class="d-block text-muted mb-1">المبلغ المتبقي</small>
                  <h4 class="mb-0">{{ selectedInvoice.dueAmount | number:'1.2-2' }} ج.م</h4>
                </div>

                <div class="mb-3">
                  <label class="form-label fw-bold">أدخل المبلغ للإيداع</label>
                  <div class="input-group input-group-lg">
                    <input type="number" class="form-control text-center fw-bold" 
                           [(ngModel)]="paymentAmount" [max]="selectedInvoice.dueAmount" min="1">
                    <span class="input-group-text bg-light">ج.م</span>
                  </div>
                </div>
              </div>
              <div class="modal-footer border-top-0 pt-0 pb-4 justify-content-center">
                <button type="button" class="btn btn-light px-4" (click)="showPaymentModal = false">إلغاء</button>
                <button type="button" class="btn btn-success px-4" (click)="confirmPayment()" [disabled]="!paymentAmount || paymentAmount <= 0">
                  <i class="fas fa-check me-1"></i> تأكيد الدفع
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
export class InvoicesComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url + '/api/app/invoice';
  private toaster = inject(ToasterService);
  private confirmation = inject(ConfirmationService);

  items: Invoice[] = [];
  patients: { id: string; name: string }[] = [];

  searchText = '';
  filterStatus = '';
  filterFromDate = '';
  filterToDate = '';
  showForm = false;
  formData: any = this.getEmptyForm();

  summaryTotal = 0;
  summaryPaid = 0;
  summaryDue = 0;
  summaryInsurance = 0;

  // Payment Modal
  showPaymentModal = false;
  selectedInvoice: Invoice | null = null;
  paymentAmount = 0;

  page = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit() {
    this.loadPatients();
    this.loadData();
  }

  getEmptyForm() {
    return {
      patientId: '', dueDate: '', discountAmount: 0, taxPercentage: 15, notes: '',
      items: [{ description: '', quantity: 1, unitPrice: 0, serviceType: 0 }]
    };
  }

  resetForm() { this.formData = this.getEmptyForm(); }

  addItem() { this.formData.items.push({ description: '', quantity: 1, unitPrice: 0, serviceType: 0 }); }
  removeItem(index: number) { this.formData.items.splice(index, 1); }

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
    if (this.filterFromDate) url += `&fromDate=${this.filterFromDate}`;
    if (this.filterToDate) url += `&toDate=${this.filterToDate}`;

    this.http.get<any>(url).subscribe({
      next: (res) => {
        this.items = res.items || [];
        this.totalCount = res.totalCount || 0;
        this.calculateSummary();
      },
      error: (err) => console.error(err)
    });
  }

  calculateSummary() {
    this.summaryTotal = this.items.reduce((s, i) => s + i.netAmount, 0);
    this.summaryPaid = this.items.reduce((s, i) => s + i.paidAmount, 0);
    this.summaryDue = this.items.reduce((s, i) => s + i.dueAmount, 0);
    this.summaryInsurance = this.items.reduce((s, i) => s + i.insuranceCoverage, 0);
  }

  getPatientName(id: string): string {
    const p = this.patients.find(x => x.id === id);
    return p ? p.name : '-';
  }

  getInvoiceSource(invoiceNumber: string): string {
    if (!invoiceNumber) return 'غير معروف';
    if (invoiceNumber.includes('-INP-')) return 'التنويم';
    if (invoiceNumber.startsWith('POS-')) return 'الصيدلية';
    return 'الاستقبال';
  }

  getInvoiceSourceClass(invoiceNumber: string): string {
    if (!invoiceNumber) return 'bg-secondary';
    if (invoiceNumber.includes('-INP-')) return 'bg-info';
    if (invoiceNumber.startsWith('POS-')) return 'bg-warning text-dark';
    return 'bg-primary';
  }

  onPageChange(page: number) { this.page = page; this.loadData(); }
  search() { this.page = 1; this.loadData(); }

  getStatusLabel(status: number): string { return statusLabels[status] || '-'; }
  getStatusColor(status: number): string { return statusColors[status] || 'secondary'; }


  printInvoice(item: Invoice) {
    const url = `${environment.apis.default.url}/api/app/billing/generate-doc/${item.id}`;
    this.http.get(url, { responseType: 'blob' }).subscribe({
      next: (blob) => {
        const fileURL = URL.createObjectURL(blob);
        window.open(fileURL, '_blank');
      },
      error: (err) => {
        console.error('Error downloading PDF', err);
        this.toaster.error('حدث خطأ أثناء تحميل ملف الطباعة', 'خطأ');
      }
    });
  }

  payInvoice(item: Invoice) {
    this.selectedInvoice = item;
    this.paymentAmount = item.dueAmount;
    this.showPaymentModal = true;
  }

  confirmPayment() {
    if (this.selectedInvoice && this.paymentAmount > 0) {
      this.http.post(environment.apis.default.url + '/api/app/payment', {
        invoiceId: this.selectedInvoice.id,
        patientId: this.selectedInvoice.patientId,
        amount: this.paymentAmount,
        paymentMethod: 0
      }).subscribe({
        next: () => {
          this.showPaymentModal = false;
          this.loadData();
          this.toaster.success('تم الدفع بنجاح!', 'نجاح');
        },
        error: (err) => {
          console.error(err);
          this.toaster.error('حدث خطأ أثناء تنفيذ عملية الدفع', 'خطأ');
        }
      });
    }
  }

  cancelInvoice(item: Invoice) {
    this.confirmation.warn(
      `هل أنت متأكد من إلغاء الفاتورة رقم ${item.invoiceNumber}؟ سيتم عكس القيود المحاسبية واسترداد المدفوعات.`,
      'تأكيد الإلغاء'
    ).subscribe((status: Confirmation.Status) => {
      if (status === Confirmation.Status.confirm) {
        this.http.post(`${this.apiUrl}/${item.id}/cancel`, {}).subscribe({
          next: () => {
            this.loadData();
            this.toaster.success('تم إلغاء الفاتورة بنجاح!', 'نجاح');
          },
          error: (err) => {
            console.error(err);
            this.toaster.error('حدث خطأ أثناء إلغاء الفاتورة', 'خطأ');
          }
        });
      }
    });
  }

  save() {
    this.http.post(this.apiUrl, this.formData).subscribe({
      next: () => {
        this.showForm = false;
        this.loadData();
        this.toaster.success('تم حفظ الفاتورة!', 'نجاح');
      },
      error: (err) => {
        console.error(err);
        this.toaster.error('حدث خطأ أثناء حفظ الفاتورة', 'خطأ');
      }
    });
  }
}
