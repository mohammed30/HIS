import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProfileService, UpdateProfileDto } from '@abp/ng.account.core/proxy';
import { ToasterService } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';

import { UserProfileService } from '@volo/ngx-lepton-x.core';

@Component({
  selector: 'app-profile-picture',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CoreModule],
  templateUrl: './profile-picture.html',
  styleUrls: ['./profile-picture.scss'],
})
export class ProfilePictureComponent implements OnInit {
  form!: FormGroup;
  profilePicturePreview: string | null = null;
  profilePictureError: string | null = null;
  userName: string = '';
  isSaving = false;
  
  originalProfile: UpdateProfileDto | null = null;

  constructor(
    private fb: FormBuilder,
    private profileService: ProfileService,
    private toaster: ToasterService,
    private lpxUserProfileService: UserProfileService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      userName: [{value: '', disabled: true}, Validators.required],
      name: [''],
      nameAr: [''],
      surname: [''],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: [''],
      profilePictureUrl: ['']
    });

    this.loadProfile();
  }

  loadProfile() {
    this.profileService.get().subscribe((profile) => {
      this.userName = profile.userName || 'U';
      this.originalProfile = {
        userName: profile.userName,
        email: profile.email,
        name: profile.name,
        surname: profile.surname,
        phoneNumber: profile.phoneNumber,
        extraProperties: profile.extraProperties || {}
      } as UpdateProfileDto;

      this.form.patchValue({
        userName: profile.userName,
        name: profile.name,
        surname: profile.surname,
        email: profile.email,
        phoneNumber: profile.phoneNumber
      });

      if (profile.extraProperties) {
        if (profile.extraProperties['NameAr']) {
          this.form.patchValue({ nameAr: profile.extraProperties['NameAr'] });
        }
        if (profile.extraProperties['ProfilePictureUrl']) {
          const picUrl = profile.extraProperties['ProfilePictureUrl'] as string;
          this.form.patchValue({ profilePictureUrl: picUrl });
          this.profilePicturePreview = picUrl;
        }
      }
    });
  }

  onProfilePictureChange(event: any) {
    const file = event.target.files[0];
    this.profilePictureError = null;

    if (!file) return;

    if (file.size > 50 * 1024) {
      this.profilePictureError = 'عذراً، حجم الصورة يتجاوز 50 كيلوبايت. يرجى اختيار صورة أصغر.';
      event.target.value = '';
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

  save() {
    if (this.form.invalid || !this.originalProfile) return;

    this.isSaving = true;
    const formValue = this.form.getRawValue();
    
    const input: UpdateProfileDto = {
      ...this.originalProfile,
      name: formValue.name,
      surname: formValue.surname,
      email: formValue.email,
      phoneNumber: formValue.phoneNumber,
      extraProperties: {
        ...this.originalProfile.extraProperties,
        NameAr: formValue.nameAr,
        ProfilePictureUrl: formValue.profilePictureUrl
      }
    };

    this.profileService.update(input).subscribe({
      next: () => {
        this.toaster.success('::SavedSuccessfully');
        this.isSaving = false;
        this.originalProfile = input;
        
        // Update the top bar avatar immediately
        if (formValue.profilePictureUrl) {
          this.lpxUserProfileService.patchUser({
            avatar: { type: 'image', source: formValue.profilePictureUrl }
          });
        }
      },
      error: () => {
        this.isSaving = false;
      }
    });
  }
}
