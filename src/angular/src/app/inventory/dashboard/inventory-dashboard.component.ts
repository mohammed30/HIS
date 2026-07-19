import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../proxy/inventory/inventory.service';
import { InventoryItemDto, WarehouseDto } from '../../proxy/inventory/dtos/models';
import { InventoryService as PharmacyInventoryService } from '../../proxy/pharmacy/inventory.service';
import { of } from 'rxjs';
import { tap } from 'rxjs/operators';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { StockTransferDto, CreateStockTransferDto } from '../../proxy/pharmacy/dtos/models';

@Component({
    selector: 'app-inventory-dashboard',
    templateUrl: './inventory-dashboard.component.html',
    providers: [ListService],
    standalone: true,
    imports: [CommonModule, ThemeSharedModule, CoreModule, NgxDatatableModule, FormsModule, ReactiveFormsModule]
})
export class InventoryDashboardComponent implements OnInit {
    data: PagedResultDto<InventoryItemDto> = { items: [], totalCount: 0 };
    lowStockItems: InventoryItemDto[] = [];
    warehouses: WarehouseDto[] = [];
    selectedWarehouseId: string = '';
    searchTerm: string = '';
    selectedType: number | null = null;

    isTransferModalOpen = false;
    transferForm: FormGroup;
    drugs: any[] = []; // Assuming we can fetch items to transfer

    constructor(
        public readonly list: ListService,
        private inventoryService: InventoryService,
        private pharmacyInventoryService: PharmacyInventoryService,
        private fb: FormBuilder
    ) {
        this.transferForm = this.fb.group({
            fromWarehouseId: ['', Validators.required],
            toWarehouseId: ['', Validators.required],
            productId: ['', Validators.required],
            quantity: [1, [Validators.required, Validators.min(1)]],
            notes: ['']
        });
    }

    ngOnInit() {
        this.loadWarehouses();

        const streamCreator = (query) => {
            if (!this.selectedWarehouseId) return this.emptypagedResult();
            
            return this.inventoryService.getStockLevels(
                this.selectedWarehouseId,
                this.searchTerm || undefined,
                (this.selectedType as any) || undefined
            ).pipe(
                tap(response => {
                    this.data = response;
                    // Filter for low stock
                    this.lowStockItems = response.items.filter(item => item.minStockLevel > 0 && item.quantity <= item.minStockLevel);
                })
            );
        };

        this.list.hookToQuery(streamCreator).subscribe((response) => {
            // Handled in tap/streamCreator usually, but list service subscription updates 'response' too.
            // Double check: list.hookToQuery returns the Observable from streamCreator.
            // We can just rely on the subscription here if we want, or side-effect in stream.
            // Actually, list service handles 'data' internal state if we use it, but here we set this.data manually?
            // The template uses 'data.items'.
            // Let's ensure lowStockItems is updated.
            this.data = response;
            this.lowStockItems = response.items.filter(item => item.minStockLevel > 0 && item.quantity <= item.minStockLevel);
        });
    }

    loadWarehouses() {
        this.inventoryService.getWarehouseList({ maxResultCount: 100 } as any).subscribe(res => {
            this.warehouses = res.items;
            if (this.warehouses.length > 0) {
                this.selectedWarehouseId = this.warehouses[0].id;
                this.list.get();
            }
        });
    }

    onWarehouseChange() {
        this.list.get();
    }

    onSearch() {
        this.list.get();
    }

    onTypeChange() {
        this.list.get();
    }

    emptypagedResult() {
        return of({ items: [], totalCount: 0 } as PagedResultDto<InventoryItemDto>);
    }

    openTransferModal() {
        this.transferForm.reset({
            fromWarehouseId: this.selectedWarehouseId,
            toWarehouseId: '',
            productId: '',
            quantity: 1,
            notes: ''
        });
        
        // Fetch products available in the selected warehouse to transfer
        if (this.selectedWarehouseId) {
            this.inventoryService.getStockLevels(this.selectedWarehouseId).subscribe(res => {
                this.drugs = res.items.filter(i => i.quantity > 0);
            });
        }
        
        this.isTransferModalOpen = true;
    }

    closeTransferModal() {
        this.isTransferModalOpen = false;
    }

    submitTransfer() {
        if (this.transferForm.invalid) return;

        const val = this.transferForm.value;
        if (val.fromWarehouseId === val.toWarehouseId) {
            alert('Cannot transfer to the same warehouse');
            return;
        }

        const createDto: any = {
            fromWarehouseId: val.fromWarehouseId,
            toWarehouseId: val.toWarehouseId,
            notes: val.notes,
            items: [
                {
                    drugId: val.productId, // Sending ServiceItemId as DrugId
                    quantity: val.quantity
                }
            ]
        };

        this.pharmacyInventoryService.createTransfer(createDto).subscribe(transfer => {
            if (transfer && transfer.id) {
                this.pharmacyInventoryService.processTransfer(transfer.id).subscribe(() => {
                    alert('تم التحويل بنجاح');
                    this.closeTransferModal();
                    this.list.get(); // Refresh table
                });
            }
        });
    }
}
