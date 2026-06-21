import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule, ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgbDateStruct, NgbDateAdapter, NgbDateNativeAdapter, NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';

import { PurchaseOrderService } from '../../../proxy/inventory/purchase-order.service';
import { SupplierService } from '../../../proxy/inventory/supplier.service';
import { ServiceItemService } from '../../../proxy/services/service-item.service';
import { InventoryService } from '../../../proxy/inventory/inventory.service';
import { PurchaseRequisitionService, PurchaseRequisitionStatus } from '../../../proxy/inventory';
import { PurchaseOrderDto, PurchaseOrderLineDto, SupplierDto } from '../../../proxy/inventory/dtos/models';
import { PurchaseOrderStatus } from '../../../proxy/inventory/purchase-order-status.enum';
import { DrugService } from '../../../proxy/pharmacy/drug.service';
import { DrugDto } from '../../../proxy/pharmacy/dtos/models';

@Component({
    selector: 'app-purchase-order-detail',
    standalone: true,
    imports: [CoreModule, CommonModule, ReactiveFormsModule, RouterModule, ThemeSharedModule, NgbDatepickerModule],
    providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
    templateUrl: './purchase-order-detail.component.html'
})
export class PurchaseOrderDetailComponent implements OnInit {
    private fb = inject(FormBuilder);
    private service = inject(PurchaseOrderService);
    private supplierService = inject(SupplierService);
    private productService = inject(DrugService);
    private inventoryService = inject(InventoryService);
    private requisitionService = inject(PurchaseRequisitionService);
    private route = inject(ActivatedRoute);
    private router = inject(Router);
    private toaster = inject(ToasterService);
    private confirmation = inject(ConfirmationService);

    form: FormGroup;
    id: string | null = null;
    requisitionId: string | null = null;
    order: PurchaseOrderDto | null = null;

    suppliers: SupplierDto[] = [];
    products: DrugDto[] = [];

    isSaving = false;
    PurchaseOrderStatus = PurchaseOrderStatus;

    get lines(): FormArray {
        return this.form.get('purchaseOrderLines') as FormArray;
    }

    constructor() {
        this.buildForm();
    }

    ngOnInit() {
        this.loadSuppliers();
        this.loadProducts();

        this.route.paramMap.subscribe(params => {
            this.id = params.get('id');
            if (this.id) {
                this.loadOrder(this.id);
            } else {
                this.route.queryParamMap.subscribe(queryParams => {
                    this.requisitionId = queryParams.get('requisitionId');
                    if (this.requisitionId) {
                        this.loadFromRequisition(this.requisitionId);
                    } else {
                        // Default values for new order
                        this.addLine();
                    }
                });
            }
        });
    }

    buildForm() {
        this.form = this.fb.group({
            supplierId: [null, Validators.required],
            orderDate: [new Date(), Validators.required],
            expectedDeliveryDate: [null],
            referenceNumber: [''],
            notes: [''],
            purchaseOrderLines: this.fb.array([])
        });
    }

    loadSuppliers() {
        this.supplierService.getList({ maxResultCount: 100 }).subscribe(res => {
            this.suppliers = res.items;
        });
    }

    loadProducts() {
        this.productService.getList({ maxResultCount: 100 }).subscribe(res => {
            this.products = res.items;
        });
    }

    loadOrder(id: string) {
        this.service.get(id).subscribe(res => {
            this.order = res;
            this.form.patchValue({
                supplierId: res.supplierId,
                orderDate: new Date(res.orderDate),
                expectedDeliveryDate: res.expectedDeliveryDate ? new Date(res.expectedDeliveryDate) : null,
                referenceNumber: res.referenceNumber,
                notes: res.notes
            });

            this.lines.clear();
            if (res.purchaseOrderLines) {
                res.purchaseOrderLines.forEach(line => {
                    this.lines.push(this.createLineGroup(line));
                });
            }

            if (res.status !== PurchaseOrderStatus.Draft) {
                this.form.disable();
            }
        });
    }

    loadFromRequisition(reqId: string) {
        this.requisitionService.get(reqId).subscribe(res => {
            this.lines.clear();
            if (res.lines && res.lines.length > 0) {
                res.lines.forEach(l => {
                    this.lines.push(this.createLineGroup({ productId: l.productId, quantity: l.quantity }));
                });
            } else {
                this.addLine();
            }
            this.form.patchValue({
                notes: 'Generated from Requisition: ' + res.requisitionNumber
            });
            this.toaster.info('Items loaded from Purchase Requisition', 'Info');
        });
    }

