import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { PatientCategoryService } from '../../proxy/general/patient-category.service';
import { PatientCategoryDto } from '../../proxy/general/models';

@Component({
  selector: 'app-patient-categories',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule, ThemeSharedModule, CoreModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card shadow-sm border-0">
        <div class="card-header d-flex justify-content-between align-items-center bg-warning text-dark py-3">
          <h5 class="mb-0 fw-bold">
            <i class="fas fa-tags me-2"></i>
            فئات المرضى - Patient Categories
          </h5>
          <button class="btn btn-dark btn-sm px-3 shadow-sm" (click)="showForm = true; editingItem = null; resetForm()">
            <i class="fas fa-plus me-1"></i> إضافة - Add
          </button>
        </div>
        <div class="card-body p-4">
          <!-- Search -->
          <div class="row g-3 mb-4">
            <div class="col-md-5 col-lg-4">
              <div class="input-group">
                <span class="input-group-text bg-light border-end-0 text-muted"><i class="fas fa-search"></i></span>
                <input type="text" class="form-control bg-light border-start-0 ps-0 text-dark" 
                       placeholder="بحث... Search..." 
                       [(ngModel)]="searchText" (input)="search()">
              </div>
            </div>
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-hover align-middle border-top">
              <thead class="bg-light">
                <tr class="text-secondary small text-uppercase">
                  <th class="ps-3">الكود - Code</th>
                  <th>الاسم (عربي) - Name (Ar)</th>
                  <th>الاسم (إنجليزي) - Name (En)</th>
                  <th class="text-center">الحالة - Status</th>
                  <th class="text-end pe-3">الإجراءات - Actions</th>
                </tr>
              </thead>
              <tbody class="text-dark">
                @for (item of items; track item.id) {
                  <tr>
                    <td class="ps-3"><code class="text-warning fw-bold">{{ item.code || '---' }}</code></td>
                    <td class="fw-semibold">{{ item.nameAr }}</td>
                    <td class="text-muted">{{ item.nameEn }}</td>
                    <td class="text-center">
                      <span [class]="item.isActive ? 'badge rounded-pill bg-success-subtle text-success border border-success' : 'badge rounded-pill bg-secondary-subtle text-secondary border border-secondary'" style="padding: 0.5em 1em;">
                        {{ item.isActive ? 'نشط - Active' : 'غير نشط - Inactive' }}
                      </span>
                    </td>
                    <td class="text-end pe-3">
                      <div class="btn-group btn-group-sm shadow-sm border rounded">
                        <button class="btn btn-white text-primary px-3" (click)="edit(item)" title="Edit">
                          <i class="fas fa-edit"></i>
                        </button>
                        <button class="btn btn-white text-danger px-3 border-start" (click)="delete(item)" title="Delete">
                          <i class="fas fa-trash-alt"></i>
                        </button>
                      </div>
                    </td>
                  </tr>
                } @empty {
                  <tr>
                    <td colspan="5" class="text-center py-5 text-muted">
                      <i class="fas fa-folder-open fa-3x mb-3 text-light"></i>
                      <p>لا توجد فئات حالياً - No categories available</p>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>

          <!-- Pagination -->
          <div class="d-flex justify-content-between align-items-center mt-4 pt-3 border-top" *ngIf="totalCount > 0">
            <span class="text-muted small">Total: {{ totalCount }}</span>
            <ngb-pagination
              [(page)]="page"
              [pageSize]="pageSize"
              [collectionSize]="totalCount"
              (pageChange)="onPageChange($event)"
              [maxSize]="5"
              [boundaryLinks]="true"
              class="pagination-sm mb-0">
            </ngb-pagination>
          </div>
        </div>
      </div>

      <!-- Modal -->
      @if (showForm) {
        <div class="modal fade show d-block" style="background: rgba(0,0,0,0.6); backdrop-filter: blur(2px); z-index: 1060;">
          <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 shadow-lg">
              <div class="modal-header bg-warning border-0">
                <h5 class="modal-title fw-bold">
                  <i class="fas fa-tags me-2"></i>
                  {{ editingItem ? 'تعديل - Edit' : 'إضافة - Add' }} فئة - Category
                </h5>
                <button type="button" class="btn-close" (click)="showForm = false"></button>
              </div>
              <div class="modal-body p-4 text-dark">
                <div class="mb-3">
                  <label class="form-label fw-bold small">الاسم (عربي) - Name (Ar) *</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.nameAr" required placeholder="مثلاً: كبار الشخصيات">
                </div>
                <div class="mb-3">
                  <label class="form-label fw-bold small">الاسم (إنجليزي) - Name (En) *</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.nameEn" required placeholder="e.g., VIP">
                </div>
                <div class="mb-3">
                  <label class="form-label fw-bold small">{{ '::Code' | abpLocalization }}</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.code" 
                         [placeholder]="editingItem ? '' : ('::AutoGeneratedCode' | abpLocalization)" 
                         [disabled]="!editingItem">
                </div>
                <div class="form-check form-switch mb-3 mt-4">
                  <input type="checkbox" class="form-check-input" [(ngModel)]="formData.isActive" id="isCatActive">
                  <label class="form-check-label fw-bold ms-2" for="isCatActive">نشط - Active</label>
                </div>
              </div>
              <div class="modal-footer border-0 p-3 bg-light rounded-bottom">
                <button type="button" class="btn btn-outline-secondary px-4 me-auto" (click)="showForm = false" [disabled]="loading">إلغاء</button>
                <button type="button" class="btn btn-dark px-5" (click)="save()" [disabled]="loading || !formData.nameAr || !formData.nameEn">
                  {{ loading ? 'جاري الحفظ...' : 'حفظ - Save' }}
                </button>
              </div>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .btn-white { background: white; border: none; }
    .btn-white:hover { background: #f8fafc; }
    .bg-success-subtle { background-color: #f0fdf4 !important; }
    .text-success { color: #16a34a !important; }
    .border-success { border-color: #bbf7d0 !important; }
  `]
})
export class PatientCategoriesComponent implements OnInit {
  private service = inject(PatientCategoryService);
  private confirmation = inject(ConfirmationService);

  items: PatientCategoryDto[] = [];
  searchText = '';
  showForm = false;
  editingItem: PatientCategoryDto | null = null;
  formData: any = this.getEmptyForm();
  loading = false;

  // Pagination
  page = 1;
  pageSize = 10;
  totalCount = 0;

  ngOnInit() {
    this.loadData();
  }

  getEmptyForm() {
    return { nameAr: '', nameEn: '', code: '', isActive: true };
  }

  resetForm() {
    this.formData = this.getEmptyForm();
  }

  loadData() {
    const skipCount = (this.page - 1) * this.pageSize;
    this.service.getList({
      filter: this.searchText,
      sorting: '',
      skipCount,
      maxResultCount: this.pageSize
    } as any).subscribe({
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

  edit(item: PatientCategoryDto) {
    this.editingItem = item;
    this.formData = { ...item };
    this.showForm = true;
  }

  save() {
    this.loading = true;
    const request = this.editingItem
      ? this.service.update(this.editingItem.id, this.formData)
      : this.service.create(this.formData);

    request.subscribe({
      next: () => {
        this.loading = false;
        this.showForm = false;
        this.loadData();
      },
      error: (err) => {
        this.loading = false;
        console.error(err);
      }
    });
  }

  delete(item: PatientCategoryDto) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.service.delete(item.id).subscribe({
          next: () => this.loadData(),
          error: (err) => console.error(err)
        });
      }
    });
  }
}
