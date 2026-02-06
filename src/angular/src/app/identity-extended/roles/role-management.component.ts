import { Component, OnInit, inject, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { IdentityRoleService, GetIdentityRolesInput, IdentityRoleDto } from '@abp/ng.identity/proxy';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgbDropdownModule, NgbModalModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PageModule } from '@abp/ng.components/page';
import { PermissionManagementModule, PermissionManagementComponent } from '@abp/ng.permission-management';

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
        ReactiveFormsModule,
        FormsModule, // Keep FormsModule for ngModel
        PageModule // Keep PageModule if it's used elsewhere or for consistency
    ],
    providers: [ListService], // Provide ListService specifically for this component
    template: `
    <div class="card">
      <div class="card-header">
        <div class="row align-items-center">
          <div class="col col-md-6">
            <h5 class="card-title mb-0">
              {{ 'AbpIdentity::Roles' | abpLocalization }}
            </h5>
          </div>
          <div class="text-end col col-md-6">
             <!-- Toolbar placeholder -->
             <button class="btn btn-primary" (click)="create()">
                 <i class="fa fa-plus me-1"></i> {{ 'AbpIdentity::NewRole' | abpLocalization }}
             </button>
          </div>
        </div>
      </div>
      <div class="card-body">
        <!-- Search -->
        <div class="row mb-3">
          <div class="col-md-6">
            <div class="input-group">
              <span class="input-group-text"><i class="fas fa-search"></i></span>
              <input type="text" class="form-control" [placeholder]="'AbpIdentity::Search' | abpLocalization"
                     [(ngModel)]="searchText" (keyup.enter)="list.get()" (ngModelChange)="list.get()">
            </div>
          </div>
        </div>

        <ngx-datatable [rows]="data.items" [count]="data.totalCount" [list]="list" default class="material" [footerHeight]="50" [limit]="10" [externalPaging]="true">
          
          <ngx-datatable-column [name]="'AbpIdentity::RoleName' | abpLocalization" prop="name">
             <ng-template let-row="row" ngx-datatable-cell-template>
                <span class="fw-bold">{{ row.name }}</span>
                <span *ngIf="row.isDefault" class="badge bg-success ms-2">{{ 'AbpIdentity::DisplayName:IsDefault' | abpLocalization }}</span>
                <span *ngIf="row.isPublic" class="badge bg-info ms-2">{{ 'AbpIdentity::DisplayName:IsPublic' | abpLocalization }}</span>
             </ng-template>
          </ngx-datatable-column>

          <ngx-datatable-column [name]="'AbpIdentity::Actions' | abpLocalization" sortable="false" [maxWidth]="260">
            <ng-template let-row="row" ngx-datatable-cell-template>
              <div class="btn-group">
                <button *abpPermission="'AbpIdentity.Roles.Update'" type="button" class="btn btn-primary btn-sm me-1" (click)="edit(row.id)">
                  {{ 'AbpIdentity::Edit' | abpLocalization }}
                </button>
                <button *abpPermission="'AbpIdentity.Roles.ManagePermissions'" type="button" class="btn btn-warning btn-sm me-1" (click)="permissions(row)">
                  {{ 'AbpIdentity::Permissions' | abpLocalization }}
                </button>
                <button *abpPermission="'AbpIdentity.Roles.Delete'" type="button" class="btn btn-danger btn-sm" (click)="delete(row.id)">
                  {{ 'AbpIdentity::Delete' | abpLocalization }}
                </button>
              </div>
            </ng-template>
          </ngx-datatable-column>
        </ngx-datatable>
        

      </div>
    </div>

    <!-- Permission Management Modal: visible at the end -->
    <abp-permission-management 
        [providerName]="'R'" 
        [providerKey]="selectedRoleId" 
        [entityDisplayName]="selectedRoleName"
        [hideBadges]="false" 
        [(visible)]="isPermissionModalVisible">
    </abp-permission-management>

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
