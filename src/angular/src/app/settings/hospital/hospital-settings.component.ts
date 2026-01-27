import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HospitalSettingsService } from '@proxy/settings';
import { HospitalSettingsDto } from '@proxy/settings/models';
import { ToasterService } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { LocalizationModule } from '@abp/ng.core';

@Component({
    selector: 'app-hospital-settings',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, LocalizationModule],
    templateUrl: './hospital-settings.component.html',
    styleUrls: ['./hospital-settings.component.scss']
})
export class HospitalSettingsComponent implements OnInit {
    form: FormGroup;
    loading = false;

    constructor(
        private fb: FormBuilder,
        private hospitalSettingsService: HospitalSettingsService,
        private toaster: ToasterService
    ) { }

    ngOnInit(): void {
        this.buildForm();
        this.loadSettings();
    }

    buildForm() {
        this.form = this.fb.group({
            hospitalName: ['', [Validators.required]],
            hospitalAddress: [''],
            hospitalPhone: [''],
            hospitalEmail: ['', [Validators.email]],
            hospitalLogo: [''],
            hospitalTaxNumber: ['']
        });
    }

    loadSettings() {
        this.loading = true;
        this.hospitalSettingsService.get().subscribe({
            next: (res: HospitalSettingsDto) => {
                this.form.patchValue(res);
                this.loading = false;
            },
            error: () => {
                this.loading = false;
                this.toaster.error('::ErrorLoadingSettings');
            }
        });
    }

    save() {
        if (this.form.invalid) {
            return;
        }

        this.loading = true;
        const input = this.form.value as HospitalSettingsDto;

        this.hospitalSettingsService.update(input).subscribe({
            next: () => {
                this.loading = false;
                this.toaster.success('::SettingsSavedSuccessfully');
            },
            error: () => {
                this.loading = false;
                this.toaster.error('::ErrorSavingSettings');
            }
        });
    }
}
