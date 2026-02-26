import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule, ListService, PagedResultDto } from '@abp/ng.core';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { InventoryService } from '../../proxy/pharmacy/inventory.service';
import { StockTransferDto } from '../../proxy/pharmacy/dtos/models';
import { InventoryItemDto } from '../../proxy/inventory/dtos/models';

@Component({
    selector: 'app-stock-dashboard',
    standalone: true,
    imports: [CommonModule, ThemeSharedModule, NgxDatatableModule, CoreModule],
    providers: [ListService],
    template: `
    <div class="row">
        <!-- Low Stock Alert -->
        <div class="col-md-6">
            <div class="card border-danger mb-3">
                <div class="card-header bg-danger text-white">
                    <h5 class="card-title mb-0"><i class="fas fa-exclamation-triangle me-2"></i> {{ '::LowStockAlert' | abpLocalization }}</h5>
                </div>
                <div class="card-body p-0">
                    <ngx-datatable [rows]="lowStockItems" [columnMode]="'force'" [headerHeight]="50" [footerHeight]="50" [rowHeight]="'auto'" [limit]="5">
                        <ngx-datatable-column [name]="'::Product' | abpLocalization" prop="productName"></ngx-datatable-column>
                        <ngx-datatable-column [name]="'::Qty' | abpLocalization" prop="quantity">
                            <ng-template let-row="row" ngx-datatable-cell-template>
                                <span class="badge bg-danger">{{ row.quantity }}</span>
                            </ng-template>
                        </ngx-datatable-column>
                    </ngx-datatable>
                </div>
            </div>
        </div>

        <!-- Pending Transfers -->
        <div class="col-md-6">
            <div class="card border-warning mb-3">
                <div class="card-header bg-warning text-dark">
                    <h5 class="card-title mb-0"><i class="fas fa-exchange-alt me-2"></i> {{ '::PendingTransfers' | abpLocalization }}</h5>
                </div>
                <div class="card-body p-0">
                     <ngx-datatable [rows]="transfers.items" [columnMode]="'force'" [headerHeight]="50" [footerHeight]="50" [rowHeight]="'auto'" [limit]="5">
                        <ngx-datatable-column [name]="'::TransferNo' | abpLocalization" prop="transferNumber"></ngx-datatable-column>
                        <ngx-datatable-column [name]="'::From' | abpLocalization" prop="fromWarehouseName"></ngx-datatable-column>
                        <ngx-datatable-column [name]="'::To' | abpLocalization" prop="toWarehouseName"></ngx-datatable-column>
                        <ngx-datatable-column [name]="'::Status' | abpLocalization" prop="status">
                            <ng-template let-value="value" ngx-datatable-cell-template>
                                {{ '::Enum:TransferStatus.' + value | abpLocalization }}
                            </ng-template>
                        </ngx-datatable-column>
                        <ngx-datatable-column [name]="'::Actions' | abpLocalization" sortable="false">
                            <ng-template let-row="row" ngx-datatable-cell-template>
                                <button *ngIf="row.status === 1" class="btn btn-sm btn-success" (click)="processTransfer(row.id)">{{ '::Receive' | abpLocalization }}</button>
                            </ng-template>
                        </ngx-datatable-column>
                    </ngx-datatable>
                </div>
            </div>
        </div>
    </div>
  `
})
export class StockDashboardComponent implements OnInit {
    lowStockItems: any[] = [];
    transfers = { items: [], totalCount: 0 } as PagedResultDto<any>;

    // Inject Generic REST Service or Manual Service
    constructor(
        private inventoryService: InventoryService
    ) { }

    ngOnInit(): void {
        this.loadLowStock();
        this.loadTransfers();
    }

    loadLowStock() {
        this.inventoryService.getLowStockReport({ maxResultCount: 10, skipCount: 0 }).subscribe(res => {
            this.lowStockItems = res.items;
        });
    }

    loadTransfers() {
        this.inventoryService.getTransfers({ maxResultCount: 10, skipCount: 0 }).subscribe(res => {
            this.transfers = res;
        });
    }

    processTransfer(id: string) {
        this.inventoryService.processTransfer(id).subscribe(() => {
            this.loadTransfers();
        });
    }
}
