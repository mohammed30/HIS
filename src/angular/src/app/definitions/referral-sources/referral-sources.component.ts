import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ReferralSourceService } from '../../proxy/general/referral-source.service';
import { ReferralSourceDto } from '../../proxy/general/models';

@Component({
  selector: 'app-referral-sources',
  standalone: true,
  imports: [CommonModule, FormsModule, NgbPaginationModule, ThemeSharedModule, CoreModule],
  template: `
    <div class="container-fluid py-4">
      <div class="card shadow-sm border-0">
        <div class="card-header d-flex justify-content-between align-items-center bg-danger text-white py-3">
          <h5 class="mb-0 fw-bold">
            <i class="fas fa-handshake me-2"></i>
            الجهات المحولة - Referral Sources
          </h5>
          <button class="btn btn-light btn-sm px-3 shadow-sm text-danger fw-bold" (click)="showForm = true; editingItem = null; resetForm()">
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
                       placeholder="بحث بالاسم... Search..." 
                       [(ngModel)]="searchText" (input)="search()">
              </div>
            </div>
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-hover align-middle border-top">
              <thead class="bg-light">
                <tr class="text-secondary small text-uppercase">
                  <th class="ps-3" style="width: 20%">الكود - Code</th>
                  <th style="width: 55%">الاسم (عربي) - Name (Ar)</th>
                  <th class="text-center">الحالة - Status</th>
                  <th class="text-end pe-3">الإجراءات - Actions</th>
                </tr>
              </thead>
              <tbody class="text-dark">
                @for (item of items; track item.id) {
                  <tr>
                    <td class="ps-3"><code class="text-danger fw-medium">{{ item.code || '---' }}</code></td>
                    <td class="fw-semibold">{{ item.nameAr }}</td>
                    <td class="text-center">
                      <span [class]="item.isActive ? 'badge rounded-pill bg-danger-subtle text-danger border border-danger-subtle' : 'badge rounded-pill bg-secondary-subtle text-secondary border border-secondary'" style="padding: 0.5em 1.2em;">
                        {{ item.isActive ? 'نشط - Active' : 'غير نشط - Inactive' }}
                      </span>
                    </td>
                    <td class="text-end pe-3">
                      <div class="btn-group btn-group-sm shadow-sm border rounded overflow-hidden">
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
                    <td colspan="5" class="text-center py-5">
                      <div class="py-4">
                        <i class="fas fa-link fa-3x mb-3 text-light"></i>
                        <p class="text-muted mb-0 font-italic">لا يوجد بيانات للجهات المحولة - No referral sources found</p>
                      </div>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>

          <!-- Pagination -->
          <div class="d-flex justify-content-between align-items-center mt-4 pt-3 border-top" *ngIf="totalCount > 0">
            <span class="text-muted small">Showing <strong>{{ items.length }}</strong> results</span>
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

      <!-- Modal -->
      @if (showForm) {
        <div class="modal fade show d-block" style="background: rgba(15, 23, 42, 0.7); backdrop-filter: blur(4px); z-index: 1060;">
          <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 shadow-2xl overflow-hidden">
              <div class="modal-header bg-danger text-white border-0 py-3">
                <h5 class="modal-title fw-bold">
                  <i class="fas fa-handshake me-2"></i>
                  {{ editingItem ? 'تعديل - Edit' : 'إضافة - Add' }} جهة محولة - Referral Source
                </h5>
                <button type="button" class="btn-close btn-close-white" (click)="showForm = false"></button>
              </div>
              <div class="modal-body p-4 pt-4">
                <div class="mb-4">
                  <label class="form-label fw-bold small text-uppercase text-secondary">الاسم (عربي) - Name (Ar) *</label>
                  <input type="text" class="form-control form-control-lg text-dark" 
                         [(ngModel)]="formData.nameAr" required placeholder="مثلاً: مستشفى السلام">
                </div>
                <div class="row align-items-center g-3">
                  <div class="col-8">
                    <label class="form-label fw-bold small text-uppercase text-secondary">{{ '::Code' | abpLocalization }}</label>
                    <input type="text" class="form-control text-dark" 
                           [(ngModel)]="formData.code" 
                           [placeholder]="editingItem ? '' : ('::AutoGeneratedCode' | abpLocalization)" 
                           [disabled]="!editingItem">
                  </div>
                  <div class="col-4 d-flex justify-content-end align-items-end mb-1">
                    <div class="form-check form-switch p-0">
                      <label class="form-check-label fw-bold small text-secondary me-3" for="isRefActive">نشط</label>
                      <input type="checkbox" class="form-check-input ms-0" 
                             [(ngModel)]="formData.isActive" id="isRefActive">
                    </div>
                  </div>
                </div>
              </div>
              <div class="modal-footer border-0 p-4 shadow">
                <button type="button" class="btn btn-outline-secondary px-4 me-auto border-2 rounded-pill fw-bold" (click)="showForm = false" [disabled]="loading">إلغاء - Cancel</button>
                <button type="button" class="btn btn-danger px-5 rounded-pill fw-bold shadow" (click)="save()" [disabled]="loading || !formData.nameAr">
                  @if (loading) {
                    <span class="spinner-border spinner-border-sm me-2"></span>
                    Saving...
                  } @else {
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
    .btn-white { background: white; border: none; }
    .btn-white:hover { background: #fdf2f2; }
    .bg-danger-subtle { background-color: #fef2f2 !important; }
    .shadow-2xl { box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25); }
    .shadow-inner { box-shadow: inset 0 2px 4px 0 rgba(0, 0, 0, 0.05); }
  `]
})
export class ReferralSourcesComponent implements OnInit {
  private service = inject(ReferralSourceService);
  private confirmation = inject(ConfirmationService);

  items: ReferralSourceDto[] = [];
  searchText = '';
  showForm = false;
  editingItem: ReferralSourceDto | null = null;
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

  edit(item: ReferralSourceDto) {
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

  delete(item: ReferralSourceDto) {
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
