import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../proxy/inventory/inventory.service';
import { InventoryItemDto, WarehouseDto } from '../../proxy/inventory/dtos/models';
import { of } from 'rxjs';
import { tap } from 'rxjs/operators';

@Component({
    selector: 'app-inventory-dashboard',
    templateUrl: './inventory-dashboard.component.html',
    providers: [ListService],
    standalone: true,
    imports: [CommonModule, ThemeSharedModule, CoreModule, NgxDatatableModule, FormsModule]
})
export class InventoryDashboardComponent implements OnInit {
    data: PagedResultDto<InventoryItemDto> = { items: [], totalCount: 0 };
    lowStockItems: InventoryItemDto[] = [];
    warehouses: WarehouseDto[] = [];
    selectedWarehouseId: string = '';

    constructor(
        public readonly list: ListService,
        private inventoryService: InventoryService
    ) { }

    ngOnInit() {
        this.loadWarehouses();

        const streamCreator = (query) => {
            if (!this.selectedWarehouseId) return this.emptypagedResult();
            return this.inventoryService.getStockLevels(this.selectedWarehouseId).pipe(
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

    emptypagedResult() {
        return of({ items: [], totalCount: 0 } as PagedResultDto<InventoryItemDto>);
    }
}