    createLineGroup(line: any = null) {
        return this.fb.group({
            productId: [line?.productId || null, Validators.required],
            quantity: [line?.quantity ?? 1, [Validators.required, Validators.min(0.0001)]],
            unitPrice: [line?.unitPrice ?? 0, [Validators.required, Validators.min(0)]],
            discount: [line?.discount ?? 0, [Validators.min(0)]],
            description: [line?.description || '']
        });
    }

    addLine() {
        this.lines.push(this.createLineGroup());
    }

    removeLine(index: number) {
        this.lines.removeAt(index);
    }

    calculateTotal() {
        let total = 0;
        this.lines.controls.forEach(control => {
            const qty = control.get('quantity')?.value || 0;
            const price = control.get('unitPrice')?.value || 0;
            const discount = control.get('discount')?.value || 0;
            total += (qty * price) - discount;
        });
        return total;
    }

    getLineTotal(index: number) {
        const control = this.lines.at(index);
        const qty = control.get('quantity')?.value || 0;
        const price = control.get('unitPrice')?.value || 0;
        const discount = control.get('discount')?.value || 0;
        return (qty * price) - discount;
    }

    save() {
        if (this.form.invalid) return;

        this.isSaving = true;
        const model = {
            ...this.form.value,
        };
        
        if (model.purchaseOrderLines) {
            model.purchaseOrderLines.forEach((l: any) => {
                l.discount = l.discount || 0;
                l.unitPrice = l.unitPrice || 0;
            });
        }

        const req = this.id
            ? this.service.update(this.id, model)
            : this.service.create(model);

        req.subscribe({
            next: (res) => {
                if (this.requisitionId) {
                    this.requisitionService.updateStatus(this.requisitionId, PurchaseRequisitionStatus.ConvertedToPO).subscribe();
                }
                this.toaster.success('::SuccessfullySaved');
                this.isSaving = false;
                if (!this.id) {
                    this.router.navigate(['/inventory/purchase-orders/edit', res.id]);
                }
            },
            error: (err) => {
                console.error(err);
                this.isSaving = false;
            }
        });
    }

    confirmOrder() {
        if (!this.id) return;
        this.confirmation.warn('::AreYouSureToConfirmOrder', '::AreYouSure').subscribe(status => {
            if (status === Confirmation.Status.confirm) {
                this.service.confirmOrder(this.id!).subscribe(() => {
                    this.toaster.success('::SuccessfullyConfirmed');
                    this.loadOrder(this.id!);
                });
            }
        });
    }

    cancelOrder() {
        if (!this.id) return;
        this.confirmation.warn('::AreYouSureToCancelOrder', '::AreYouSure').subscribe(status => {
            if (status === Confirmation.Status.confirm) {
                this.service.cancelOrder(this.id!).subscribe(() => {
                    this.toaster.success('::SuccessfullyCancelled');
                    this.loadOrder(this.id!);
                });
            }
        });
    }

    showPriceComparison(index: number) {
        const productId = this.lines.at(index).get('productId')?.value;
        if (!productId) return;

        // Simplified: Alerting the feature for now as modal needs more boilerplate
        this.service.getPriceComparison(productId).subscribe(res => {
            if (res.length === 0) {
                this.toaster.info('::NoPriceHistoryFound', '::PriceAnalysis');
            } else {
                const history = res.map(x => `${x.supplierName}: ${x.unitPrice} (${new Date(x.orderDate).toLocaleDateString()})`).join('\n');
                this.toaster.info(history, 'Last 5 Purchase Prices');
            }
        });
    }

    receiveOrder() {
        if (!this.id) return;
        this.confirmation.warn('::ReceiveOrderConfirm', '::ReceiveOrder').subscribe(status => {
            if (status === Confirmation.Status.confirm) {
                this.inventoryService.getWarehouseList({ maxResultCount: 1 }).subscribe(whs => {
                    const whId = whs.items[0]?.id;
                    if (whId) {
                        this.service.receiveOrder(this.id!, whId).subscribe(() => {
                            this.toaster.success('::OrderReceivedSuccess', '::Success');
                            this.loadOrder(this.id!);
                        });
                    } else {
                        this.toaster.error('No warehouse found to receive stock into.', 'Error');
                    }
                });
            }
        });
    }
}
