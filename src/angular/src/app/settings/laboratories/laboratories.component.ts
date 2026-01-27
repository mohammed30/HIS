import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';

interface Laboratory {
  id: string;
  code: string;
  nameAr: string;
  nameEn?: string;
  description?: string;
  location?: string;
  departmentId?: string;
  isActive: boolean;
  sortOrder: number;
}
interface Lookup {
  id: string;
  name: string;
}

@Component({
  selector: 'app-laboratories',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-flask me-2"></i>
            المعامل - Laboratories
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
                  <th>القسم</th>
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
                     <td>{{ getDepartmentName(item.departmentId) }}</td>
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
                <h5 class="modal-title">{{ editingItem ? 'تعديل' : 'إضافة' }} معمل</h5>
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
                    <label class="form-label">القسم</label>
                    <select class="form-select" [(ngModel)]="formData.departmentId">
                      <option value="">اختر القسم</option>
                      @for (dept of departments; track dept.id) {
                        <option [value]="dept.id">{{ dept.name }}</option>
                      }
                    </select>
                  </div>
                <div class="mb-3">
                  <label class="form-label">الموقع</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.location">
                </div>
                <div class="mb-3">
                  <label class="form-label">الوصف</label>
                  <textarea class="form-control" [(ngModel)]="formData.description"></textarea>
                </div>
                <div class="mb-3">
                  <label class="form-label">الترتيب</label>
                  <input type="number" class="form-control" [(ngModel)]="formData.sortOrder">
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
export class LaboratoriesComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url + '/api/app/laboratory';

  items: Laboratory[] = [];
  departments: Lookup[] = [];
  searchText = '';
  showForm = false;
  editingItem: Laboratory | null = null;
  formData: Partial<Laboratory> = this.getEmptyForm();

  // Pagination
  page = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit() {
    this.loadData();
    this.loadDepartments();
  }

  getEmptyForm(): Partial<Laboratory> {
    return { code: '', nameAr: '', nameEn: '', description: '', location: '', departmentId: '', isActive: true, sortOrder: 0 };
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

  onPageChange(page: number) {
    this.page = page;
    this.loadData();
  }

  loadDepartments() {
    this.http.get<Lookup[]>(environment.apis.default.url + '/api/app/department/lookup').subscribe({
      next: (res) => this.departments = res,
      error: (err) => console.error(err)
    });
  }

  getDepartmentName(id?: string): string {
    return this.departments.find(d => d.id === id)?.name || '-';
  }

  search() {
    this.page = 1;
    this.loadData();
  }

  edit(item: Laboratory) {
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

  delete(item: Laboratory) {
    if (confirm('هل أنت متأكد من الحذف؟')) {
      this.http.delete(`${this.apiUrl}/${item.id}`).subscribe({
        next: () => this.loadData(),
        error: (err) => console.error(err)
      });
    }
  }
}
