import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

interface Doctor {
  id: string;
  code: string;
  nameAr: string;
  nameEn: string;
  specialtyId: string;
  departmentId: string;
  mobile?: string;
  email?: string;
  nationalId?: string;
  isActive: boolean;
}

interface Lookup {
  id: string;
  name: string;
}

@Component({
  selector: 'app-doctors',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-user-md me-2"></i>
            الأطباء - Doctors
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
                  <th>التخصص</th>
                  <th>القسم</th>
                  <th>الجوال</th>
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
                    <td>{{ getSpecialtyName(item.specialtyId) }}</td>
                    <td>{{ getDepartmentName(item.departmentId) }}</td>
                    <td>{{ item.mobile }}</td>
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
                    <td colspan="8" class="text-center text-muted py-4">لا توجد بيانات</td>
                  </tr>
                }
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Modal Form -->
      @if (showForm) {
        <div class="modal show d-block" style="background: rgba(0,0,0,0.5)">
          <div class="modal-dialog modal-lg">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">{{ editingItem ? 'تعديل' : 'إضافة' }} طبيب</h5>
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
                    <label class="form-label">التخصص *</label>
                    <select class="form-select" [(ngModel)]="formData.specialtyId" required>
                      <option value="">اختر التخصص</option>
                      @for (spec of specialties; track spec.id) {
                        <option [value]="spec.id">{{ spec.name }}</option>
                      }
                    </select>
                  </div>
                  <div class="col-md-6 mb-3">
                    <label class="form-label">القسم *</label>
                    <select class="form-select" [(ngModel)]="formData.departmentId" required>
                      <option value="">اختر القسم</option>
                      @for (dept of departments; track dept.id) {
                        <option [value]="dept.id">{{ dept.name }}</option>
                      }
                    </select>
                  </div>
                </div>

                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label class="form-label">الجوال</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.mobile">
                  </div>
                   <div class="col-md-6 mb-3">
                    <label class="form-label">البريد الإلكتروني</label>
                    <input type="email" class="form-control" [(ngModel)]="formData.email">
                  </div>
                </div>
                 <div class="mb-3">
                    <label class="form-label">رقم الهوية</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.nationalId">
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
export class DoctorsComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url + '/api/app/doctor';

  items: Doctor[] = [];
  departments: Lookup[] = [];
  specialties: Lookup[] = [];

  searchText = '';
  showForm = false;
  editingItem: Doctor | null = null;
  formData: Partial<Doctor> = this.getEmptyForm();

  ngOnInit() {
    this.loadData();
    this.loadLookups();
  }

  getEmptyForm(): Partial<Doctor> {
    return { code: '', nameAr: '', nameEn: '', specialtyId: '', departmentId: '', mobile: '', email: '', nationalId: '', isActive: true };
  }

  resetForm() { this.formData = this.getEmptyForm(); }

  loadData() {
    this.http.get<any>(`${this.apiUrl}?searchText=${this.searchText}`).subscribe({
      next: (res) => this.items = res.items || [],
      error: (err) => console.error(err)
    });
  }

  loadLookups() {
    this.http.get<Lookup[]>(environment.apis.default.url + '/api/app/department/lookup').subscribe({
      next: (res) => this.departments = res,
      error: (err) => console.error(err)
    });
    this.http.get<Lookup[]>(environment.apis.default.url + '/api/app/specialty/lookup').subscribe({
      next: (res) => this.specialties = res,
      error: (err) => console.error(err)
    });
  }

  getDepartmentName(id?: string): string {
    return this.departments.find(d => d.id === id)?.name || '-';
  }
  getSpecialtyName(id?: string): string {
    return this.specialties.find(s => s.id === id)?.name || '-';
  }

  search() { this.loadData(); }

  edit(item: Doctor) {
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

  delete(item: Doctor) {
    if (confirm('هل أنت متأكد من الحذف؟')) {
      this.http.delete(`${this.apiUrl}/${item.id}`).subscribe({ next: () => this.loadData() });
    }
  }
}
