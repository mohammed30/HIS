import { Component, Input, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import { PatientInsuranceService } from '../../proxy/insurance/patient-insurance.service';
import { InsurancePlanService } from '../../proxy/insurance/insurance-plan.service';
import { PatientInsuranceDto, CreateUpdatePatientInsuranceDto } from '../../proxy/insurance/models';
import { PatientInsuranceStatus, patientInsuranceStatusOptions } from '../../proxy/insurance/patient-insurance-status.enum';

@Component({
  selector: 'app-patient-insurance',
  standalone: true,
  imports: [CommonModule, FormsModule, ThemeSharedModule, CoreModule],
  template: `
    <div class="card">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h5 class="mb-0">
          <i class="fas fa-id-card-alt me-2"></i> تأمين المريض
        </h5>
        <button class="btn btn-sm btn-primary" (click)="showForm = true; editingItem = null; resetForm()">
          <i class="fas fa-plus me-1"></i> إضافة تأمين
        </button>
      </div>
      <div class="card-body p-0">
        <div class="list-group list-group-flush">
          @for (item of items; track item.id) {
            <div class="list-group-item p-3" [class.bg-light]="!item.isPrimary">
              <div class="d-flex justify-content-between align-items-start">
                <div>
                  <h6 class="mb-1 d-flex align-items-center">
                    <span class="badge rounded-pill me-2" 
                          [class.bg-success]="item.status === 0"
                          [class.bg-danger]="item.status === 1 || item.status === 2"
                          [class.bg-warning]="item.status === 3">
                      {{ '::Enum:PatientInsuranceStatus:' + item.status | abpLocalization }}
                    </span>
                    {{ item.insuranceCompanyName }} — {{ item.insurancePlanName }}
                    @if (item.isPrimary) {
                      <span class="badge bg-primary ms-2"><i class="fas fa-star me-1"></i> رئيسي</span>
                    }
                  </h6>
                  <p class="mb-1 text-muted small">
                    <i class="fas fa-hashtag me-1"></i> بوليصة: <strong>{{ item.policyNumber }}</strong> | 
                    بطاقة: <strong>{{ item.cardNumber }}</strong>
                  </p>
                  <p class="mb-0 small text-muted">
                    <i class="far fa-calendar-alt me-1"></i> الصلاحية: 
                    {{ item.startDate | date:'mediumDate' }} - {{ item.endDate | date:'mediumDate' }}
                  </p>
                </div>
                <div>
                  <button class="btn btn-sm btn-outline-primary me-1" (click)="edit(item)">
                    <i class="fas fa-edit"></i>
                  </button>
                  <button class="btn btn-sm btn-outline-danger" (click)="delete(item)">
                    <i class="fas fa-trash"></i>
                  </button>
                </div>
              </div>
            </div>
          } @empty {
            <div class="p-4 text-center text-muted">
              لا توجد بطاقات تأمين مسجلة لهذا المريض
            </div>
          }
        </div>
      </div>
    </div>

    <!-- Modal Form -->
    @if (showForm) {
      <div class="modal show d-block" style="background: rgba(0,0,0,0.5); z-index: 1050;">
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">{{ editingItem ? 'تعديل' : 'إضافة' }} تأمين</h5>
              <button type="button" class="btn-close" (click)="showForm = false"></button>
            </div>
            <div class="modal-body">
              <div class="mb-3">
                <label class="form-label">خطة التأمين *</label>
                <select class="form-select" [(ngModel)]="formData.insurancePlanId" required>
                  <option value="">اختر الخطة</option>
                  @for (plan of plans; track plan.id) {
                    <option [value]="plan.id">{{ plan.nameAr || plan.nameEn }} ({{ plan.insuranceCompanyName }})</option>
                  }
                </select>
              </div>

              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">رقم البوليصة *</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.policyNumber" required>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">رقم البطاقة *</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.cardNumber" required>
                </div>
              </div>

              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">تاريخ البداية *</label>
                  <input type="date" class="form-control" [(ngModel)]="formData.startDate" required>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">تاريخ الانتهاء *</label>
                  <input type="date" class="form-control" [(ngModel)]="formData.endDate" required>
                </div>
              </div>

              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">الحالة</label>
                  <select class="form-select" [(ngModel)]="formData.status">
                    @for (opt of statusOptions; track opt.value) {
                      <option [value]="opt.value">{{ '::Enum:PatientInsuranceStatus:' + opt.value | abpLocalization }}</option>
                    }
                  </select>
                </div>
                <div class="col-md-6 mb-3 d-flex align-items-end">
                  <div class="form-check mb-2">
                    <input type="checkbox" class="form-check-input" id="isPrimary" [(ngModel)]="formData.isPrimary">
                    <label class="form-check-label" for="isPrimary">تأمين رئيسي</label>
                  </div>
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
                <i class="fas fa-save me-1"></i> حفظ
              </button>
            </div>
          </div>
        </div>
      </div>
    }
  `
})
export class PatientInsuranceComponent implements OnInit {
  @Input() patientId!: string;

  private patientInsuranceService = inject(PatientInsuranceService);
  private planService = inject(InsurancePlanService);
  private confirmation = inject(ConfirmationService);

  items: PatientInsuranceDto[] = [];
  plans: any[] = [];
  statusOptions = patientInsuranceStatusOptions;

  showForm = false;
  editingItem: PatientInsuranceDto | null = null;
  formData: CreateUpdatePatientInsuranceDto = this.getEmptyForm();

  ngOnInit() {
    if (this.patientId) {
      this.loadPlans();
      this.loadData();
    }
  }

  getEmptyForm(): CreateUpdatePatientInsuranceDto {
    return {
      patientId: this.patientId,
      insurancePlanId: '',
      policyNumber: '',
      cardNumber: '',
      startDate: new Date().toISOString().split('T')[0],
      endDate: new Date(new Date().setFullYear(new Date().getFullYear() + 1)).toISOString().split('T')[0],
      isPrimary: this.items.length === 0,
      status: PatientInsuranceStatus.Active,
      notes: ''
    };
  }

  resetForm() { 
    this.formData = this.getEmptyForm(); 
  }

  loadPlans() {
    this.planService.getList({ maxResultCount: 1000 } as any).subscribe({
      next: (res) => this.plans = res.items,
      error: (err) => console.error(err)
    });
  }

  loadData() {
    this.patientInsuranceService.getByPatient(this.patientId).subscribe({
      next: (res) => this.items = res,
      error: (err) => console.error(err)
    });
  }

  edit(item: PatientInsuranceDto) {
    this.editingItem = item;
    this.formData = { 
      ...item, 
      startDate: item.startDate ? item.startDate.split('T')[0] : '',
      endDate: item.endDate ? item.endDate.split('T')[0] : ''
    };
    this.showForm = true;
  }

  save() {
    if (!this.formData.insurancePlanId || !this.formData.policyNumber || !this.formData.cardNumber) return;
    
    this.formData.patientId = this.patientId;
    
    const req = this.editingItem?.id
      ? this.patientInsuranceService.update(this.editingItem.id, this.formData)
      : this.patientInsuranceService.create(this.formData);
      
    req.subscribe({
      next: () => { this.showForm = false; this.loadData(); },
      error: (err) => console.error(err)
    });
  }

  delete(item: PatientInsuranceDto) {
    if (!item.id) return;
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.patientInsuranceService.delete(item.id as string).subscribe({ next: () => this.loadData() });
      }
    });
  }
}
