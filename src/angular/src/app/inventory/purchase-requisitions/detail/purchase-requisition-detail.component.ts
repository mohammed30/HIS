import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import { PurchaseRequisitionService, PurchaseRequisitionStatus } from '../../../proxy/inventory';
import { DrugService } from '../../../proxy/pharmacy/drug.service';

@Component({
    selector: 'app-purchase-requisition-detail',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule, ThemeSharedModule, CoreModule],
    template: `
    <div class="card shadow-sm">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h5 class="m-0">
            <i class="fas fa-edit me-2"></i>
            {{ (id ? '::EditRequisition' : '::NewRequisition') | abpLocalization }}
        </h5>
        <button class="btn btn-outline-secondary" routerLink="/inventory/purchase-requisitions">
            <i class="fas fa-arrow-left me-1"></i> {{ '::Back' | abpLocalization }}
        </button>
      </div>
      <div class="card-body">
        <form [formGroup]="form">
            <div class="row">
                <div class="col-md-6 mb-3">
                    <label class="form-label">{{ '::RequiredDate' | abpLocalization }} *</label>
                    <input type="date" class="form-control" formControlName="requiredDate">
                </div>
                <div class="col-md-6 mb-3">
                    <label class="form-label">{{ '::Department' | abpLocalization }} *</label>
                    <input type="text" class="form-control" value="Main Hospital" readonly>
                </div>
            </div>
            <div class="mb-3">
                <label class="form-label">{{ '::Notes' | abpLocalization }}</label>
                <textarea class="form-control" formControlName="notes"></textarea>
            </div>

            <hr>
            <h6>{{ '::RequestedItems' | abpLocalization }}</h6>
            <div formArrayName="lines">
                <div *ngFor="let line of lines.controls; let i=index" [formGroupName]="i" class="row mb-2 align-items-center">
                    <div class="col-md-5">
                        <select class="form-select" formControlName="productId">
                            <option [ngValue]="null">{{ '::SelectProduct' | abpLocalization }}</option>
                            <option *ngFor="let p of products" [value]="p.serviceItemId">{{ p.scientificName }} ({{ p.brandName }})</option>
                        </select>
                    </div>
                    <div class="col-md-3">
                        <input type="number" class="form-control" formControlName="quantity" [placeholder]="'::Qty' | abpLocalization">
                    </div>
                    <div class="col-md-3">
                        <input type="text" class="form-control" formControlName="description" [placeholder]="'::Notes' | abpLocalization">
                    </div>
                    <div class="col-md-1">
                        <button class="btn btn-sm btn-outline-danger border-0" (click)="removeLine(i)">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                </div>
            </div>
            <button class="btn btn-sm btn-outline-primary mt-2" (click)="addLine()">
                <i class="fas fa-plus me-1"></i> {{ '::AddItem' | abpLocalization }}
            </button>
        </form>

        <div class="d-flex justify-content-end gap-2 border-top pt-3 mt-4">
            <button class="btn btn-warning" (click)="convertToPO()" *ngIf="id && currentStatus === 2" [disabled]="isSaving">
                <i class="fas fa-shopping-cart me-1"></i> {{ '::ConvertToPO' | abpLocalization }}
            </button>
            <button class="btn btn-primary" (click)="approve()" *ngIf="id && currentStatus === 0" [disabled]="isSaving">
                <i class="fas fa-check me-1"></i> {{ '::Approve' | abpLocalization }}
            </button>
            <button class="btn btn-success p-premium" (click)="save()" [disabled]="form.invalid || isSaving">
                <i class="fas fa-save me-1"></i> {{ isSaving ? ('::Saving' | abpLocalization) : ('::SaveRequisition' | abpLocalization) }}
            </button>
        </div>
      </div>
    </div>
  `
})
export class PurchaseRequisitionDetailComponent implements OnInit {
    private fb = inject(FormBuilder);
    private service = inject(PurchaseRequisitionService);
    private productService = inject(DrugService);
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private toaster = inject(ToasterService);

    form: FormGroup;
    id: string | null = null;
    products: any[] = [];
    isSaving = false;
    currentStatus: PurchaseRequisitionStatus = 0;

    get lines(): FormArray {
        return this.form.get('lines') as FormArray;
    }

    constructor() {
        this.form = this.fb.group({
            requiredDate: [new Date().toISOString().split('T')[0], Validators.required],
            notes: [''],
            lines: this.fb.array([])
        });
    }

    ngOnInit() {
        this.productService.getList({ maxResultCount: 100 }).subscribe(res => {
            this.products = res.items;
        });

        this.id = this.route.snapshot.params['id'];
        if (this.id) {
            this.service.get(this.id).subscribe(res => {
                this.currentStatus = res.status;
                if (res.requiredDate) {
                    res.requiredDate = res.requiredDate.split('T')[0];
                }
                this.form.patchValue(res);
                this.lines.clear();
                if (res.lines) {
                    res.lines.forEach(l => this.lines.push(this.createLineGroup(l)));
                }
                if (this.lines.length === 0) {
                    this.addLine();
                }
            });
        } else {
            this.addLine();
        }
    }

    createLineGroup(line: any = null) {
        return this.fb.group({
            productId: [line ? line.productId : null, Validators.required],
            quantity: [line ? line.quantity : 1, [Validators.required, Validators.min(1)]],
            description: [line ? line.description : '']
        });
    }

    addLine() {
        this.lines.push(this.createLineGroup());
    }

    removeLine(index: number) {
        this.lines.removeAt(index);
    }

    save() {
        if (this.form.invalid) return;
        this.isSaving = true;
        const req = this.id ? this.service.update(this.id, this.form.value) : this.service.create(this.form.value);
        req.subscribe({
            next: () => {
                this.toaster.success('Saved successfully');
                this.router.navigate(['/inventory/purchase-requisitions']);
            },
            error: () => this.isSaving = false
        });
    }

    approve() {
        if (!this.id) return;
        this.isSaving = true;
        this.service.updateStatus(this.id, PurchaseRequisitionStatus.Approved).subscribe({
            next: () => {
                this.toaster.success('Approved successfully');
                this.currentStatus = PurchaseRequisitionStatus.Approved;
                this.isSaving = false;
            },
            error: () => this.isSaving = false
        });
    }

    convertToPO() {
        if (!this.id) return;
        this.router.navigate(['/inventory/purchase-orders/create'], { queryParams: { requisitionId: this.id } });
    }
}
