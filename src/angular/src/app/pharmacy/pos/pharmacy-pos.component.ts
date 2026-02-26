import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import { FormsModule } from '@angular/forms';
import { PosService } from '../../proxy/pharmacy/pos.service';
import { PosSaleDto, PosSaleItemDto, PosProductDto } from '../../proxy/pharmacy/dtos/models';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-pharmacy-pos',
    standalone: true,
    imports: [CommonModule, ThemeSharedModule, CoreModule, FormsModule],
    template: `
    <div class="row">
        <!-- Left: Product Scan & Cart -->
        <div class="col-md-8">
            <div class="card mb-3">
                <div class="card-body">
                    <div class="input-group mb-3">
                        <span class="input-group-text"><i class="fas fa-barcode"></i></span>
                        <input type="text" class="form-control" [(ngModel)]="barcodeInput" (keyup.enter)="scanBarcode()" [placeholder]="'::ScanBarcodeOrSearch' | abpLocalization" autofocus>
                        <button class="btn btn-primary" (click)="scanBarcode()">{{ '::Add' | abpLocalization }}</button>
                    </div>
                </div>
            </div>

            <div class="card">
                <div class="card-header bg-light">
                    <h5 class="mb-0"><i class="fas fa-shopping-cart me-2"></i> {{ '::ShoppingList' | abpLocalization }}</h5>
                </div>
                <div class="table-responsive">
                    <table class="table table-hover align-middle mb-0">
                        <thead>
                            <tr>
                                <th>{{ '::Product' | abpLocalization }}</th>
                                <th width="120">{{ '::Price' | abpLocalization }}</th>
                                <th width="120">{{ '::Qty' | abpLocalization }}</th>
                                <th width="120">{{ '::Total' | abpLocalization }}</th>
                                <th width="50"></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr *ngFor="let item of cartItems; let i = index">
                                <td>
                                    <strong>{{ item.name }}</strong><br>
                                    <small class="text-muted">{{ item.barcode }}</small>
                                </td>
                                <td>{{ item.price | currency }}</td>
                                <td>
                                    <input type="number" class="form-control form-control-sm" [(ngModel)]="item.quantity" (change)="updateTotal()" min="1">
                                </td>
                                <td>{{ item.price * item.quantity | currency }}</td>
                                <td>
                                    <button class="btn btn-sm btn-outline-danger" (click)="removeItem(i)"><i class="fas fa-trash"></i></button>
                                </td>
                            </tr>
                            <tr *ngIf="cartItems.length === 0">
                                <td colspan="5" class="text-center py-4 text-muted">{{ '::CartIsEmpty' | abpLocalization }}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

        <!-- Right: Summary & Checkout -->
        <div class="col-md-4">
            <div class="card bg-primary text-white mb-3">
                <div class="card-body text-center">
                    <h3>{{ totalAmount | currency }}</h3>
                    <small>{{ '::TotalAmount' | abpLocalization }}</small>
                </div>
            </div>

            <div class="card">
                <div class="card-body">
                    <div class="mb-3">
                        <label class="form-label">{{ '::Patient' | abpLocalization }}</label>
                        <input type="text" class="form-control" placeholder="Search Patient (Optional)">
                        <!-- Simple mockup, real implementation needs Patient Search/Select -->
                    </div>
                    
                    <div class="mb-3">
                        <label class="form-label">{{ '::PaymentMethod' | abpLocalization }}</label>
                        <select class="form-select" [(ngModel)]="paymentMethod">
                            <option [ngValue]="1">{{ '::Cash' | abpLocalization }}</option>
                            <option [ngValue]="2">{{ '::Card' | abpLocalization }}</option>
                        </select>
                    </div>

                    <div class="mb-3">
                        <label class="form-label">{{ '::AmountPaid' | abpLocalization }}</label>
                        <input type="number" class="form-control" [(ngModel)]="paidAmount" (change)="calculateChange()">
                    </div>
                    
                    <div class="d-flex justify-content-between mb-3" *ngIf="paidAmount > 0">
                         <span>{{ '::Change' | abpLocalization }}:</span>
                         <strong>{{ (paidAmount - totalAmount) | currency }}</strong>
                    </div>

                    <div class="d-grid">
                        <button class="btn btn-success btn-lg" (click)="checkout()" [disabled]="cartItems.length === 0">
                            <i class="fas fa-cash-register me-2"></i> {{ '::Checkout' | abpLocalization }}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
  `
})
export class PharmacyPosComponent implements OnInit {
    barcodeInput = '';
    cartItems: any[] = [];
    totalAmount = 0;
    paidAmount = 0;
    paymentMethod = 1;
    patientId = null; // Should be Guid or null

    constructor(
        private posService: PosService,
        private toaster: ToasterService
    ) { }

    ngOnInit() { }

    scanBarcode() {
        if (!this.barcodeInput) return;

        this.posService.getProductByBarcode(this.barcodeInput).subscribe({
            next: (product) => {
                this.addToCart(product);
                this.barcodeInput = '';
            },
            error: () => {
                this.toaster.error('::ProductNotFound', '::Error');
            }
        });
    }

    addToCart(product: PosProductDto) {
        const existing = this.cartItems.find(x => x.id === product.id);
        if (existing) {
            existing.quantity++;
        } else {
            this.cartItems.push({
                ...product,
                quantity: 1
            });
        }
        this.updateTotal();
    }

    removeItem(index: number) {
        this.cartItems.splice(index, 1);
        this.updateTotal();
    }

    updateTotal() {
        this.totalAmount = this.cartItems.reduce((sum, item) => sum + (item.price * item.quantity), 0);
        this.calculateChange();
    }

    calculateChange() {
        // Logic for change display
    }

    checkout() {
        if (this.cartItems.length === 0) return;

        const sale: PosSaleDto = {
            patientId: this.patientId || null, // Handle null properly in backend
            totalAmount: this.totalAmount,
            paidAmount: this.paidAmount,
            paymentMethod: this.paymentMethod,
            items: this.cartItems.map(x => ({
                drugId: x.id,
                quantity: x.quantity,
                unitPrice: x.price,
                discount: 0
            }))
        };

        this.posService.processSale(sale).subscribe(() => {
            this.toaster.success('::SaleCompleted', '::Success');
            this.cartItems = [];
            this.totalAmount = 0;
            this.paidAmount = 0;
        });
    }
}
