import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';

interface InsuranceCompany {
    id: string;
    code: string;
    nameAr: string;
    nameEn?: string;
    phone?: string;
    email?: string;
    address?: string;
    contactPerson?: string;
    contactPhone?: string;
    website?: string;
    notes?: string;
    isActive: boolean;
    sortOrder: number;
}

@Component({
    selector: 'app-insurance-companies',
    standalone: true,
    imports: [CommonModule, FormsModule, NgbPaginationModule],
    template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-building me-2"></i>
            شركات التأمين - Insurance Companies
          </h5>
          <button class="btn btn-primary" (click)="showForm = true; editingItem = null; resetForm()">
            <i class="fas fa-plus me-1"></i> إضافة
          </button>
        </div>
        <div class="card-body">
          <!-- Search -->
          <div class="row mb-3">
            <div class="col-md-4">
              <div class="input-group">
                <span class="input-group-text"><i class="fas fa-search"></i></span>
                <input type="text" class="form-control" placeholder="بحث..." 
                       [(ngModel)]="searchText" (input)="search()">
              </div>
            </div>
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-striped table-hover">
              <thead class="table-dark">
                <tr>
                  <th>الكود</th>
                  <th>الاسم (عربي)</th>
                  <th>الاسم (إنجليزي)</th>
                  <th>الهاتف</th>
                  <th>البريد</th>
                  <th>الحالة</th>
                  <th>الإجراءات</th>
                </tr>
              </thead>
              <tbody>
                @for (item of items; track item.id) {
                  <tr>
                    <td>{{ item.code }}</td>
                    <td>{{ item.nameAr }}</td>
                    <td>{{ item.nameEn }}</td>
                    <td>{{ item.phone }}</td>
                    <td>{{ item.email }}</td>
                    <td>
                      <span [class]="item.isActive ? 'badge bg-success' : 'badge bg-secondary'">
                        {{ item.isActive ? 'نشط' : 'غير نشط' }}
                      </span>
                    </td>
                    <td>
                      <button class="btn btn-sm btn-outline-primary me-1" (click)="edit(item)">
                        <i class="fas fa-edit"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-danger" (click)="delete(item)">
                        <i class="fas fa-trash"></i>
                      </button>
                    </td>
                  </tr>
                } @empty {
                  <tr>
                    <td colspan="7" class="text-center text-muted py-4">لا توجد بيانات</td>
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

      <!-- Modal Form -->
      @if (showForm) {
        <div class="modal show d-block" style="background: rgba(0,0,0,0.5)">
          <div class="modal-dialog modal-lg">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">{{ editingItem ? 'تعديل' : 'إضافة' }} شركة تأمين</h5>
                <button type="button" class="btn-close" (click)="showForm = false"></button>
              </div>
              <div class="modal-body">
                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label class="form-label">الاسم بالعربية *</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.nameAr" required>
                  </div>
                  <div class="col-md-6 mb-3">
                    <label class="form-label">الاسم بالإنجليزية</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.nameEn">
                  </div>
                </div>
                
                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label class="form-label">الهاتف</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.phone">
                  </div>
                  <div class="col-md-6 mb-3">
                    <label class="form-label">البريد الإلكتروني</label>
                    <input type="email" class="form-control" [(ngModel)]="formData.email">
                  </div>
                </div>

                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label class="form-label">جهة الاتصال</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.contactPerson">
                  </div>
                  <div class="col-md-6 mb-3">
                    <label class="form-label">هاتف جهة الاتصال</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.contactPhone">
                  </div>
                </div>

                <div class="mb-3">
                  <label class="form-label">العنوان</label>
                  <textarea class="form-control" rows="2" [(ngModel)]="formData.address"></textarea>
                </div>

                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label class="form-label">الموقع الإلكتروني</label>
                    <input type="url" class="form-control" [(ngModel)]="formData.website">
                  </div>
                  <div class="col-md-6 mb-3">
                    <label class="form-label">الترتيب</label>
                    <input type="number" class="form-control" [(ngModel)]="formData.sortOrder">
                  </div>
                </div>

                <div class="mb-3">
                  <label class="form-label">ملاحظات</label>
                  <textarea class="form-control" rows="2" [(ngModel)]="formData.notes"></textarea>
                </div>

                <div class="form-check mb-3">
                  <input type="checkbox" class="form-check-input" [(ngModel)]="formData.isActive" id="isActive">
                  <label class="form-check-label" for="isActive">نشط</label>
                </div>
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-secondary" (click)="showForm = false">إلغاء</button>
                <button type="button" class="btn btn-primary" (click)="save()">
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
export class InsuranceCompaniesComponent implements OnInit {
    private http = inject(HttpClient);
    private apiUrl = environment.apis.default.url + '/api/app/insurance-company';

    items: InsuranceCompany[] = [];
    searchText = '';
    showForm = false;
    editingItem: InsuranceCompany | null = null;
    formData: Partial<InsuranceCompany> = this.getEmptyForm();

    page = 1;
    pageSize = 10;
    totalCount = 0;

    ngOnInit() {
        this.loadData();
    }

    getEmptyForm(): Partial<InsuranceCompany> {
        return { nameAr: '', nameEn: '', phone: '', email: '', address: '', contactPerson: '', contactPhone: '', website: '', notes: '', isActive: true, sortOrder: 0 };
    }

    resetForm() { this.formData = this.getEmptyForm(); }

    loadData() {
        const skipCount = (this.page - 1) * this.pageSize;
        this.http.get<any>(`${this.apiUrl}?searchText=${this.searchText}&skipCount=${skipCount}&maxResultCount=${this.pageSize}`).subscribe({
            next: (res) => {
                this.items = res.items || [];
                this.totalCount = res.totalCount || 0;
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

    edit(item: InsuranceCompany) {
        this.editingItem = item;
        this.formData = { ...item };
        this.showForm = true;
    }

    save() {
        const req = this.editingItem
            ? this.http.put(`${this.apiUrl}/${this.editingItem.id}`, this.formData)
            : this.http.post(this.apiUrl, this.formData);
        req.subscribe({
            next: () => { this.showForm = false; this.loadData(); },
            error: (err) => console.error(err)
        });
    }

    delete(item: InsuranceCompany) {
        if (confirm('هل أنت متأكد من الحذف؟')) {
            this.http.delete(`${this.apiUrl}/${item.id}`).subscribe({ next: () => this.loadData() });
        }
    }
}
