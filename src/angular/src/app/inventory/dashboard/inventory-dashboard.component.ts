import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { FormsModule } from '@angular/forms';
import { InventoryService } from '../../proxy/inventory/inventory.service';
import { InventoryItemDto, WarehouseDto } from '../../proxy/inventory/models';
import { of } from 'rxjs';

@Component({
    selector: 'app-inventory-dashboard',
    templateUrl: './inventory-dashboard.component.html',
    providers: [ListService],
    standalone: true,
    imports: [CommonModule, ThemeSharedModule, CoreModule, NgxDatatableModule, FormsModule]
})
export class InventoryDashboardComponent implements OnInit {
    data: PagedResultDto<InventoryItemDto> = { items: [], totalCount: 0 };
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
            return this.inventoryService.getStockLevels(this.selectedWarehouseId);
        };

        this.list.hookToQuery(streamCreator).subscribe((response) => {
            this.data = response;
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
