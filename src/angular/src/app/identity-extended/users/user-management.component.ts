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
          
          <ngx-datatable-column [name]="'AbpIdentity::UserName' | abpLocalization" prop="userName">
            <ng-template let-row="row" ngx-datatable-cell-template>
              {{ row.userName }}
            </ng-template>
          </ngx-datatable-column>
          <ngx-datatable-column name="الاسم بالعربي" prop="name">
            <ng-template let-row="row" ngx-datatable-cell-template>
              {{ row.name }}
            </ng-template>
          </ngx-datatable-column>
          <ngx-datatable-column [name]="'AbpIdentity::EmailAddress' | abpLocalization" prop="email" [width]="300">
            <ng-template let-row="row" ngx-datatable-cell-template>
              <div class="d-flex align-items-center justify-content-start" dir="ltr">
                <img [src]="row.extraProperties?.ProfilePictureUrl || ('https://ui-avatars.com/api/?name=' + (row.name || row.userName) + '&background=random&rounded=true&size=32')"
                     class="rounded-circle" style="margin-right: 10px;" width="32" height="32" alt="Avatar"
                     (error)="$event.target.src='assets/images/avatar/avatar-1.jpg'">
                <span class="text-truncate">{{ row.email }}</span>
              </div>
            </ng-template>
          </ngx-datatable-column>
          <ngx-datatable-column [name]="'AbpIdentity::PhoneNumber' | abpLocalization" prop="phoneNumber">
            <ng-template let-row="row" ngx-datatable-cell-template>
              <span dir="ltr">{{ row.phoneNumber || '-' }}</span>
            </ng-template>
          </ngx-datatable-column>

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
        <button type="button" class="btn-close" aria-label="Close" (click)="modal.dismiss()">
          <span aria-hidden="true" class="d-none d-sm-inline">&times;</span>
          <i class="fa fa-times d-inline d-sm-none"></i>
        </button>
      </div>
      <div class="modal-body">
        <form [formGroup]="form" (ngSubmit)="save()">
          <div class="mb-4 text-center">
            <label for="profilePicture" class="form-label d-block fw-bold mb-3">الصورة الشخصية</label>
            <div class="position-relative d-inline-block">
              <img [src]="profilePicturePreview || 'assets/images/avatar/avatar-1.jpg'"
                   class="rounded-circle border shadow-sm" width="100" height="100" alt="Profile Picture" style="object-fit: cover;"
                   (error)="profilePicturePreview = 'https://ui-avatars.com/api/?name=' + (form.get('userName')?.value || 'U') + '&background=random&rounded=true&size=100'">
              <input type="file" id="profilePicture" class="d-none" accept="image/png, image/jpeg, image/gif" (change)="onProfilePictureChange($event)">
              <button type="button" class="btn btn-sm btn-light position-absolute bottom-0 end-0 rounded-circle border shadow"
                      style="width: 32px; height: 32px; padding: 0; display: flex; align-items: center; justify-content: center; transform: translate(25%, 25%);"
                      onclick="document.getElementById('profilePicture').click()">
                 <i class="fas fa-camera text-primary"></i>
              </button>
            </div>
            <div *ngIf="profilePictureError" class="text-danger mt-2 small fw-bold">{{ profilePictureError }}</div>
            <div class="text-muted mt-2" style="font-size: 0.8rem;">الحجم الأقصى المسموح به هو 50 كيلوبايت (JPG/PNG).</div>
          </div>
          <div class="mb-3">
            <label for="userName" class="form-label">{{ 'AbpIdentity::UserName' | abpLocalization }}</label>
            <input type="text" id="userName" class="form-control" formControlName="userName">
          </div>
          <div class="mb-3">
            <label for="name" class="form-label">الاسم (بالعربي)</label>
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
          <div class="mb-3">
             <label for="password" class="form-label">
                 {{ 'AbpIdentity::Password' | abpLocalization }}
                 <span *ngIf="selectedUser?.id" class="text-muted small fw-normal ms-2">(اتركه فارغاً إذا لم ترغب بتغييره)</span>
             </label>
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
  selectedUserName: string = '';

  profilePicturePreview: string | null = null;
  profilePictureError: string | null = null;

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
      password: [''],
      profilePictureUrl: ['']
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
    this.profilePicturePreview = null;
    this.profilePictureError = null;
    this.form.reset();
    this.form.get('password')?.setValidators([Validators.required]); // Password required for new users
    this.form.get('password')?.updateValueAndValidity();
    this.modalService.open(this.userModal);
  }

  edit(id: string) {
    this.service.get(id).subscribe(user => {
      this.selectedUser = user;
      
      this.form.reset(); // Ensure previous values are cleared
      // Patch standard fields
      this.form.patchValue(user);
      
      this.profilePicturePreview = null;
      this.profilePictureError = null;

      // Patch extra properties
      if (user.extraProperties) {
        if (user.extraProperties['ProfilePictureUrl']) {
          const picUrl = user.extraProperties['ProfilePictureUrl'] as string;
          this.form.patchValue({ profilePictureUrl: picUrl });
          this.profilePicturePreview = picUrl;
        }
      }

      this.form.get('password')?.clearValidators(); // Password optional for edit
      this.form.get('password')?.updateValueAndValidity();
      this.modalService.open(this.userModal);
    });
  }

  save() {
    if (this.form.invalid) return;

    const formValue = this.form.value;
    
    // Construct the input matching IdentityUserCreateDto / IdentityUserUpdateDto
    const input: any = {
      ...formValue,
      extraProperties: {
        ProfilePictureUrl: formValue.profilePictureUrl
      }
    };

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

  onProfilePictureChange(event: any) {
    const file = event.target.files[0];
    this.profilePictureError = null;

    if (!file) return;

    // Check size (50KB max)
    if (file.size > 50 * 1024) {
      this.profilePictureError = 'عذراً، حجم الصورة يتجاوز 50 كيلوبايت. يرجى اختيار صورة أصغر.';
      event.target.value = ''; // clear input
      return;
    }

    const reader = new FileReader();
    reader.onload = (e: any) => {
      const base64String = e.target.result;
      this.profilePicturePreview = base64String;
      this.form.patchValue({ profilePictureUrl: base64String });
    };
    reader.readAsDataURL(file);
  }
}
