import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { environment } from '../../environments/environment';

interface Patient {
  id: string;
  mrn: string;
  firstNameAr: string;
  middleNameAr?: string;
  lastNameAr: string;
  firstNameEn?: string;
  middleNameEn?: string;
  lastNameEn?: string;
  fullNameAr: string;
  fullNameEn?: string;
  dateOfBirth: string;
  gender: number;
  mobileNumber: string;
  identityNumber?: string;
  email?: string;
  address?: string;
  city?: string;
  bloodType?: string;
  category?: number;
  isActive: boolean;
  creationTime: string;
}

@Component({
  selector: 'app-patients',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-user-injured me-2"></i>
            المرضى - Patients
          </h5>
          <button class="btn btn-primary" (click)="showForm = true; editingItem = null; resetForm()">
            <i class="fas fa-plus me-1"></i> إضافة ملف جديد
          </button>
        </div>
        <div class="card-body">
          <!-- Search -->
          <div class="row mb-3">
            <div class="col-md-6">
              <div class="input-group">
                <span class="input-group-text"><i class="fas fa-search"></i></span>
                <input type="text" class="form-control" placeholder="بحث بالاسم، رقم الملف، الهوية، أو الجوال..." 
                       [(ngModel)]="searchText" (input)="search()">
              </div>
            </div>
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-striped table-hover align-middle">
              <thead class="table-dark">
                <tr>
                  <th>رقم الملف (MRN)</th>
                  <th>الاسم الكامل</th>
                  <th>تاريخ الميلاد (العمر)</th>
                  <th>الجنس</th>
                  <th>الجوال</th>
                  <th>الحالة</th>
                  <th>الإجراءات</th>
                </tr>
              </thead>
              <tbody>
                @for (item of items; track item.id) {
                  <tr>
                    <td><span class="fw-bold text-primary">{{ item.mrn }}</span></td>
                    <td>
                      <div>{{ item.fullNameAr }}</div>
                      <small class="text-muted">{{ item.fullNameEn }}</small>
                    </td>
                    <td>
                      {{ item.dateOfBirth | date:'yyyy-MM-dd' }}
                      <span class="badge bg-light text-dark ms-1">{{ calculateAge(item.dateOfBirth) }} سنة</span>
                    </td>
                    <td>{{ getGender(item.gender) }}</td>
                    <td>{{ item.mobileNumber }}</td>
                    <td>
                      <span [class]="item.isActive ? 'badge bg-success' : 'badge bg-secondary'">
                        {{ item.isActive ? 'نشط' : 'غير نشط' }}
                      </span>
                    </td>
                    <td>
                      <a class="btn btn-sm btn-outline-success me-1" [href]="'/patients/' + item.id + '/medical-record'" title="السجل الطبي">
                        <i class="fas fa-file-medical"></i>
                      </a>
                      <button class="btn btn-sm btn-outline-primary me-1" (click)="edit(item)" title="تعديل">
                        <i class="fas fa-edit"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-danger" (click)="delete(item)" title="حذف">
                        <i class="fas fa-trash"></i>
                      </button>
                    </td>
                  </tr>
                } @empty {
                  <tr>
                    <td colspan="7" class="text-center text-muted py-5">
                      <i class="fas fa-folder-open fs-1 d-block mb-3"></i>
                      لا توجد بيانات للمرضى
                    </td>
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
          <div class="modal-dialog modal-xl">
            <div class="modal-content">
              <div class="modal-header bg-light">
                <h5 class="modal-title">
                  <i class="fas fa-user-circle me-2"></i>
                  {{ editingItem ? 'تعديل بيانات مريض' : 'تسجيل مريض جديد' }}
                </h5>
                <button type="button" class="btn-close" (click)="showForm = false"></button>
              </div>
              <div class="modal-body p-4">
                
                <!-- Personal Info -->
                <h6 class="border-bottom pb-2 mb-3 text-primary">البيانات الشخصية</h6>
                <div class="row">
                  <div class="col-md-4 mb-3">
                    <label class="form-label">الاسم الأول (عربي) *</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.firstNameAr" required>
                  </div>
                  <div class="col-md-4 mb-3">
                    <label class="form-label">اسم الأب (عربي)</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.middleNameAr">
                  </div>
                  <div class="col-md-4 mb-3">
                    <label class="form-label">اللقب (عربي) *</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.lastNameAr" required>
                  </div>
                </div>

                <div class="row">
                  <div class="col-md-4 mb-3">
                    <label class="form-label">First Name (En)</label>
                    <input type="text" class="form-control text-start" dir="ltr" [(ngModel)]="formData.firstNameEn">
                  </div>
                  <div class="col-md-4 mb-3">
                    <label class="form-label">Middle Name (En)</label>
                    <input type="text" class="form-control text-start" dir="ltr" [(ngModel)]="formData.middleNameEn">
                  </div>
                  <div class="col-md-4 mb-3">
                    <label class="form-label">Last Name (En)</label>
                    <input type="text" class="form-control text-start" dir="ltr" [(ngModel)]="formData.lastNameEn">
                  </div>
                </div>

                <div class="row">
                  <div class="col-md-3 mb-3">
                    <label class="form-label">تاريخ الميلاد *</label>
                    <input type="date" class="form-control" [(ngModel)]="formData.dateOfBirth" required>
                  </div>
                  <div class="col-md-3 mb-3">
                    <label class="form-label">الجنس *</label>
                    <select class="form-select" [(ngModel)]="formData.gender" required>
                      <option [ngValue]="1">ذكر</option>
                      <option [ngValue]="2">أنثى</option>
                    </select>
                  </div>
                  <div class="col-md-3 mb-3">
                    <label class="form-label">رقم الهوية</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.identityNumber">
                  </div>
                  <div class="col-md-3 mb-3">
                    <label class="form-label">فصيلة الدم</label>
                    <select class="form-select" [(ngModel)]="formData.bloodType">
                      <option [ngValue]="null">-- اختر --</option>
                      <option value="A+">A+</option>
                      <option value="A-">A-</option>
                      <option value="B+">B+</option>
                      <option value="B-">B-</option>
                      <option value="O+">O+</option>
                      <option value="O-">O-</option>
                      <option value="AB+">AB+</option>
                      <option value="AB-">AB-</option>
                    </select>
                  </div>
                </div>

                <!-- Contact Info -->
                <h6 class="border-bottom pb-2 mb-3 mt-4 text-primary">بيانات الاتصال والعنوان</h6>
                <div class="row">
                  <div class="col-md-4 mb-3">
                    <label class="form-label">رقم الجوال *</label>
                    <input type="tel" class="form-control text-start" dir="ltr" [(ngModel)]="formData.mobileNumber" required>
                  </div>
                  <div class="col-md-4 mb-3">
                    <label class="form-label">البريد الإلكتروني</label>
                    <input type="email" class="form-control text-start" dir="ltr" [(ngModel)]="formData.email">
                  </div>
                  <div class="col-md-4 mb-3">
                    <label class="form-label">المدينة</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.city">
                  </div>
                  <div class="col-md-12 mb-3">
                    <label class="form-label">العنوان التفصيلي</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.address">
                  </div>
                </div>

                <div class="form-check mt-3">
                  <input type="checkbox" class="form-check-input" [(ngModel)]="formData.isActive" id="isActive">
                  <label class="form-check-label" for="isActive">ملف نشط</label>
                </div>
              </div>
              <div class="modal-footer bg-light">
                <button type="button" class="btn btn-secondary px-4" (click)="showForm = false">إلغاء</button>
                <button type="button" class="btn btn-primary px-4" (click)="save()">
                  <i class="fas fa-save me-1"></i> حفظ البيانات
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
    .table th { white-space: nowrap; }
  `]
})
export class PatientsComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url + '/api/app/patient';

  items: Patient[] = [];
  searchText = '';
  showForm = false;
  editingItem: Patient | null = null;
  formData: Partial<Patient> = this.getEmptyForm();

  // Pagination
  page = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit() {
    this.loadData();
  }

  getEmptyForm(): Partial<Patient> {
    return {
      firstNameAr: '', lastNameAr: '',
      dateOfBirth: '', gender: 1,
      mobileNumber: '', identityNumber: '',
      isActive: true,
      bloodType: null
    };
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

  edit(item: Patient) {
    this.editingItem = item;
    this.formData = { ...item };
    // Fix date format for input[type=date]
    if (this.formData.dateOfBirth) {
      this.formData.dateOfBirth = this.formData.dateOfBirth.split('T')[0];
    }
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

  delete(item: Patient) {
    if (confirm(`هل أنت متأكد من حذف ملف المريض ${item.fullNameAr}؟`)) {
      this.http.delete(`${this.apiUrl}/${item.id}`).subscribe({ next: () => this.loadData() });
    }
  }

  getGender(gender: number): string {
    return gender === 1 ? 'ذكر' : 'أنثى';
  }

  calculateAge(dateOfBirth: string): number {
    if (!dateOfBirth) return 0;
    const today = new Date();
    const birthDate = new Date(dateOfBirth);
    let age = today.getFullYear() - birthDate.getFullYear();
    const m = today.getMonth() - birthDate.getMonth();
    if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    return age;
  }
}
