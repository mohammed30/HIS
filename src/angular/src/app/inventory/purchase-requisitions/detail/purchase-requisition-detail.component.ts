import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
import { PurchaseRequisitionService } from '../../../proxy/inventory';
import { ServiceItemService } from '../../../proxy/services/service-item.service';

@Component({
    selector: 'app-purchase-requisition-detail',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, RouterModule, ThemeSharedModule],
    template: `
    <div class="card shadow-sm">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h5 class="m-0">
            <i class="fas fa-edit me-2"></i>
            {{ id ? 'Edit Requisition' : 'New Requisition' }}
        </h5>
        <button class="btn btn-outline-secondary" routerLink="/inventory/purchase-requisitions">
            <i class="fas fa-arrow-left me-1"></i> Back
        </button>
      </div>
      <div class="card-body">
        <form [formGroup]="form">
            <div class="row">
                <div class="col-md-6 mb-3">
                    <label class="form-label">Required Date *</label>
                    <input type="date" class="form-control" formControlName="requiredDate">
                </div>
                <div class="col-md-6 mb-3">
                    <label class="form-label">Department *</label>
                    <input type="text" class="form-control" value="Main Hospital" readonly>
                </div>
            </div>
            <div class="mb-3">
                <label class="form-label">Notes</label>
                <textarea class="form-control" formControlName="notes"></textarea>
            </div>

            <hr>
            <h6>Requested Items</h6>
            <div formArrayName="lines">
                <div *ngFor="let line of lines.controls; let i=index" [formGroupName]="i" class="row mb-2 align-items-center">
                    <div class="col-md-5">
                        <select class="form-select" formControlName="productId">
                            <option [ngValue]="null">Select Product</option>
                            <option *ngFor="let p of products" [value]="p.id">{{ p.nameAr }}</option>
                        </select>
                    </div>
                    <div class="col-md-3">
                        <input type="number" class="form-control" formControlName="quantity" placeholder="Qty">
                    </div>
                    <div class="col-md-3">
                        <input type="text" class="form-control" formControlName="description" placeholder="Notes">
                    </div>
                    <div class="col-md-1">
                        <button class="btn btn-sm btn-outline-danger border-0" (click)="removeLine(i)">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                </div>
            </div>
            <button class="btn btn-sm btn-outline-primary mt-2" (click)="addLine()">
                <i class="fas fa-plus me-1"></i> Add Item
            </button>
        </form>

        <div class="d-flex justify-content-end gap-2 border-top pt-3 mt-4">
            <button class="btn btn-success p-premium" (click)="save()" [disabled]="form.invalid || isSaving">
                <i class="fas fa-save me-1"></i> {{ isSaving ? 'Saving...' : 'Save Requisition' }}
            </button>
        </div>
      </div>
    </div>
  `
})
export class PurchaseRequisitionDetailComponent implements OnInit {
    private fb = inject(FormBuilder);
    private service = inject(PurchaseRequisitionService);
    private productService = inject(ServiceItemService);
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private toaster = inject(ToasterService);

    form: FormGroup;
    id: string | null = null;
    products: any[] = [];
    isSaving = false;

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
                this.form.patchValue(res);
                this.lines.clear();
                res.lines.forEach(l => this.lines.push(this.createLineGroup(l)));
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
}
