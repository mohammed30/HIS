import { Component, OnInit, inject, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { IdentityRoleService, GetIdentityRolesInput, IdentityRoleDto } from '@abp/ng.identity/proxy';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgbDropdownModule, NgbModalModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PageModule } from '@abp/ng.components/page';
import { PermissionManagementModule } from '@abp/ng.permission-management';
import { CustomPermissionManagement } from '../../shared/components/custom-permission-management/custom-permission-management';

@Component({
    selector: 'app-role-management',
    standalone: true,
    imports: [
        CommonModule,
        CoreModule,
        ThemeSharedModule,
        NgxDatatableModule,
        NgbDropdownModule,
        NgbModalModule,
        PermissionManagementModule,
        CustomPermissionManagement,
        ReactiveFormsModule,
        FormsModule, // Keep FormsModule for ngModel
        PageModule // Keep PageModule if it's used elsewhere or for consistency
    ],
    providers: [ListService], // Provide ListService specifically for this component
    template: `
    <div class="card shadow-sm rounded-4 overflow-hidden border-0">
      <div class="card-header bg-transparent border-bottom-0 pt-4 pb-2 px-4">
        <div class="d-flex justify-content-between align-items-center">
          <h4 class="card-title mb-0 fw-bold text-primary">
             <i class="fas fa-users-cog me-2"></i> {{ 'AbpIdentity::Roles' | abpLocalization }}
          </h4>
          <button class="btn btn-primary rounded-pill px-4 shadow-sm fw-bold" (click)="create()">
             <i class="fas fa-plus me-1"></i> {{ 'AbpIdentity::NewRole' | abpLocalization }}
          </button>
        </div>
      </div>
      <div class="card-body px-4 pb-4">
        <!-- Search -->
        <div class="row mb-4">
          <div class="col-md-5">
            <div class="input-group input-group-lg shadow-sm rounded-pill overflow-hidden border">
              <span class="input-group-text bg-transparent border-0 text-muted ps-4"><i class="fas fa-search"></i></span>
              <input type="text" class="form-control border-0 bg-transparent shadow-none ps-2" [placeholder]="'AbpIdentity::Search' | abpLocalization"
                     [(ngModel)]="searchText" (keyup.enter)="list.get()" (ngModelChange)="list.get()">
            </div>
          </div>
        </div>

        <div class="table-responsive">
          <ngx-datatable [rows]="data.items" [count]="data.totalCount" [list]="list" default class="material rounded-3 shadow-sm border" [rowHeight]="60" [headerHeight]="50" [footerHeight]="50" [limit]="10" [externalPaging]="true">
            
            <ngx-datatable-column [name]="'AbpIdentity::RoleName' | abpLocalization" prop="name">
               <ng-template let-row="row" ngx-datatable-cell-template>
                  <div class="d-flex align-items-center h-100">
                    <div class="avatar-sm text-primary rounded-circle d-flex justify-content-center align-items-center me-3 shadow-sm border border-primary" style="width: 35px; height: 35px; background-color: rgba(var(--bs-primary-rgb), 0.1);">
                      <i class="fas fa-user-shield"></i>
                    </div>
                    <span class="fw-bold fs-6">{{ row.name }} <span class="text-muted ms-1 fw-normal" style="font-size: 0.9em;" *ngIf="('Role:' + row.name | abpLocalization) !== 'Role:' + row.name">({{ 'Role:' + row.name | abpLocalization }})</span></span>
                    <span *ngIf="row.isDefault" class="badge rounded-pill ms-3 px-3 py-2" style="background-color: rgba(var(--bs-success-rgb), 0.1); color: var(--bs-success); border: 1px solid rgba(var(--bs-success-rgb), 0.2);"><i class="fas fa-check-circle me-1"></i> {{ 'AbpIdentity::DisplayName:IsDefault' | abpLocalization }}</span>
                    <span *ngIf="row.isPublic" class="badge rounded-pill ms-2 px-3 py-2" style="background-color: rgba(var(--bs-info-rgb), 0.1); color: var(--bs-info); border: 1px solid rgba(var(--bs-info-rgb), 0.2);"><i class="fas fa-globe me-1"></i> {{ 'AbpIdentity::DisplayName:IsPublic' | abpLocalization }}</span>
                  </div>
               </ng-template>
            </ngx-datatable-column>

            <ngx-datatable-column [name]="'AbpIdentity::Actions' | abpLocalization" sortable="false" [maxWidth]="200" cellClass="text-end" headerClass="text-end">
              <ng-template let-row="row" ngx-datatable-cell-template>
                <div class="d-flex justify-content-end align-items-center h-100 gap-2">
                  <button *abpPermission="'AbpIdentity.Roles.Update'" type="button" class="btn btn-icon rounded-circle shadow-sm border" style="background-color: rgba(var(--bs-primary-rgb), 0.1); color: var(--bs-primary); border-color: rgba(var(--bs-primary-rgb), 0.2) !important;" (click)="edit(row.id)" [title]="'AbpIdentity::Edit' | abpLocalization">
                    <i class="fas fa-edit text-primary"></i>
                  </button>
                  <button *abpPermission="'AbpIdentity.Roles.ManagePermissions'" type="button" class="btn btn-icon rounded-circle shadow-sm border" style="background-color: rgba(var(--bs-warning-rgb), 0.1); color: var(--bs-warning); border-color: rgba(var(--bs-warning-rgb), 0.2) !important;" (click)="permissions(row)" [title]="'AbpIdentity::Permissions' | abpLocalization">
                    <i class="fas fa-key text-warning"></i>
                  </button>
                  <button *abpPermission="'AbpIdentity.Roles.Delete'" type="button" class="btn btn-icon rounded-circle shadow-sm border" style="background-color: rgba(var(--bs-danger-rgb), 0.1); color: var(--bs-danger); border-color: rgba(var(--bs-danger-rgb), 0.2) !important;" (click)="delete(row.id)" [title]="'AbpIdentity::Delete' | abpLocalization">
                    <i class="fas fa-trash text-danger"></i>
                  </button>
                </div>
              </ng-template>
            </ngx-datatable-column>
          </ngx-datatable>
        </div>

      </div>
    </div>

    <!-- Permission Management Modal: visible at the end -->
    <app-custom-permission-management 
        [providerName]="'R'" 
        [providerKey]="selectedRoleName" 
        [(visible)]="isPermissionModalVisible">
    </app-custom-permission-management>

    <!-- Edit/Create Role Modal -->
    <ng-template #roleModal let-modal>
      <div class="modal-header">
        <h5 class="modal-title">{{ (selectedRole?.id ? 'AbpIdentity::Edit' : 'AbpIdentity::NewRole') | abpLocalization }}</h5>
        <button type="button" class="btn-close" aria-label="Close" (click)="modal.dismiss()"></button>
      </div>
      <div class="modal-body">
        <form [formGroup]="form" (ngSubmit)="save()">
          <div class="mb-3">
            <label for="roleName" class="form-label">{{ 'AbpIdentity::RoleName' | abpLocalization }}</label>
            <input type="text" id="roleName" class="form-control" formControlName="name">
          </div>
          <div class="form-check mb-3">
            <input type="checkbox" id="isDefault" class="form-check-input" formControlName="isDefault">
            <label for="isDefault" class="form-check-label">{{ 'AbpIdentity::DisplayName:IsDefault' | abpLocalization }}</label>
          </div>
           <div class="form-check mb-3">
            <input type="checkbox" id="isPublic" class="form-check-input" formControlName="isPublic">
            <label for="isPublic" class="form-check-label">{{ 'AbpIdentity::DisplayName:IsPublic' | abpLocalization }}</label>
          </div>
        </form>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" (click)="modal.dismiss()">{{ 'AbpIdentity::Cancel' | abpLocalization }}</button>
        <button type="button" class="btn btn-primary" (click)="save()" [disabled]="form.invalid">{{ 'AbpIdentity::Save' | abpLocalization }}</button>
      </div>
    </ng-template>
  `
})
export class RoleManagementComponent implements OnInit {
    protected service = inject(IdentityRoleService);
    protected confirmation = inject(ConfirmationService);
    protected fb = inject(FormBuilder);
    protected modalService = inject(NgbModal);
    protected cd = inject(ChangeDetectorRef);
    list = inject(ListService);

