import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { PharmacySettingsService } from '@proxy/settings/pharmacy-settings.service';
import { PharmacySettingsDto } from '@proxy/settings/models';
import { ToasterService } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { LocalizationModule, CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-pharmacy-settings',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, LocalizationModule, CoreModule, ThemeSharedModule],
    templateUrl: './pharmacy-settings.component.html'
})
export class PharmacySettingsComponent implements OnInit {
    form: FormGroup;
    loading = false;

    constructor(
        private fb: FormBuilder,
        private pharmacySettingsService: PharmacySettingsService,
        private toaster: ToasterService
    ) { }

    ngOnInit(): void {
        this.buildForm();
        this.loadSettings();
    }

    buildForm() {
        this.form = this.fb.group({
            allowNegativeStock: [false]
        });
    }

    loadSettings() {
        this.loading = true;
        this.pharmacySettingsService.get().subscribe({
            next: (res: PharmacySettingsDto) => {
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
        this.loading = true;
        const input = this.form.value as PharmacySettingsDto;

        this.pharmacySettingsService.update(input).subscribe({
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
