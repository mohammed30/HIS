import { Component, OnInit, inject, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { CoreModule, ListService } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

interface Clinic {
  id: string;
  code: string;
  nameAr: string;
  nameEn?: string;
  departmentId?: string;
  departmentName?: string;
  location?: string;
  roomNumber?: string;
  extensionNumber?: string;
  consultationFee?: number;
  isActive: boolean;
  sortOrder: number;
}

interface Lookup {
  id: string;
  name: string;
}

@Component({
  selector: 'app-clinics',
  standalone: true,
  imports: [CommonModule, CoreModule, ThemeSharedModule, FormsModule, ReactiveFormsModule],
  providers: [ListService],
  template: `
    <div class="card">
      <div class="card-header d-flex justify-content-between align-items-center">
        <div>
          <h5 class="card-title mb-0">العيادات - Clinics</h5>
          <small class="text-muted">إدارة عيادات وغرف المستشفى</small>
        </div>
        <button class="btn btn-primary" (click)="create()">
          <i class="fas fa-plus me-1"></i> إضافة
        </button>
      </div>
      <div class="card-body">
        <div class="row mb-3">
          <div class="col-md-4">
            <div class="input-group">
              <span class="input-group-text"><i class="fas fa-search"></i></span>
              <input type="text" class="form-control" placeholder="بحث..." 
                     [(ngModel)]="searchText" (input)="search()">
            </div>
          </div>
        </div>

        <ngx-datatable [rows]="items" [count]="totalCount" [list]="list" default>
          <ngx-datatable-column name="الكود" prop="code"></ngx-datatable-column>
          <ngx-datatable-column name="الاسم" prop="nameAr"></ngx-datatable-column>
          <ngx-datatable-column name="القسم" prop="departmentName">
            <ng-template let-value="value" ngx-datatable-cell-template>
              {{ value || '-' }}
            </ng-template>
          </ngx-datatable-column>
          <ngx-datatable-column name="مكان العيادة" prop="location"></ngx-datatable-column>
          <ngx-datatable-column name="رقم الغرفة" prop="roomNumber"></ngx-datatable-column>
          <ngx-datatable-column name="الحالة" prop="isActive">
            <ng-template let-value="value" ngx-datatable-cell-template>
              <span class="badge" [class.bg-success]="value" [class.bg-secondary]="!value">
                {{ value ? 'نشط' : 'غير نشط' }}
              </span>
            </ng-template>
          </ngx-datatable-column>
          <ngx-datatable-column name="الإجراءات" sortable="false">
            <ng-template let-row="row" ngx-datatable-cell-template>
              <button class="btn btn-sm btn-outline-primary me-1" (click)="edit(row)">
                <i class="fas fa-pencil-alt"></i>
              </button>
              <button class="btn btn-sm btn-outline-danger" (click)="delete(row)">
                <i class="fas fa-trash"></i>
              </button>
            </ng-template>
          </ngx-datatable-column>
        </ngx-datatable>
      </div>
    </div>

    <abp-modal [(visible)]="showForm">
      <ng-template #abpHeader>
        <h3>{{ editingItem ? 'تعديل' : 'إضافة' }} عيادة</h3>
      </ng-template>

      <ng-template #abpBody>
        <form #clinicForm="ngForm">
          <div class="mb-3">
            <label class="form-label">القسم</label>
            <select class="form-select" [(ngModel)]="formData.departmentId" name="departmentId">
              <option value="">اختر القسم</option>
              <option *ngFor="let dept of departments" [value]="dept.id">{{ dept.name }}</option>
            </select>
          </div>
          <div class="row">
            <div class="col-md-6 mb-3">
              <label class="form-label">الاسم بالعربية *</label>
              <input type="text" class="form-control" [(ngModel)]="formData.nameAr" name="nameAr" required>
            </div>
            <div class="col-md-6 mb-3">
              <label class="form-label">الاسم بالإنجليزية</label>
              <input type="text" class="form-control" [(ngModel)]="formData.nameEn" name="nameEn">
            </div>
          </div>
          <div class="row">
            <div class="col-md-4 mb-3">
              <label class="form-label">مكان العيادة</label>
              <input type="text" class="form-control" [(ngModel)]="formData.location" name="location">
            </div>
            <div class="col-md-4 mb-3">
              <label class="form-label">رقم الغرفة</label>
              <input type="text" class="form-control" [(ngModel)]="formData.roomNumber" name="roomNumber">
            </div>
            <div class="col-md-4 mb-3">
              <label class="form-label">التحويلة</label>
              <input type="text" class="form-control" [(ngModel)]="formData.extensionNumber" name="extensionNumber">
            </div>
          </div>
          <div class="form-check mt-3">
            <input type="checkbox" class="form-check-input" [(ngModel)]="formData.isActive" name="isActive" id="isActive">
            <label class="form-check-label" for="isActive">نشط</label>
          </div>
        </form>
      </ng-template>

      <ng-template #abpFooter>
        <button type="button" class="btn btn-secondary" (click)="showForm = false">إلغاء</button>
        <button type="button" class="btn btn-primary" (click)="save()" [disabled]="form?.invalid">
          <i class="fas fa-save me-1"></i> حفظ
        </button>
      </ng-template>
    </abp-modal>
  `,
  styles: [`.modal { z-index: 1050; }`]
})
export class ClinicsComponent implements OnInit {
  @ViewChild('clinicForm') form: NgForm;
  private cdr = inject(ChangeDetectorRef);
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url + '/api/app/clinic';
  public readonly list = inject(ListService);
  private confirmation = inject(ConfirmationService);

  items: Clinic[] = [];
  departments: Lookup[] = [];
  searchText = '';
  showForm = false;
  editingItem: Clinic | null = null;
  formData: Partial<Clinic> = this.getEmptyForm();

  // Pagination
  page = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit() {
    const streamCreator = (query) => this.getData(query);
    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.items = response.items as Clinic[];
      this.totalCount = response.totalCount;
    });
    this.loadDepartments();
  }

  create() {
    this.editingItem = null;
    this.resetForm();
    this.showForm = true;
    setTimeout(() => this.cdr.detectChanges());
  }

  getEmptyForm(): Partial<Clinic> {
    return { code: '', nameAr: '', nameEn: '', departmentId: '', location: '', roomNumber: '', extensionNumber: '', consultationFee: 0, isActive: true, sortOrder: 0 };
  }

  resetForm() { this.formData = this.getEmptyForm(); }

  getData(query: any) {
    return this.http.get<any>(`${this.apiUrl}?searchText=${this.searchText}&skipCount=${query.skipCount}&maxResultCount=${query.maxResultCount}`);
  }

  loadData() {
    this.list.get();
  }

  onPageChange(page: number) {
    this.page = page;
    this.loadData();
  }

  loadDepartments() {
    this.http.get<Lookup[]>(environment.apis.default.url + '/api/app/department/medical-departments-lookup').subscribe({
      next: (res) => this.departments = res,
      error: (err) => console.error(err)
    });
  }

  search() {
    this.page = 1;
    this.loadData();
  }

  edit(item: Clinic) {
    this.editingItem = item;
    this.formData = { ...item };
    this.showForm = true;
    setTimeout(() => this.cdr.detectChanges());
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

  delete(item: Clinic) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.http.delete(`${this.apiUrl}/${item.id}`).subscribe({ next: () => this.loadData() });
      }
    });
  }
}
