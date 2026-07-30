import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import { InsurancePlanService } from '../../proxy/insurance/insurance-plan.service';
import { InsuranceCompanyService } from '../../proxy/insurance/insurance-company.service';
import { InsurancePlanDto, CreateUpdateInsurancePlanDto, LookupDto } from '../../proxy/insurance/models';
import { InsurancePlanType, insurancePlanTypeOptions } from '../../proxy/insurance/insurance-plan-type.enum';
import { InsurancePlanClass, insurancePlanClassOptions } from '../../proxy/insurance/insurance-plan-class.enum';

@Component({
  selector: 'app-insurance-plans',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule, ThemeSharedModule, CoreModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-file-medical me-2"></i>
            خطط التأمين - Insurance Plans
          </h5>
          <button class="btn btn-primary" (click)="showForm = true; editingItem = null; resetForm()">
            <i class="fas fa-plus me-1"></i> إضافة
          </button>
        </div>
        <div class="card-body">
          <!-- Filters -->
          <div class="row mb-3">
            <div class="col-md-4">
              <div class="input-group">
                <span class="input-group-text"><i class="fas fa-search"></i></span>
                <input type="text" class="form-control" placeholder="بحث..." 
                       [(ngModel)]="searchText" (input)="search()">
              </div>
            </div>
            <div class="col-md-3">
              <select class="form-select" [(ngModel)]="filterCompanyId" (change)="search()">
                <option value="">كل الشركات</option>
                @for (comp of companies; track comp.id) {
                  <option [value]="comp.id">{{ comp.name }}</option>
                }
              </select>
            </div>
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-striped table-hover">
              <thead class="table-dark">
                <tr>
                  <th>الكود</th>
                  <th>الشركة</th>
                  <th>الخطة</th>
                  <th>النوع</th>
                  <th>الفئة</th>
                  <th>نسبة التغطية</th>
                  <th>المشاركة</th>
                  <th>الحالة</th>
                  <th>الإجراءات</th>
                </tr>
              </thead>
              <tbody>
                @for (item of items; track item.id) {
                  <tr>
                    <td>{{ item.code }}</td>
                    <td>{{ item.insuranceCompanyName }}</td>
                    <td>{{ item.nameAr }}</td>
                    <td>{{ getPlanTypeName(item.planType) }}</td>
                    <td>{{ getPlanClassName(item.planClass) }}</td>
                    <td>{{ item.coveragePercentage }}%</td>
                    <td>{{ item.coPaymentPercentage }}%</td>
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
                <h5 class="modal-title">{{ editingItem ? 'تعديل' : 'إضافة' }} خطة تأمين</h5>
                <button type="button" class="btn-close" (click)="showForm = false"></button>
              </div>
              <div class="modal-body">
                <div class="row">
                  <div class="col-md-6 mb-3">
                    <label class="form-label">شركة التأمين *</label>
                    <select class="form-select" [(ngModel)]="formData.insuranceCompanyId" required>
                      <option value="">اختر الشركة</option>
                      @for (comp of companies; track comp.id) {
                        <option [value]="comp.id">{{ comp.name }}</option>
                      }
                    </select>
                  </div>
                  <div class="col-md-3 mb-3">
                    <label class="form-label">نوع الخطة</label>
                    <select class="form-select" [(ngModel)]="formData.planType">
                      @for (opt of planTypeOptions; track opt.value) {
                        <option [value]="opt.value">{{ getPlanTypeName(opt.value) }}</option>
                      }
                    </select>
                  </div>
                  <div class="col-md-3 mb-3">
                    <label class="form-label">الفئة (Class)</label>
                    <select class="form-select" [(ngModel)]="formData.planClass">
                      @for (opt of planClassOptions; track opt.value) {
                        <option [value]="opt.value">{{ getPlanClassName(opt.value) }}</option>
                      }
                    </select>
                  </div>
                </div>

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
                  <div class="col-md-4 mb-3">
                    <label class="form-label">نسبة التغطية (%)</label>
                    <input type="number" class="form-control" [(ngModel)]="formData.coveragePercentage" min="0" max="100">
                  </div>
                  <div class="col-md-4 mb-3">
                    <label class="form-label">نسبة المشاركة (%)</label>
                    <input type="number" class="form-control" [(ngModel)]="formData.coPaymentPercentage" min="0" max="100">
                  </div>
                  <div class="col-md-4 mb-3">
                    <label class="form-label">قيمة الخصم (Deductible)</label>
                    <input type="number" class="form-control" [(ngModel)]="formData.deductibleAmount" min="0">
                  </div>
                </div>

                <div class="row mb-3">
                  <div class="col-md-6">
                    <label class="form-label">الحد الأقصى للتغطية</label>
                    <input type="number" class="form-control" [(ngModel)]="formData.maxCoverageAmount" min="0">
                  </div>
                </div>

                <div class="row mb-3">
                  <div class="col-md-3">
                    <div class="form-check">
                      <input type="checkbox" class="form-check-input" [(ngModel)]="formData.includesMedications" id="meds">
                      <label class="form-check-label" for="meds">يشمل الأدوية</label>
                    </div>
                  </div>
                  <div class="col-md-3">
                    <div class="form-check">
                      <input type="checkbox" class="form-check-input" [(ngModel)]="formData.includesLab" id="lab">
                      <label class="form-check-label" for="lab">يشمل المختبر</label>
                    </div>
                  </div>
                  <div class="col-md-3">
                    <div class="form-check">
                      <input type="checkbox" class="form-check-input" [(ngModel)]="formData.includesRadiology" id="rad">
                      <label class="form-check-label" for="rad">يشمل الأشعة</label>
                    </div>
                  </div>
                  <div class="col-md-3">
                    <div class="form-check">
                      <input type="checkbox" class="form-check-input" [(ngModel)]="formData.includesInpatient" id="inp">
                      <label class="form-check-label" for="inp">يشمل التنويم</label>
                    </div>
                  </div>
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
export class InsurancePlansComponent implements OnInit {
  private planService = inject(InsurancePlanService);
  private companyService = inject(InsuranceCompanyService);
  private confirmation = inject(ConfirmationService);

  items: InsurancePlanDto[] = [];
  companies: LookupDto[] = [];

  planTypeOptions = insurancePlanTypeOptions;
  planClassOptions = insurancePlanClassOptions;

  searchText = '';
  filterCompanyId = '';
  showForm = false;
  editingItem: InsurancePlanDto | null = null;
  formData: CreateUpdateInsurancePlanDto = this.getEmptyForm();

  page = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit() {
    this.loadCompanies();
    this.loadData();
  }

  getEmptyForm(): CreateUpdateInsurancePlanDto {
    return {
      insuranceCompanyId: '', nameAr: '', nameEn: '', planType: InsurancePlanType.Individual, planClass: InsurancePlanClass.ClassA,
      coveragePercentage: 80, coPaymentPercentage: 20, deductibleAmount: 0,
      includesMedications: true, includesLab: true, includesRadiology: true, includesInpatient: false,
      isActive: true, sortOrder: 0
    };
  }

  resetForm() { this.formData = this.getEmptyForm(); }

  loadCompanies() {
    this.companyService.getLookup().subscribe({
      next: (res) => this.companies = res,
      error: (err) => console.error(err)
    });
  }

  loadData() {
    const skipCount = (this.page - 1) * this.pageSize;
    this.planService.getList({
      searchText: this.searchText,
      insuranceCompanyId: this.filterCompanyId || undefined,
      skipCount: skipCount,
      maxResultCount: this.pageSize
    }).subscribe({
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

  edit(item: InsurancePlanDto) {
    this.editingItem = item;
    this.formData = { ...item };
    this.showForm = true;
  }


  save() {
    const req = this.editingItem?.id
      ? this.planService.update(this.editingItem.id, this.formData)
      : this.planService.create(this.formData);
    req.subscribe({
      next: () => { this.showForm = false; this.search(); },
      error: (err) => console.error(err)
    });
  }

  delete(item: InsurancePlanDto) {
    if (!item.id) return;
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.planService.delete(item.id as string).subscribe({ next: () => this.search() });
      }
    });
  }

  getPlanTypeName(typeValue: any): string {
    const val = Number(typeValue);
    switch (val) {
      case 0: return 'أفراد';
      case 1: return 'عائلي';
      case 2: return 'شركات';
      case 3: return 'حكومي';
      default: return 'غير معروف';
    }
  }

  getPlanClassName(classValue: any): string {
    const val = Number(classValue);
    switch (val) {
      case 0: return 'فئة أ (Class A)';
      case 1: return 'فئة ب (Class B)';
      case 2: return 'فئة ج (Class C)';
      default: return 'غير معروف';
    }
  }
}
