import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

interface Specialty {
  id: string;
  code: string;
  nameAr: string;
  nameEn?: string;
  description?: string;
  isActive: boolean;
  sortOrder: number;
}

@Component({
  selector: 'app-specialties',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-stethoscope me-2"></i>
            التخصصات - Specialties
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
                  <th>الوصف</th>
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
                    <td>{{ item.description }}</td>
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
        </div>
      </div>

      <!-- Modal Form -->
      @if (showForm) {
        <div class="modal show d-block" style="background: rgba(0,0,0,0.5)">
          <div class="modal-dialog">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">{{ editingItem ? 'تعديل' : 'إضافة' }} تخصص</h5>
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
export class SpecialtiesComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url + '/api/app/specialty';

  items: Specialty[] = [];
  searchText = '';
  showForm = false;
  editingItem: Specialty | null = null;
  formData: Partial<Specialty> = this.getEmptyForm();

  ngOnInit() {
    this.loadData();
  }

  getEmptyForm(): Partial<Specialty> {
    return { code: '', nameAr: '', nameEn: '', description: '', isActive: true, sortOrder: 0 };
  }

  resetForm() {
    this.formData = this.getEmptyForm();
  }

  loadData() {
    this.http.get<any>(`${this.apiUrl}?searchText=${this.searchText}`).subscribe({
      next: (res) => this.items = res.items || [],
      error: (err) => console.error(err)
    });
  }

  search() {
    this.loadData();
  }

  edit(item: Specialty) {
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

  delete(item: Specialty) {
    if (confirm('هل أنت متأكد من الحذف؟')) {
      this.http.delete(`${this.apiUrl}/${item.id}`).subscribe({
        next: () => this.loadData(),
        error: (err) => console.error(err)
      });
    }
  }
}
