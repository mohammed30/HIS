import { Component, OnInit, inject, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CoreModule, ListService } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
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
  imports: [CommonModule, CoreModule, ThemeSharedModule, FormsModule, ReactiveFormsModule],
  providers: [ListService],
  template: `
    <div class="card">
      <div class="card-header d-flex justify-content-between align-items-center">
        <div>
          <h5 class="card-title mb-0">{{ '::Menu:Patients' | abpLocalization }}</h5>
          <small class="text-muted">{{ '::ManagePatientsSubtitle' | abpLocalization }}</small>
        </div>
        <button class="btn btn-primary" (click)="create()">
          <i class="fas fa-plus me-1"></i> {{ '::New' | abpLocalization }}
        </button>
      </div>
      <div class="card-body">
        <div class="row mb-3">
          <div class="col-md-6">
            <div class="input-group">
              <span class="input-group-text"><i class="fas fa-search"></i></span>
              <input type="text" class="form-control" [placeholder]="'::Search' | abpLocalization" 
                     [(ngModel)]="searchText" (input)="search()">
            </div>
          </div>
        </div>

        <ngx-datatable [rows]="items" [count]="totalCount" [list]="list" default>
          <ngx-datatable-column [name]="'::MRN' | abpLocalization" prop="mrn">
            <ng-template let-value="value" ngx-datatable-cell-template>
              <span class="fw-bold text-primary">{{ value }}</span>
            </ng-template>
          </ngx-datatable-column>
          <ngx-datatable-column [name]="'::FullName' | abpLocalization" prop="fullNameAr">
             <ng-template let-row="row" let-value="value" ngx-datatable-cell-template>
                <div>{{ value }}</div>
                <small class="text-muted">{{ row.fullNameEn }}</small>
             </ng-template>
          </ngx-datatable-column>
          <ngx-datatable-column [name]="'::Age' | abpLocalization">
            <ng-template let-row="row" ngx-datatable-cell-template>
              {{ row.dateOfBirth | date:'yyyy-MM-dd' }}
              <span class="badge bg-light text-dark ms-1">{{ calculateAge(row.dateOfBirth) }} سنة</span>
            </ng-template>
          </ngx-datatable-column>
          <ngx-datatable-column [name]="'::Gender' | abpLocalization" prop="gender">
            <ng-template let-value="value" ngx-datatable-cell-template>
              {{ getGender(value) }}
            </ng-template>
          </ngx-datatable-column>
          <ngx-datatable-column [name]="'::MobileNumber' | abpLocalization" prop="mobileNumber"></ngx-datatable-column>
          <ngx-datatable-column [name]="'::Status' | abpLocalization" prop="isActive">
            <ng-template let-value="value" ngx-datatable-cell-template>
              <span class="badge" [class.bg-success]="value" [class.bg-secondary]="!value">
                {{ value ? ('::Active' | abpLocalization) : ('::Inactive' | abpLocalization) }}
              </span>
            </ng-template>
          </ngx-datatable-column>
          <ngx-datatable-column [name]="'::Actions' | abpLocalization" sortable="false">
            <ng-template let-row="row" ngx-datatable-cell-template>
              <a class="btn btn-sm btn-outline-success me-1" [href]="'/patients/' + row.id + '/medical-record'" [title]="'::MedicalRecord' | abpLocalization">
                <i class="fas fa-vials"></i>
              </a>
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

    <abp-modal [(visible)]="showForm" [options]="{ size: 'xl' }">
      <ng-template #abpHeader>
        <h3>{{ (editingItem ? '::Edit' : '::New') | abpLocalization }} {{ '::Menu:Patients' | abpLocalization }}</h3>
      </ng-template>

      <ng-template #abpBody>
        <form #patientForm="ngForm">
          <h6 class="border-bottom pb-2 mb-3 text-primary">{{ '::PersonalInformation' | abpLocalization }}</h6>
          <div class="row">
            <div class="col-md-4 mb-3">
              <label class="form-label">{{ '::FirstName' | abpLocalization }} (Ar) *</label>
              <input type="text" class="form-control" [(ngModel)]="formData.firstNameAr" name="firstNameAr" required>
            </div>
            <div class="col-md-4 mb-3">
              <label class="form-label">{{ '::MiddleName' | abpLocalization }} (Ar)</label>
              <input type="text" class="form-control" [(ngModel)]="formData.middleNameAr" name="middleNameAr">
            </div>
            <div class="col-md-4 mb-3">
              <label class="form-label">{{ '::LastName' | abpLocalization }} (Ar) *</label>
              <input type="text" class="form-control" [(ngModel)]="formData.lastNameAr" name="lastNameAr" required>
            </div>
          </div>
          <div class="row">
            <div class="col-md-4 mb-3">
              <label class="form-label">First Name (En)</label>
              <input type="text" class="form-control" [(ngModel)]="formData.firstNameEn" name="firstNameEn">
            </div>
            <div class="col-md-4 mb-3">
              <label class="form-label">Middle Name (En)</label>
              <input type="text" class="form-control" [(ngModel)]="formData.middleNameEn" name="middleNameEn">
            </div>
            <div class="col-md-4 mb-3">
              <label class="form-label">Last Name (En)</label>
              <input type="text" class="form-control" [(ngModel)]="formData.lastNameEn" name="lastNameEn">
            </div>
          </div>
          <div class="row">
            <div class="col-md-3 mb-3">
              <label class="form-label">{{ '::DateOfBirth' | abpLocalization }} *</label>
              <input type="date" class="form-control" [(ngModel)]="formData.dateOfBirth" name="dateOfBirth" required>
            </div>
            <div class="col-md-3 mb-3">
              <label class="form-label">{{ '::Gender' | abpLocalization }} *</label>
              <select class="form-select" [(ngModel)]="formData.gender" name="gender" required>
                <option [ngValue]="0">ذكر</option>
                <option [ngValue]="1">أنثى</option>
              </select>
            </div>
            <div class="col-md-3 mb-3">
              <label class="form-label">{{ '::IdentityNumber' | abpLocalization }}</label>
              <input type="text" class="form-control" [(ngModel)]="formData.identityNumber" name="identityNumber">
            </div>
            <div class="col-md-3 mb-3">
              <label class="form-label">{{ '::BloodType' | abpLocalization }}</label>
              <select class="form-select" [(ngModel)]="formData.bloodType" name="bloodType">
                 <option [ngValue]="null">-- اختر --</option>
                 <option value="A+">A+</option><option value="A-">A-</option>
                 <option value="B+">B+</option><option value="B-">B-</option>
                 <option value="O+">O+</option><option value="O-">O-</option>
                 <option value="AB+">AB+</option><option value="AB-">AB-</option>
                 <option value="O+">O+</option><option value="O-">O-</option>
                 <option value="AB+">AB+</option><option value="AB-">AB-</option>
              </select>
            </div>
          </div>

          <h6 class="border-bottom pb-2 mb-3 mt-4 text-primary">{{ '::ContactInformation' | abpLocalization }}</h6>
          <div class="row">
            <div class="col-md-4 mb-3">
              <label class="form-label">{{ '::MobileNumber' | abpLocalization }} *</label>
              <input type="tel" class="form-control" [(ngModel)]="formData.mobileNumber" name="mobileNumber" required>
            </div>
            <div class="col-md-4 mb-3">
              <label class="form-label">{{ '::Email' | abpLocalization }}</label>
              <input type="email" class="form-control" [(ngModel)]="formData.email" name="email">
            </div>
            <div class="col-md-4 mb-3">
              <label class="form-label">{{ '::City' | abpLocalization }}</label>
              <input type="text" class="form-control" [(ngModel)]="formData.city" name="city">
            </div>
            <div class="col-md-12 mb-3">
              <label class="form-label">{{ '::Address' | abpLocalization }}</label>
              <input type="text" class="form-control" [(ngModel)]="formData.address" name="address">
            </div>
          </div>
          <div class="form-check">
            <input type="checkbox" class="form-check-input" [(ngModel)]="formData.isActive" name="isActive" id="isActive">
            <label class="form-check-label" for="isActive">{{ '::Active' | abpLocalization }}</label>
          </div>
        </form>
      </ng-template>

      <ng-template #abpFooter>
        <button type="button" class="btn btn-secondary" (click)="showForm = false">{{ '::Cancel' | abpLocalization }}</button>
        <button type="button" class="btn btn-primary" (click)="save()" [disabled]="form?.invalid">
          <i class="fas fa-save me-1"></i> {{ '::Save' | abpLocalization }}
        </button>
      </ng-template>
    </abp-modal>
  `,
  styles: [`
    .modal { z-index: 1050; }
    .table th { white-space: nowrap; }
  `]
})
export class PatientsComponent implements OnInit {
  @ViewChild('patientForm') form: NgForm;
  private cdr = inject(ChangeDetectorRef);
  private http = inject(HttpClient);
  private apiUrl = environment.apis.default.url + '/api/app/patient';
  public readonly list = inject(ListService);

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
    const streamCreator = (query) => this.getData(query);
    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.items = response.items as Patient[];
      this.totalCount = response.totalCount;
    });
  }

  create() {
    this.editingItem = null;
    this.resetForm();
    this.showForm = true;
    setTimeout(() => this.cdr.detectChanges());
  }

  getEmptyForm(): Partial<Patient> {
    return {
      firstNameAr: '', lastNameAr: '',
      dateOfBirth: '', gender: 0,
      mobileNumber: '', identityNumber: '',
      isActive: true,
      bloodType: null
    };
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
    setTimeout(() => this.cdr.detectChanges());
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
    return gender === 0 ? 'ذكر' : 'أنثى';
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
