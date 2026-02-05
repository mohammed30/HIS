import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CoreModule } from '@abp/ng.core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { PharmacyService } from '../../pharmacy.service';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-drug-dialog',
    standalone: true,
    imports: [CommonModule, ThemeSharedModule, ReactiveFormsModule, CoreModule],
    template: `
    <div class="modal-header">
      <h5 class="modal-title">{{ (id ? '::EditDrug' : '::NewDrug') | abpLocalization }}</h5>
      <button type="button" class="btn-close" (click)="activeModal.dismiss()"></button>
    </div>
    <div class="modal-body">
      <form [formGroup]="form">
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">{{ '::Barcode' | abpLocalization }} <span class="text-danger">*</span></label>
                <input type="text" class="form-control" formControlName="barcode" [placeholder]="'::ScanOrTypeBarcode' | abpLocalization">
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">{{ '::Price' | abpLocalization }} <span class="text-danger">*</span></label>
                <input type="number" class="form-control" formControlName="price">
            </div>
        </div>
 
        <div class="mb-3">
            <label class="form-label">{{ '::BrandName' | abpLocalization }} <span class="text-danger">*</span></label>
            <input type="text" class="form-control" formControlName="brandName">
        </div>
 
        <div class="mb-3">
            <label class="form-label">{{ '::ScientificName' | abpLocalization }} <span class="text-danger">*</span></label>
            <input type="text" class="form-control" formControlName="scientificName">
        </div>
 
        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">{{ '::Strength' | abpLocalization }}</label>
                <input type="text" class="form-control" formControlName="strength">
            </div>
            <div class="col-md-6 mb-3">
                <label class="form-label">{{ '::Form' | abpLocalization }}</label>
                <select class="form-select" formControlName="form">
                    <option value="">{{ '::SelectForm' | abpLocalization }}</option>
                    <option value="Tablet">{{ '::Tablet' | abpLocalization }}</option>
                    <option value="Capsule">{{ '::Capsule' | abpLocalization }}</option>
                    <option value="Syrup">{{ '::Syrup' | abpLocalization }}</option>
                    <option value="Injection">{{ '::Injection' | abpLocalization }}</option>
                    <option value="Cream">{{ '::Cream' | abpLocalization }}</option>
                </select>
            </div>
        </div>
 
        <div class="mb-3">
            <label class="form-label">{{ '::Manufacturer' | abpLocalization }}</label>
            <input type="text" class="form-control" formControlName="manufacturer">
        </div>
      </form>
    </div>
    <div class="modal-footer">
      <button type="button" class="btn btn-secondary" (click)="activeModal.dismiss()">{{ '::Cancel' | abpLocalization }}</button>
      <button type="button" class="btn btn-primary" (click)="save()" [disabled]="form.invalid || saving">
        <i class="fa fa-save me-1"></i> {{ (saving ? '::Saving' : '::Save') | abpLocalization }}
      </button>
    </div>
  `
})
export class DrugDialogComponent implements OnInit {
    @Input() id: string; // Add Input
    form: FormGroup;
    saving = false;

    constructor(
        public activeModal: NgbActiveModal,
        private fb: FormBuilder,
        private pharmacyService: PharmacyService
    ) { }

    ngOnInit() {
        this.buildForm();
        if (this.id) {
            this.pharmacyService.getDrug(this.id).subscribe(res => {
                this.form.patchValue(res);
                // Also set service price if available separately, but for now assuming flat structure or ignored
            });
        }
    }

    buildForm() {
        this.form = this.fb.group({
            barcode: ['', Validators.required],
            brandName: ['', Validators.required],
            scientificName: ['', Validators.required],
            strength: [''],
            form: [''],
            manufacturer: [''],
            price: [0, Validators.required]
        });
    }

    save() {
        if (this.form.invalid) return;

        this.saving = true;
        const request = this.id
            ? this.pharmacyService.updateDrug(this.id, this.form.value)
            : this.pharmacyService.createDrug(this.form.value);

        request.subscribe({
            next: () => {
                this.activeModal.close(true);
            },
            error: () => {
                this.saving = false;
            }
        });
    }
}
