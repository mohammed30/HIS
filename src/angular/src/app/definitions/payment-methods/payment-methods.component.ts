import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { PaymentMethodService } from '../../proxy/general/payment-method.service';
import { PaymentMethodDto } from '../../proxy/general/models';

@Component({
    selector: 'app-payment-methods',
    standalone: true,
    imports: [CommonModule, FormsModule, NgbPaginationModule, ThemeSharedModule, CoreModule],
    template: `
    <div class="container-fluid py-4">
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center bg-info text-white">
          <h5 class="mb-0">
            <i class="fas fa-credit-card me-2"></i>
            {{ 'Menu:PaymentMethods' | abpLocalization }}
          </h5>
          <button class="btn btn-light" (click)="showForm = true; editingItem = null; resetForm()" *abpPermission="'HIS.Definitions.PaymentMethods'">
            <i class="fas fa-plus me-1"></i> {{ 'New' | abpLocalization }}
          </button>
        </div>
        <div class="card-body">
          <!-- Search -->
          <div class="row mb-3">
            <div class="col-md-4">
              <div class="input-group text-dark">
                <span class="input-group-text"><i class="fas fa-search"></i></span>
                <input type="text" class="form-control" placeholder="{{ 'Search' | abpLocalization }}" 
                       [(ngModel)]="searchText" (input)="search()">
              </div>
            </div>
          </div>

          <!-- Table -->
          <div class="table-responsive">
            <table class="table table-striped table-hover border">
              <thead class="table-info text-white">
                <tr>
                  <th>{{ 'Code' | abpLocalization }}</th>
                  <th>{{ 'Name' | abpLocalization }} (Ar)</th>
                  <th>{{ 'Name' | abpLocalization }} (En)</th>
                  <th>{{ 'IsDefault' | abpLocalization }}</th>
                  <th>{{ 'Status' | abpLocalization }}</th>
                  <th>{{ 'Actions' | abpLocalization }}</th>
                </tr>
              </thead>
              <tbody>
                @for (item of items; track item.id) {
                  <tr>
                    <td>{{ item.code }}</td>
                    <td>{{ item.nameAr }}</td>
                    <td>{{ item.nameEn }}</td>
                    <td>
                      @if (item.isDefault) {
                        <span class="badge bg-primary">
                          <i class="fas fa-check"></i>
                        </span>
                      }
                    </td>
                    <td>
                      <span [class]="item.isActive ? 'badge bg-success' : 'badge bg-secondary'">
                        {{ item.isActive ? ('Active' | abpLocalization) : ('Inactive' | abpLocalization) }}
                      </span>
                    </td>
                    <td>
                      <button class="btn btn-sm btn-outline-primary me-2" (click)="edit(item)" title="{{ 'Edit' | abpLocalization }}" *abpPermission="'HIS.Definitions.PaymentMethods'">
                        <i class="fas fa-edit"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-danger" (click)="delete(item)" title="{{ 'Delete' | abpLocalization }}" *abpPermission="'HIS.Definitions.PaymentMethods'">
                        <i class="fas fa-trash"></i>
                      </button>
                    </td>
                  </tr>
                } @empty {
                  <tr>
                    <td colspan="6" class="text-center text-muted py-4">
                      <i class="fas fa-info-circle me-1"></i> {{ 'NoDataFound' | abpLocalization }}
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
        <div class="modal show d-block" style="background: rgba(0,0,0,0.5); z-index: 1050;">
          <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-lg border-0">
              <div class="modal-header">
                <h5 class="modal-title">
                  <i class="fas fa-credit-card me-2"></i>
                  {{ (editingItem ? 'EditPaymentMethod' : 'NewPaymentMethod') | abpLocalization }}
                </h5>
                <button type="button" class="btn-close" (click)="showForm = false"></button>
              </div>
              <div class="modal-body p-4">
                 @if (editingItem) {
                  <div class="mb-3">
                    <label class="form-label fw-bold">{{ 'Code' | abpLocalization }}</label>
                    <input type="text" class="form-control" [(ngModel)]="formData.code" 
                           [disabled]="true">
                  </div>
                }
                <div class="mb-3">
                  <label class="form-label fw-bold">{{ 'Name' | abpLocalization }} (Ar) *</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.nameAr" required>
                </div>
                <div class="mb-3">
                  <label class="form-label fw-bold">{{ 'Name' | abpLocalization }} (En) *</label>
                  <input type="text" class="form-control" [(ngModel)]="formData.nameEn" required>
                </div>
               
                <div class="form-check form-switch mb-3">
                  <input type="checkbox" class="form-check-input" [(ngModel)]="formData.isDefault" id="isDefault">
                  <label class="form-check-label fw-bold" for="isDefault">{{ 'IsDefault' | abpLocalization }}</label>
                </div>
                <div class="form-check form-switch mb-3">
                  <input type="checkbox" class="form-check-input" [(ngModel)]="formData.isActive" id="isActive">
                  <label class="form-check-label fw-bold" for="isActive">{{ 'Active' | abpLocalization }}</label>
                </div>
              </div>
              <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary px-4" (click)="showForm = false" [disabled]="loading">{{ 'Cancel' | abpLocalization }}</button>
                <button type="button" class="btn btn-info text-white px-4" (click)="save()" [disabled]="loading || !formData.nameAr || !formData.nameEn">
                  <i class="fas" [class]="loading ? 'fa-spinner fa-spin' : 'fa-save'"></i>
                  <span class="ms-1">{{ (loading ? 'Saving' : 'Save') | abpLocalization }}</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      }
    </div>
  `,
    styles: [`
    .table th { font-weight: 600; }
    .card { border: none; border-radius: 12px; overflow: hidden; }
    .card-header { border-bottom: none; }
    .btn-info { transition: all 0.3s ease; }
    .btn-info:hover { filter: brightness(0.9); transform: translateY(-1px); }
  `]
})
export class PaymentMethodsComponent implements OnInit {
    private service = inject(PaymentMethodService);
    private confirmation = inject(ConfirmationService);

    items: PaymentMethodDto[] = [];
    searchText = '';
    showForm = false;
    editingItem: PaymentMethodDto | null = null;
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
        return { nameAr: '', nameEn: '', code: '', isActive: true, isDefault: false };
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

    edit(item: PaymentMethodDto) {
        this.editingItem = item;
        // Create a copy of the item for the form
        this.formData = { ...item };
        this.showForm = true;
    }

    save() {
        this.loading = true;
        if (this.editingItem) {
            this.service.update(this.editingItem.id, this.formData).subscribe({
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
        } else {
            this.service.create(this.formData).subscribe({
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
    }

    delete(item: PaymentMethodDto) {
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
