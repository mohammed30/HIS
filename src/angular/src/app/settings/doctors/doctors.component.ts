import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { environment } from '../../../environments/environment';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ViewEncapsulation } from '@angular/core';

interface Doctor {
  id: string;
  code: string;
  nameAr: string;
  nameEn: string;
  specialtyId: string;
  specialtyName?: string;
  departmentId: string;
  departmentName?: string;
  clinicId?: string;
  clinicName?: string;
  mobileNumber?: string;
  email?: string;
  licenseNumber?: string;
  consultationFee: number;
  morningConsultationFee: number;
  eveningConsultationFee: number;
  isActive: boolean;
  doctorPercentage?: number;
  accountId?: string;
}

interface Lookup {
  id: string;
  name: string;
}

@Component({
  selector: 'app-doctors',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule, ThemeSharedModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-user-md me-2"></i>
            الأطباء - Doctors
          </h5>
          <div>
            <button class="btn btn-warning me-2" (click)="syncAccounts()">
              <i class="fas fa-sync-alt me-1"></i> مزامنة حسابات الأطباء القدامى
            </button>
            <button class="btn btn-primary" (click)="showForm = true; editingItem = null; resetForm()">
              <i class="fas fa-plus me-1"></i> إضافة
            </button>
          </div>
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
                  <th (click)="toggleSort('code')" style="cursor: pointer;">
                    الكود
                    @if (sortKey === 'code') {
                      <i class="fas" [class.fa-sort-up]="sortOrder === 'asc'" [class.fa-sort-down]="sortOrder === 'desc'"></i>
                    } @else {
                      <i class="fas fa-sort text-muted ms-1"></i>
                    }
                  </th>
                  <th (click)="toggleSort('nameAr')" style="cursor: pointer;">
                    الاسم (عربي)
                    @if (sortKey === 'nameAr') {
                      <i class="fas" [class.fa-sort-up]="sortOrder === 'asc'" [class.fa-sort-down]="sortOrder === 'desc'"></i>
                    } @else {
                      <i class="fas fa-sort text-muted ms-1"></i>
                    }
                  </th>
                  <th (click)="toggleSort('nameEn')" style="cursor: pointer;">
                    الاسم (إنجليزي)
                    @if (sortKey === 'nameEn') {
                      <i class="fas" [class.fa-sort-up]="sortOrder === 'asc'" [class.fa-sort-down]="sortOrder === 'desc'"></i>
                    } @else {
                      <i class="fas fa-sort text-muted ms-1"></i>
                    }
                  </th>
                  <th (click)="toggleSort('specialtyId')" style="cursor: pointer;">
                    التخصص
                    @if (sortKey === 'specialtyId') {
                      <i class="fas" [class.fa-sort-up]="sortOrder === 'asc'" [class.fa-sort-down]="sortOrder === 'desc'"></i>
                    } @else {
                      <i class="fas fa-sort text-muted ms-1"></i>
                    }
                  </th>
                  <th (click)="toggleSort('departmentId')" style="cursor: pointer;">
                    القسم
                    @if (sortKey === 'departmentId') {
                      <i class="fas" [class.fa-sort-up]="sortOrder === 'asc'" [class.fa-sort-down]="sortOrder === 'desc'"></i>
                    } @else {
                      <i class="fas fa-sort text-muted ms-1"></i>
                    }
                  </th>
                  <th>العيادة</th>
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
                    <td>{{ item.clinicName || '-' }}</td>
                    <td>{{ item.mobileNumber }}</td>
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
                     <div class="col-md-6 mb-3 position-relative" (clickOutside)="isSpecDropdownOpen = false">
                    <label class="form-label">التخصص *</label>
                    <div class="dropdown">
                      <input type="text" class="form-control cursor-pointer" 
                             [class.selected-placeholder]="!!selectedSpecName"
                             [placeholder]="selectedSpecName || 'اختر التخصص (بحث...)'" 
                             [(ngModel)]="specSearchText" 
                             (click)="isSpecDropdownOpen = !isSpecDropdownOpen"
                             (input)="isSpecDropdownOpen = true; filterSpecialties()"
                             name="specSearch"
                             autocomplete="off"
                             required>
                      <div class="dropdown-menu w-100 shadow" [class.show]="isSpecDropdownOpen" style="max-height: 250px; overflow-y: auto;">
                        @for (spec of filteredSpecialties; track spec.id) {
                          <button type="button" class="dropdown-item" (click)="selectSpecialty(spec)">
                            {{ spec.name }}
                          </button>
                        }
                        @if (filteredSpecialties.length === 0) {
                          <span class="dropdown-item text-muted">لا يوجد نتائج</span>
                        }
                      </div>
                    </div>
                  </div>
                    <div class="col-md-6 mb-3 position-relative" (clickOutside)="isDeptDropdownOpen = false">
                    <label class="form-label">القسم *</label>
                    <div class="dropdown">
                      <input type="text" class="form-control cursor-pointer" 
                             [class.selected-placeholder]="!!selectedDeptName"
                             [placeholder]="selectedDeptName || 'اختر القسم (بحث بالكود أو الاسم)'" 
                             [(ngModel)]="deptSearchText" 
                             (click)="isDeptDropdownOpen = !isDeptDropdownOpen"
                             (input)="isDeptDropdownOpen = true; filterDepartments()"
                             name="deptSearch"
                             autocomplete="off">
                      <div class="dropdown-menu w-100 shadow" [class.show]="isDeptDropdownOpen" style="max-height: 250px; overflow-y: auto;">
                        @for (dept of filteredDepartments; track dept.id) {
                          <button type="button" class="dropdown-item" (click)="selectDepartment(dept)">
                            {{ dept.name }}
                          </button>
                        }
                        @if (filteredDepartments.length === 0) {
                          <span class="dropdown-item text-muted">لا يوجد نتائج</span>
                        }
                      </div>
                    </div>
                  </div>
                </div>

                <div class="row">
                  <div class="col-md-6 mb-3 position-relative" (clickOutside)="isClinicDropdownOpen = false">
                    <label class="form-label">العيادة</label>
                    <div class="dropdown">
                      <input type="text" class="form-control cursor-pointer" 
                             [class.selected-placeholder]="!!selectedClinicName"
                             [placeholder]="selectedClinicName || 'اختر العيادة (اختياري)'" 
                             [(ngModel)]="clinicSearchText" 
                             (click)="isClinicDropdownOpen = !isClinicDropdownOpen"
                             (input)="isClinicDropdownOpen = true; filterClinicsList()"
                             name="clinicSearch"
                             autocomplete="off"
                             [disabled]="!formData.departmentId">
                      <div class="dropdown-menu w-100 shadow" [class.show]="isClinicDropdownOpen" style="max-height: 250px; overflow-y: auto;">
                        @if (formData.departmentId) {
                          @for (clinic of visibleClinics; track clinic.id) {
                            <button type="button" class="dropdown-item" (click)="selectClinic(clinic)">
                              {{ clinic.name }}
                            </button>
                          }
                          @if (visibleClinics.length === 0) {
                            <span class="dropdown-item text-muted">لا يوجد نتائج</span>
                          }
                        } @else {
                          <span class="dropdown-item text-muted">الرجاء اختيار القسم أولاً</span>
                        }
                      </div>
                    </div>
                  </div>
                  <div class="col-md-6 mb-3">
                    <label class="form-label">الجوال</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.mobileNumber">
                  </div>
                </div>
                
                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label class="form-label">رقم الهوية</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.licenseNumber">
                  </div>
                    <div class="col-md-6 mb-3">
                      <label class="form-label">البريد الإلكتروني</label>
                      <input type="email" class="form-control" [(ngModel)]="formData.email" name="email" #emailCtrl="ngModel" email>
                      @if (emailCtrl.invalid && (emailCtrl.dirty || emailCtrl.touched)) {
                        <div class="text-danger small mt-1">الرجاء إدخال بريد إلكتروني صحيح</div>
                      }
                    </div>
                </div>

                <div class="row">
                  <div class="col-md-4 mb-3">
                    <label class="form-label">سعر الكشف (عام)</label>
                    <input type="number" class="form-control" [(ngModel)]="formData.consultationFee">
                  </div>
                    <div class="col-md-4 mb-3">
                      <label class="form-label">سعر الكشف (صباحي)</label>
                      <input type="number" class="form-control" [(ngModel)]="formData.morningConsultationFee">
                    </div>
                    <div class="col-md-4 mb-3">
                      <label class="form-label">سعر الكشف (مسائي)</label>
                      <input type="number" class="form-control" [(ngModel)]="formData.eveningConsultationFee">
                    </div>
                  </div>

                <div class="form-check mb-3">
                  <input type="checkbox" class="form-check-input" [(ngModel)]="formData.isActive" id="isActive">
                  <label class="form-check-label" for="isActive">نشط</label>
                </div>

                <!-- نسبة الطبيب والمستشفى -->
                <div class="card border-primary mb-3">
                  <div class="card-header bg-primary text-white py-2">
                    <i class="fas fa-percent me-1"></i> توزيع الإيرادات
                  </div>
                  <div class="card-body">
                    <div class="row align-items-center">
                      <div class="col-md-5 mb-3">
                        <label class="form-label fw-bold">نسبة الطبيب %</label>
                        <div class="input-group">
                          <input type="number" class="form-control" [(ngModel)]="formData.doctorPercentage"
                            (input)="onPercentageChange()"
                            min="0" max="100" step="1">
                          <span class="input-group-text">%</span>
                        </div>
                      </div>
                      <div class="col-md-2 text-center d-none d-md-block">
                        <div class="text-muted small">من 100%</div>
                      </div>
                      <div class="col-md-5 mb-3">
                        <label class="form-label fw-bold">نسبة المستشفى %</label>
                        <div class="input-group">
                          <input type="number" class="form-control bg-light" [value]="hospitalPercentage" readonly>
                          <span class="input-group-text">%</span>
                        </div>
                      </div>
                    </div>
                    <!-- شريط التوزيع البصري -->
                    <div class="mb-2">
                      <div class="d-flex justify-content-between small mb-1">
                        <span class="text-success fw-bold">الطبيب: {{ formData.doctorPercentage || 0 }}%</span>
                        <span class="text-primary fw-bold">المستشفى: {{ hospitalPercentage }}%</span>
                      </div>
                      <div class="progress" style="height: 20px; border-radius: 10px;">
                        <div class="progress-bar bg-success" role="progressbar"
                          [style.width.%]="formData.doctorPercentage || 0"
                          style="transition: width 0.3s ease; border-radius: 10px 0 0 10px;">
                          <span *ngIf="(formData.doctorPercentage || 0) > 10">{{ formData.doctorPercentage }}%</span>
                        </div>
                        <div class="progress-bar bg-primary" role="progressbar"
                          [style.width.%]="hospitalPercentage"
                          style="transition: width 0.3s ease; border-radius: 0 10px 10px 0;">
                          <span *ngIf="hospitalPercentage > 10">{{ hospitalPercentage }}%</span>
                        </div>
                      </div>
                    </div>
                    <div class="text-muted small mt-1">
                      <i class="fas fa-info-circle me-1"></i>
                      تطبق هذه النسبة على جميع إيرادات الطبيب (كشف، مختبر، أشعة، عمليات...)
                    </div>
                  </div>
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
  encapsulation: ViewEncapsulation.None,
  styles: [
    `.modal { z-index: 1050; }`,
    `.selected-placeholder::placeholder { color: #000 !important; opacity: 1 !important; }`,
    `html[data-theme="dark"] body .selected-placeholder::placeholder { color: #fff !important; }`
  ]
})
export class DoctorsComponent implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url + '/api/app/doctor';
  private confirmation = inject(ConfirmationService);

  items: Doctor[] = [];
  departments: Lookup[] = [];
  filteredDepartments: Lookup[] = [];
  deptSearchText = '';
  selectedDeptName = '';
  isDeptDropdownOpen = false;

  specialties: Lookup[] = [];
  filteredSpecialties: Lookup[] = [];
  specSearchText = '';
  selectedSpecName = '';
  isSpecDropdownOpen = false;

  allClinics: any[] = [];
  filteredClinics: Lookup[] = [];
  visibleClinics: Lookup[] = [];
  clinicSearchText = '';
  selectedClinicName = '';
  isClinicDropdownOpen = false;

  searchText = '';
  showForm = false;
  editingItem: Doctor | null = null;
  formData: Partial<Doctor> = this.getEmptyForm();

  // Pagination & Sorting
  page = 1;
  pageSize = 10;
  totalCount = 0;
  sortKey = '';
  sortOrder: 'asc' | 'desc' = 'asc';

  ngOnInit() {
    this.loadData();
    this.loadLookups();
  }

  loadLookups() {
    this.http.get<Lookup[]>(environment.apis.default.url + '/api/app/department/medical-departments-lookup').subscribe({
      next: (res) => {
        this.departments = res;
        this.filteredDepartments = res;
      },
      error: (err) => console.error(err)
    });
    this.http.get<Lookup[]>(environment.apis.default.url + '/api/app/specialty/lookup').subscribe({
      next: (res) => {
        this.specialties = res;
        this.filteredSpecialties = res;
      },
      error: (err) => console.error(err)
    });
    // Load clinics but keep them all for filtering
    this.http.get<any[]>(environment.apis.default.url + '/api/app/clinic/lookup').subscribe({
      next: (res) => {
        this.allClinics = res;
        this.filterClinics();
      },
      error: (err) => console.error(err)
    });
  }

  filterDepartments() {
    if (!this.deptSearchText) {
      this.filteredDepartments = this.departments;
    } else {
      const term = this.deptSearchText.toLowerCase();
      this.filteredDepartments = this.departments.filter(d => d.name.toLowerCase().includes(term));
    }
  }

  selectDepartment(dept: Lookup) {
    this.formData.departmentId = dept.id;
    this.selectedDeptName = dept.name;
    this.deptSearchText = '';
    this.isDeptDropdownOpen = false;
    this.filterDepartments(); // reset filter
    this.onDepartmentChange();
  }

  onDepartmentChange() {
    this.formData.clinicId = null as any;
    this.selectedClinicName = '';
    this.clinicSearchText = '';
    this.filterClinics();
  }

  filterSpecialties() {
    if (!this.specSearchText) {
      this.filteredSpecialties = this.specialties;
    } else {
      const term = this.specSearchText.toLowerCase();
      this.filteredSpecialties = this.specialties.filter(d => d.name.toLowerCase().includes(term));
    }
  }

  selectSpecialty(spec: Lookup) {
    this.formData.specialtyId = spec.id;
    this.selectedSpecName = spec.name;
    this.specSearchText = '';
    this.isSpecDropdownOpen = false;
    this.filterSpecialties(); // reset filter
  }

  filterClinicsList() {
    if (!this.clinicSearchText) {
      this.visibleClinics = this.filteredClinics;
    } else {
      const term = this.clinicSearchText.toLowerCase();
      this.visibleClinics = this.filteredClinics.filter(d => d.name.toLowerCase().includes(term));
    }
  }

  selectClinic(clinic: Lookup) {
    this.formData.clinicId = clinic.id;
    this.selectedClinicName = clinic.name;
    this.clinicSearchText = '';
    this.isClinicDropdownOpen = false;
    this.filterClinicsList(); // reset filter
  }

  filterClinics() {
    if (!this.formData.departmentId) {
      this.filteredClinics = [];
      return;
    }
    // we need to know which department each clinic belongs to.
    // The lookup might not have it. Let's check the Clinic DTO.
    // Actually, I should probably call GetByDepartment if the lookup doesn't have it.
    this.http.get<any>(`${environment.apis.default.url}/api/app/clinic?departmentId=${this.formData.departmentId}&maxResultCount=1000`).subscribe({
      next: (res) => {
        console.log('Clinics Response:', res);
        const data = res.items ? res.items : res;
        this.filteredClinics = data.map(c => ({ 
          id: c.id || c.Id, 
          name: c.nameAr || c.NameAr || c.name || c.Name || 'مجهول' 
        }));
        this.visibleClinics = this.filteredClinics;
      },
      error: (err) => console.error(err)
    });
  }

  get hospitalPercentage(): number {
    return 100 - (this.formData.doctorPercentage || 0);
  }

  onPercentageChange() {
    if ((this.formData.doctorPercentage ?? 0) > 100) this.formData.doctorPercentage = 100;
    if ((this.formData.doctorPercentage ?? 0) < 0) this.formData.doctorPercentage = 0;
  }

  getEmptyForm(): Partial<Doctor> {
    return { code: '', nameAr: '', nameEn: '', specialtyId: null as any, departmentId: null as any, clinicId: null as any, mobileNumber: '', email: '', licenseNumber: '', consultationFee: 0, morningConsultationFee: 0, eveningConsultationFee: 0, isActive: true, doctorPercentage: 60 };
  }

  resetForm() { 
    this.formData = this.getEmptyForm(); 
    this.selectedDeptName = '';
    this.deptSearchText = '';
    this.selectedSpecName = '';
    this.specSearchText = '';
    this.selectedClinicName = '';
    this.clinicSearchText = '';
  }

  loadData() {
    const skipCount = (this.page - 1) * this.pageSize;
    let url = `${this.apiUrl}?searchText=${this.searchText}&skipCount=${skipCount}&maxResultCount=${this.pageSize}`;
    
    if (this.sortKey) {
      const sorting = `${this.sortKey} ${this.sortOrder}`;
      url += `&sorting=${sorting}`;
    }

    this.http.get<any>(url).subscribe({
      next: (res) => {
        this.items = res.items || [];
        this.totalCount = res.totalCount || 0;
      },
      error: (err) => console.error(err)
    });
  }

  toggleSort(key: string) {
    if (this.sortKey === key) {
      this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortKey = key;
      this.sortOrder = 'asc';
    }
    this.loadData();
  }

  onPageChange(page: number) {
    this.page = page;
    this.loadData();
  }

  search() {
    this.page = 1;
    this.loadData();
  }

  syncAccounts() {
    this.confirmation.info('هل أنت متأكد من مزامنة حسابات الأطباء القدامى؟ (سيقوم النظام بإنشاء حسابات لكل طبيب ليس لديه حساب)', 'تأكيد المزامنة').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.http.post(`${this.apiUrl}/sync-old-doctors-accounts`, {}).subscribe({
          next: () => {
            alert('تم إنشاء وربط الحسابات بنجاح!');
            this.loadData();
          },
          error: (err) => {
            console.error(err);
            alert('حدث خطأ أثناء المزامنة.');
          }
        });
      }
    });
  }

  edit(item: Doctor) {
    this.editingItem = item;
    this.formData = { ...item };
    this.selectedDeptName = (item as any).departmentName || this.getDepartmentName(item.departmentId);
    this.selectedSpecName = (item as any).specialtyName || this.getSpecialtyName(item.specialtyId);
    this.selectedClinicName = (item as any).clinicName || '';
    this.showForm = true;
    if (this.formData.departmentId) {
      this.filterClinics();
    }
  }

  getDepartmentName(id?: string): string {
    return this.departments.find(d => d.id === id)?.name || '-';
  }

  getSpecialtyName(id?: string): string {
    return this.specialties.find(s => s.id === id)?.name || '-';
  }

  save() {
    if (this.formData.email && !/^[a-zA-Z0-9._%+\\-]+@[a-zA-Z0-9.\\-]+\.[a-zA-Z]{2,4}$/.test(this.formData.email)) {
      alert('الرجاء إدخال بريد إلكتروني صحيح.');
      return;
    }

    const req = this.editingItem
      ? this.http.put(`${this.apiUrl}/${this.editingItem.id}`, this.formData)
      : this.http.post(this.apiUrl, this.formData);
    req.subscribe({
      next: () => { this.showForm = false; this.loadData(); },
      error: (err) => console.error(err)
    });
  }

  delete(item: Doctor) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.http.delete(`${this.apiUrl}/${item.id}`).subscribe({ next: () => this.loadData() });
      }
    });
  }
}
