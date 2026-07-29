import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import { InsuranceServicePriceService } from '../../proxy/insurance/insurance-service-price.service';
import { InsurancePlanService } from '../../proxy/insurance/insurance-plan.service';
import { ServiceItemService } from '../../proxy/services/service-item.service';
import { InsuranceServicePriceDto, CreateUpdateInsuranceServicePriceDto, LookupDto } from '../../proxy/insurance/models';
import { ServiceItemDto } from '../../proxy/services/models';

@Component({
  selector: 'app-insurance-service-prices',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule, ThemeSharedModule, CoreModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">
            <i class="fas fa-tags me-2"></i>
            {{ '::Menu:InsuranceServicePrices' | abpLocalization }}
          </h5>
          <button class="btn btn-primary" (click)="showForm = true; editingItem = null; resetForm()">
            <i class="fas fa-plus me-1"></i> {{ '::Add' | abpLocalization }}
          </button>
        </div>
        <div class="card-body">
          <!-- Filters -->
          <div class="row mb-3">
            <div class="col-md-4">
              <label class="form-label small fw-bold">خطة التأمين</label>
              <select class="form-select" [(ngModel)]="filterPlanId" (change)="search()">
                <option value="">-- كل الخطط --</option>
                @for (plan of plans; track plan.id) {
                  <option [value]="plan.id">{{ plan.nameAr || plan.nameEn }}</option>
                }
              </select>
            </div>
            <div class="col-md-4">
              <label class="form-label small fw-bold">&nbsp;</label>
              <button class="btn btn-outline-secondary d-block" (click)="search()">
                <i class="fas fa-search me-1"></i> بحث
              </button>
            </div>
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-striped table-hover">
              <thead class="table-dark">
                <tr>
                  <th>الخطة</th>
                  <th>الخدمة</th>
                  <th>الكود</th>
                  <th>السعر المخصص</th>
                  <th>ملاحظات</th>
                  <th>الإجراءات</th>
                </tr>
              </thead>
              <tbody>
                @for (item of items; track item.id) {
                  <tr>
                    <td>{{ item.insurancePlanName }}</td>
                    <td>{{ item.serviceItemName }}</td>
                    <td>{{ item.serviceItemCode }}</td>
                    <td class="text-primary fw-bold">{{ item.customPrice }}</td>
                    <td>{{ item.notes }}</td>
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
            <span class="text-muted">الإجمالي: {{ totalCount }}</span>
          </div>
        </div>
      </div>

      <!-- Modal Form -->
      @if (showForm) {
        <div class="modal show d-block" style="background: rgba(0,0,0,0.5)">
          <div class="modal-dialog">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title">{{ editingItem ? 'تعديل' : 'إضافة' }} سعر خدمة للتأمين</h5>
                <button type="button" class="btn-close" (click)="showForm = false"></button>
              </div>
              <div class="modal-body">
                <div class="mb-3">
                  <label class="form-label">خطة التأمين *</label>
                  <select class="form-select" [(ngModel)]="formData.insurancePlanId" required>
                    <option value="">اختر الخطة</option>
                    @for (plan of plans; track plan.id) {
                      <option [value]="plan.id">{{ plan.nameAr || plan.nameEn }}</option>
                    }
                  </select>
                </div>
                
                <div class="mb-3">
                  <label class="form-label">الخدمة *</label>
                  <select class="form-select" [(ngModel)]="formData.serviceItemId" required>
                    <option value="">اختر الخدمة</option>
                    @for (svc of services; track svc.id) {
                      <option [value]="svc.id">{{ svc.nameAr || svc.nameEn }} ({{ svc.code }})</option>
                    }
                  </select>
                </div>

                <div class="mb-3">
                  <label class="form-label">السعر المخصص *</label>
                  <input type="number" class="form-control" [(ngModel)]="formData.customPrice" required min="0">
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
    </div>
  `,
  styles: [`.modal { z-index: 1050; }`]
})
export class InsuranceServicePricesComponent implements OnInit {
  private priceService = inject(InsuranceServicePriceService);
  private planService = inject(InsurancePlanService);
  private serviceItemService = inject(ServiceItemService);
  private confirmation = inject(ConfirmationService);

  items: InsuranceServicePriceDto[] = [];
  plans: any[] = [];
  services: ServiceItemDto[] = [];

  filterPlanId = '';
  showForm = false;
  editingItem: InsuranceServicePriceDto | null = null;
  formData: CreateUpdateInsuranceServicePriceDto = this.getEmptyForm();

  page = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit() {
    this.loadPlans();
    this.loadServices();
    this.loadData();
  }

  getEmptyForm(): CreateUpdateInsuranceServicePriceDto {
    return {
      insurancePlanId: '',
      serviceItemId: '',
      customPrice: 0,
      notes: ''
    };
  }

  resetForm() { this.formData = this.getEmptyForm(); }

  loadPlans() {
    this.planService.getList({ maxResultCount: 1000 } as any).subscribe({
      next: (res) => this.plans = res.items,
      error: (err) => console.error(err)
    });
  }

  loadServices() {
    this.serviceItemService.getList({ maxResultCount: 1000 } as any).subscribe({
      next: (res) => this.services = res.items,
      error: (err) => console.error(err)
    });
  }

  loadData() {
    const skipCount = (this.page - 1) * this.pageSize;
    this.priceService.getList({
      insurancePlanId: this.filterPlanId || undefined,
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

  edit(item: InsuranceServicePriceDto) {
    this.editingItem = item;
    this.formData = { ...item };
    this.showForm = true;
  }

  save() {
    if (!this.formData.insurancePlanId || !this.formData.serviceItemId) return;
    
    const req = this.editingItem?.id
      ? this.priceService.update(this.editingItem.id, this.formData)
      : this.priceService.create(this.formData);
      
    req.subscribe({
      next: () => { this.showForm = false; this.loadData(); },
      error: (err) => console.error(err)
    });
  }

  delete(item: InsuranceServicePriceDto) {
    if (!item.id) return;
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.priceService.delete(item.id as string).subscribe({ next: () => this.loadData() });
      }
    });
  }
}
