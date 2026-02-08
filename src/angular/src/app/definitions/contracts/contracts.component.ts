import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ContractService } from '../../proxy/general/contract.service';
import { ContractDto } from '../../proxy/general/models';

@Component({
  selector: 'app-contracts',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule, ThemeSharedModule, CoreModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card shadow-sm border-0">
        <div class="card-header d-flex justify-content-between align-items-center bg-primary text-white py-3">
          <h5 class="mb-0 fw-bold">
            <i class="fas fa-file-contract me-2"></i>
            التعاقدات - Contracts
          </h5>
          <button class="btn btn-light btn-sm px-3 shadow-sm" (click)="showForm = true; editingItem = null; resetForm()">
            <i class="fas fa-plus me-1"></i> إضافة - Add
          </button>
        </div>
        <div class="card-body p-4">
          <!-- Search Row -->
          <div class="row g-3 mb-4">
            <div class="col-md-5 col-lg-4">
              <div class="input-group">
                <span class="input-group-text bg-light border-end-0 text-muted"><i class="fas fa-search"></i></span>
                <input type="text" class="form-control bg-light border-start-0 ps-0" 
                       placeholder="بحث بالاسم أو الكود... Search..." 
                       [(ngModel)]="searchText" (input)="search()">
              </div>
            </div>
          </div>

          <!-- Data Grid -->
          <div class="table-responsive">
            <table class="table table-hover align-middle border-top">
              <thead class="bg-light">
                <tr class="text-secondary small text-uppercase">
                  <th class="ps-3" style="width: 15%">الكود - Code</th>
                  <th style="width: 30%">الاسم (عربي) - Name (Ar)</th>
                  <th style="width: 30%">الاسم (إنجليزي) - Name (En)</th>
                  <th class="text-center">الحالة - Status</th>
                  <th class="text-end pe-3">الإجراءات - Actions</th>
                </tr>
              </thead>
              <tbody class="text-dark">
                @for (item of items; track item.id) {
                  <tr>
                    <td class="ps-3"><code class="text-primary fw-medium">{{ item.code || '---' }}</code></td>
                    <td class="fw-semibold">{{ item.nameAr }}</td>
                    <td class="text-muted">{{ item.nameEn }}</td>
                    <td class="text-center">
                      <span [class]="item.isActive ? 'badge rounded-pill bg-success-subtle text-success border border-success' : 'badge rounded-pill bg-secondary-subtle text-secondary border border-secondary'" style="padding: 0.5em 1em;">
                        <i class="fas fa-circle small me-1" [class.text-success]="item.isActive" [class.text-secondary]="!item.isActive"></i>
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
                      <div class="d-flex flex-column align-items-center">
                        <i class="fas fa-inbox fa-3x mb-3 text-light"></i>
                        <p class="mb-0">لا توجد بيانات حالياً - No data available</p>
                      </div>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>

          <!-- Pagination -->
          <div class="d-flex flex-column flex-sm-row justify-content-between align-items-center mt-4 pt-3 border-top" *ngIf="totalCount > 0">
            <span class="text-muted small mb-3 mb-sm-0">
              Showing <strong>{{ items.length }}</strong> of <strong>{{ totalCount }}</strong> entries
            </span>
            <ngb-pagination
              [(page)]="page"
              [pageSize]="pageSize"
              [collectionSize]="totalCount"
              (pageChange)="onPageChange($event)"
              [maxSize]="5"
              [rotate]="true"
              [boundaryLinks]="true"
              class="pagination-sm mb-0">
            </ngb-pagination>
          </div>
        </div>
      </div>

      <!-- Enhanced Modal Form -->
      @if (showForm) {
        <div class="modal fade show d-block" style="background: rgba(15, 23, 42, 0.75); backdrop-filter: blur(4px); z-index: 1060;">
          <div class="modal-dialog modal-dialog-centered shadow">
            <div class="modal-content border-0 shadow-2xl">
              <div class="modal-header border-0 pb-0">
                <h5 class="modal-title fw-bold text-dark d-flex align-items-center">
                  <span class="bg-primary-subtle text-primary rounded p-2 me-3">
                    <i class="fas fa-file-contract"></i>
                  </span>
                  {{ editingItem ? 'تعديل - Edit' : 'إضافة - Add' }} تعاقد - Contract
                </h5>
                <button type="button" class="btn-close shadow-none" (click)="showForm = false"></button>
              </div>
              <div class="modal-body p-4 pt-4 text-dark">
                <div class="mb-4">
                  <label class="form-label fw-semibold mb-1 small text-secondary">الاسم (عربي) - Name (Ar) <span class="text-danger">*</span></label>
                  <input type="text" class="form-control form-control-lg border-light-subtle bg-light-subtle shadow-sm" 
                         [(ngModel)]="formData.nameAr" required placeholder="مثلاً: شركة أرامكو">
                </div>
                <div class="mb-4">
                  <label class="form-label fw-semibold mb-1 small text-secondary">الاسم (إنجليزي) - Name (En) <span class="text-danger">*</span></label>
                  <input type="text" class="form-control form-control-lg border-light-subtle bg-light-subtle shadow-sm" 
                         [(ngModel)]="formData.nameEn" required placeholder="e.g., Aramco Company">
                </div>
                <div class="row g-3 mb-4">
                  <div class="col-md-7">
                    <label class="form-label fw-semibold mb-1 small text-secondary">{{ '::Code' | abpLocalization }}</label>
                    <input type="text" class="form-control border-light-subtle bg-light-subtle shadow-sm" 
                           [(ngModel)]="formData.code" 
                           [placeholder]="editingItem ? '' : ('::AutoGeneratedCode' | abpLocalization)" 
                           [disabled]="!editingItem">
                  </div>
                  <div class="col-md-5 d-flex align-items-end pb-1">
                    <div class="form-check form-switch custom-switch p-0 ms-3">
                      <label class="form-check-label fw-semibold small text-secondary me-2 cursor-pointer" for="isActiveToggle">
                        {{ formData.isActive ? 'نشط - Active' : 'غير نشط - Inactive' }}
                      </label>
                      <input type="checkbox" class="form-check-input ms-0 shadow-none cursor-pointer" 
                             [(ngModel)]="formData.isActive" id="isActiveToggle">
                    </div>
                  </div>
                </div>
              </div>
              <div class="modal-footer border-0 bg-light-subtle p-3 rounded-bottom-4">
                <button type="button" class="btn btn-link text-secondary text-decoration-none px-4 me-auto" (click)="showForm = false" [disabled]="loading">
                  إلغاء - Cancel
                </button>
                <button type="button" class="btn btn-primary px-5 shadow-sm rounded-3" (click)="save()" [disabled]="loading || !formData.nameAr || !formData.nameEn">
                  @if (loading) {
                    <span class="spinner-border spinner-border-sm me-2"></span>
                    جاري الحفظ...
                  } @else {
                    <i class="fas fa-save me-2"></i>
                    حفظ - Save
                  }
                </button>
              </div>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .cursor-pointer { cursor: pointer; }
    .custom-switch .form-check-input { width: 3em; height: 1.5em; }
    .btn-white { background: white; border: none; }
    .btn-white:hover { background: #f8fafc; }
    .modal-content { border-radius: 1.25rem; }
    .shadow-2xl { box-shadow: 0 25px 50px -12px rgb(0 0 0 / 0.25); }
    .bg-success-subtle { background-color: #f0fdf4 !important; }
    .bg-primary-subtle { background-color: #eff6ff !important; }
    .text-success { color: #16a34a !important; }
    .border-success { border-color: #bbf7d0 !important; }
    .table thead th { border-bottom: none; border-top: none; }
    .form-control-lg { font-size: 1rem; }
  `]
})
export class ContractsComponent implements OnInit {
  private service = inject(ContractService);
  private confirmation = inject(ConfirmationService);

  items: ContractDto[] = [];
  searchText = '';
  showForm = false;
  editingItem: ContractDto | null = null;
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

  edit(item: ContractDto) {
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

  delete(item: ContractDto) {
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
