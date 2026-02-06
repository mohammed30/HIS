import { Component, OnInit, inject, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { IdentityUserService, GetIdentityUsersInput, IdentityUserDto } from '@abp/ng.identity/proxy';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgbDropdownModule, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { eIdentityComponents } from '@abp/ng.identity';
import { PageModule } from '@abp/ng.components/page';
import { PermissionManagementModule, PermissionManagementComponent } from '@abp/ng.permission-management';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, ThemeSharedModule, NgxDatatableModule, NgbDropdownModule, FormsModule, ReactiveFormsModule, CoreModule, PageModule, PermissionManagementModule],
  providers: [ListService],
  template: `
    <div class="card">
      <div class="card-header">
        <div class="row align-items-center">
          <div class="col col-md-6">
            <h5 class="card-title mb-0">
              {{ 'AbpIdentity::Users' | abpLocalization }}
            </h5>
          </div>
          <div class="text-end col col-md-6">
             <!-- Toolbar placeholder -->
             <button class="btn btn-primary" (click)="create()">
                 <i class="fa fa-plus me-1"></i> {{ 'AbpIdentity::NewUser' | abpLocalization }}
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
          
          <ngx-datatable-column [name]="'AbpIdentity::UserName' | abpLocalization" prop="userName"></ngx-datatable-column>
          <ngx-datatable-column [name]="'AbpIdentity::EmailAddress' | abpLocalization" prop="email"></ngx-datatable-column>
          <ngx-datatable-column [name]="'AbpIdentity::PhoneNumber' | abpLocalization" prop="phoneNumber"></ngx-datatable-column>

          <ngx-datatable-column [name]="'AbpIdentity::Actions' | abpLocalization" sortable="false" [maxWidth]="260">
            <ng-template let-row="row" ngx-datatable-cell-template>
              <div class="btn-group">
                <button *abpPermission="'AbpIdentity.Users.Update'" type="button" class="btn btn-primary btn-sm me-1" (click)="edit(row.id)">
                  {{ 'AbpIdentity::Edit' | abpLocalization }}
                </button>
                <button *abpPermission="'AbpIdentity.Users.ManagePermissions'" type="button" class="btn btn-warning btn-sm me-1" (click)="permissions(row)">
                  {{ 'AbpIdentity::Permissions' | abpLocalization }}
                </button>
                <button *abpPermission="'AbpIdentity.Users.Delete'" type="button" class="btn btn-danger btn-sm" (click)="delete(row.id)">
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
        [providerName]="'U'" 
        [providerKey]="selectedUserId" 
        [entityDisplayName]="selectedUserName"
        [hideBadges]="false" 
        [(visible)]="isPermissionModalVisible">
    </abp-permission-management>

    <!-- Edit/Create User Modal -->
    <ng-template #userModal let-modal>
      <div class="modal-header">
        <h5 class="modal-title">{{ (selectedUser?.id ? 'AbpIdentity::Edit' : 'AbpIdentity::NewUser') | abpLocalization }}</h5>
        <button type="button" class="btn-close" aria-label="Close" (click)="modal.dismiss()"></button>
      </div>
      <div class="modal-body">
        <form [formGroup]="form" (ngSubmit)="save()">
          <div class="mb-3">
            <label for="userName" class="form-label">{{ 'AbpIdentity::UserName' | abpLocalization }}</label>
            <input type="text" id="userName" class="form-control" formControlName="userName">
          </div>
          <div class="mb-3">
            <label for="name" class="form-label">{{ 'AbpIdentity::Name' | abpLocalization }}</label>
            <input type="text" id="name" class="form-control" formControlName="name">
          </div>
          <div class="mb-3">
            <label for="surname" class="form-label">{{ 'AbpIdentity::Surname' | abpLocalization }}</label>
            <input type="text" id="surname" class="form-control" formControlName="surname">
          </div>
          <div class="mb-3">
            <label for="email" class="form-label">{{ 'AbpIdentity::EmailAddress' | abpLocalization }}</label>
            <input type="email" id="email" class="form-control" formControlName="email">
          </div>
           <div class="mb-3">
            <label for="phoneNumber" class="form-label">{{ 'AbpIdentity::PhoneNumber' | abpLocalization }}</label>
            <input type="text" id="phoneNumber" class="form-control" formControlName="phoneNumber">
          </div>
          <!-- Password field logic is complex (required for create, optional for edit), simplified here -->
          <div class="mb-3" *ngIf="!selectedUser?.id">
             <label for="password" class="form-label">{{ 'AbpIdentity::Password' | abpLocalization }}</label>
             <input type="password" id="password" class="form-control" formControlName="password">
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
export class UserManagementComponent implements OnInit {
  protected service = inject(IdentityUserService);
  protected confirmation = inject(ConfirmationService);
  protected fb = inject(FormBuilder);
  protected modalService = inject(NgbModal);
  protected cd = inject(ChangeDetectorRef);
  list = inject(ListService);

  @ViewChild('userModal') userModal: any;

  data: PagedResultDto<IdentityUserDto> = { items: [], totalCount: 0 };
  searchText = '';

  // Permission State
  isPermissionModalVisible = false;
  selectedUserId = '';
  selectedUserName = '';

  // Form State
  form: FormGroup;
  selectedUser: IdentityUserDto | null = null;

  constructor() {
    this.form = this.fb.group({
      userName: ['', Validators.required],
      name: [''],
      surname: [''],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      password: ['']
    });
  }

  ngOnInit() {
    this.list.maxResultCount = 10;
    this.list.hookToQuery((query) => {
      query.maxResultCount = 10; // FORCE 10 items per page
      return this.service.getList({ ...query, filter: this.searchText } as GetIdentityUsersInput);
    }).subscribe(res => {
      this.data = res;
    });
  }

  create() {
    this.selectedUser = null;
    this.form.reset();
    this.form.get('password')?.setValidators([Validators.required]); // Password required for new users
    this.form.get('password')?.updateValueAndValidity();
    this.modalService.open(this.userModal);
  }

  edit(id: string) {
    this.service.get(id).subscribe(user => {
      this.selectedUser = user;
      this.form.patchValue(user);
      this.form.get('password')?.clearValidators(); // Password optional for edit
      this.form.get('password')?.updateValueAndValidity();
      this.modalService.open(this.userModal);
    });
  }

  save() {
    if (this.form.invalid) return;

    const input = this.form.value;
    const request = this.selectedUser?.id
      ? this.service.update(this.selectedUser.id, input)
      : this.service.create(input);

    request.subscribe(() => {
      this.list.get();
      this.modalService.dismissAll();
    });
  }

  permissions(row: IdentityUserDto) {
    this.selectedUserId = row.id;
    this.selectedUserName = row.userName;
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
