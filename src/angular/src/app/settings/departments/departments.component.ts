import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { environment } from '../../../environments/environment';

interface Department {
  id: string;
  code: string;
  nameAr: string;
  nameEn?: string;
  description?: string;
  location?: string;
  extensionNumber?: string;
  isActive: boolean;
  sortOrder: number;
  costCenterId?: string;
}

import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-departments',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule, ThemeSharedModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-building me-2"></i>
            الأقسام - Departments
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
                  <th>الموقع</th>
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
                    <td>{{ item.location }}</td>
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
                    <td colspan="6" class="text-center text-muted py-4">لا توجد بيانات</td>
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
            <span class="text-muted">Total: {{ totalCount }}</span>
          </div>

        </div>
      </div>


      <!-- Modal Form -->
      @if (showForm) {
        <div class="modal show d-block" style="background: rgba(0,0,0,0.5)">
          <div class="modal-dialog">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">{{ editingItem ? 'تعديل' : 'إضافة' }} قسم</h5>
                <button type="button" class="btn-close" (click)="showForm = false"></button>
              </div>
              <div class="modal-body">

                <div class="mb-3">
                  <label class="form-label">الاسم (عربي) *</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.nameAr" required>
                </div>
                <div class="mb-3">
                  <label class="form-label">الاسم (إنجليزي)</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.nameEn">
                </div>
                <div class="mb-3">
                  <label class="form-label">الوصف</label>
                  <textarea class="form-control" [(ngModel)]="formData.description"></textarea>
                </div>
                <div class="mb-3">
                  <label class="form-label">الموقع</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.location">
                </div>
                <div class="mb-3">
                  <label class="form-label">رقم التحويلة</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.extensionNumber">
                </div>
                <div class="mb-3">
                  <label class="form-label">الترتيب</label>
                  <input type="number" class="form-control" [(ngModel)]="formData.sortOrder">
                </div>
                <div class="mb-3">
                  <label class="form-label">مركز التكلفة (الحساب)</label>
                  <select class="form-select" [(ngModel)]="formData.costCenterId">
                    <option [ngValue]="null">-- اختر مركز التكلفة --</option>
                    @for (acc of accounts; track acc.id) {
                      <option [ngValue]="acc.id">{{ acc.code }} - {{ acc.name }}</option>
                    }
                  </select>
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
  styles: [`
    .modal { z-index: 1050; }
    .table th, .table td { vertical-align: middle; }
  `]
})
export class DepartmentsComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url + '/api/app/department';
  private confirmation = inject(ConfirmationService);

  items: Department[] = [];
  accounts: any[] = [];
  searchText = '';
  showForm = false;
  editingItem: Department | null = null;
  formData: Partial<Department> = this.getEmptyForm();

  // Pagination
  page = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit() {
    this.loadData();
    this.loadAccounts();
  }

  getEmptyForm(): Partial<Department> {
    return { code: '', nameAr: '', nameEn: '', description: '', location: '', extensionNumber: '', isActive: true, sortOrder: 0, costCenterId: null };
  }

  resetForm() {
    this.formData = this.getEmptyForm();
  }

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

  loadAccounts() {
    this.http.get<any>(environment.apis.default.url + '/api/app/account?maxResultCount=1000').subscribe({
      next: (res) => {
        this.accounts = res.items || [];
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

  edit(item: Department) {
    this.editingItem = item;
    this.formData = { ...item };
    this.showForm = true;
  }

  save() {
    if (this.editingItem) {
      this.http.put(`${this.apiUrl}/${this.editingItem.id}`, this.formData).subscribe({
        next: () => { this.showForm = false; this.loadData(); },
        error: (err) => console.error(err)
      });
    } else {
      this.http.post(this.apiUrl, this.formData).subscribe({
        next: () => { this.showForm = false; this.loadData(); },
        error: (err) => console.error(err)
      });
    }
  }

  delete(id: string) {
    this.confirmation.warn('هل أنت متأكد من الحذف؟', 'تأكيد الحذف').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.http.delete(`${this.apiUrl}/${id}`).subscribe({
          next: () => this.loadData(),
          error: (err) => console.error(err)
        });
      }
    });
  }
}