    @ViewChild('roleModal') roleModal: any;

    data: PagedResultDto<IdentityRoleDto> = { items: [], totalCount: 0 };
    searchText = '';

    // Permission State
    isPermissionModalVisible = false;
    selectedRoleId = '';
    selectedRoleName = '';

    // Form State
    form: FormGroup;
    selectedRole: IdentityRoleDto | null = null;

    constructor() {
        this.form = this.fb.group({
            name: ['', Validators.required],
            isDefault: [false],
            isPublic: [true]
        });
    }

    ngOnInit() {
        this.list.maxResultCount = 10; // Explicitly set page size

        this.list.hookToQuery((query) => {
            query.maxResultCount = 10; // FORCE 10 items per page
            return this.service.getList({ ...query, filter: this.searchText } as GetIdentityRolesInput);
        }).subscribe(res => {
            this.data = res;
        });
    }

    create() {
        this.selectedRole = null;
        this.form.reset({ isPublic: true, isDefault: false });
        this.modalService.open(this.roleModal);
    }

    edit(id: string) {
        this.service.get(id).subscribe(role => {
            this.selectedRole = role;
            this.form.patchValue(role);
            this.modalService.open(this.roleModal);
        });
    }

    save() {
        if (this.form.invalid) return;

        const input = this.form.value;
        const request = this.selectedRole?.id
            ? this.service.update(this.selectedRole.id, input)
            : this.service.create(input);

        request.subscribe(() => {
            this.list.get();
            this.modalService.dismissAll();
        });
    }

    permissions(row: IdentityRoleDto) {
        this.selectedRoleId = row.id;
        this.selectedRoleName = row.name;
        this.cd.detectChanges();
        this.isPermissionModalVisible = true;
    }

    delete(id: string) {
        this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.service.delete(id).subscribe(() => this.list.get());
            }
        });
    }
}
