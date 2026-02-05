import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { InventoryService } from '../../proxy/inventory/inventory.service';
import { WarehouseDto, CreateUpdateWarehouseDto } from '../../proxy/inventory/dtos/models';

@Component({
    selector: 'app-warehouse-management',
    templateUrl: './warehouse-management.component.html',
    providers: [ListService],
    standalone: true,
    imports: [CommonModule, ThemeSharedModule, CoreModule, ReactiveFormsModule, NgxDatatableModule, NgbDropdownModule]
})
export class WarehouseManagementComponent implements OnInit {
    data: PagedResultDto<WarehouseDto> = { items: [], totalCount: 0 };
    isModalOpen = false;
    form: FormGroup;
    selectedWarehouse: WarehouseDto = {} as WarehouseDto;

    constructor(
        public readonly list: ListService,
        private inventoryService: InventoryService,
        private fb: FormBuilder,
        private confirmation: ConfirmationService
    ) { }

    ngOnInit() {
        const streamCreator = (query) => this.inventoryService.getWarehouseList(query);

        this.list.hookToQuery(streamCreator).subscribe((response) => {
            this.data = response;
        });
    }

    createWarehouse() {
        this.selectedWarehouse = {} as WarehouseDto;
        this.buildForm();
        this.isModalOpen = true;
    }

    editWarehouse(id: string) {
        this.inventoryService.getWarehouseList({ maxResultCount: 1 } as any).subscribe((res) => { // Limitation: API doesn't have Get(id) yet? No, it doesn't.
            // Wait, I need a Get(id) method in the AppService!
            // For now, I'll filter from the list or update AppService.
            // Actually, checking InventoryAppService... it has GetWarehouseList, Create, Update. 
            // It DOES NOT have Get(id). 
            // I will fix this in the backend shortly. For now, I will use the row data passed from the view if possible, or fetch list.
            // Better: Pass the DTO directly from the row.
        });
    }

    // Revised edit method receiving the DTO
    openEditModal(warehouse: WarehouseDto) {
        this.selectedWarehouse = warehouse;
        this.buildForm();
        this.isModalOpen = true;
    }

    buildForm() {
        this.form = this.fb.group({
            name: [this.selectedWarehouse.name || '', [Validators.required, Validators.maxLength(128)]],
            location: [this.selectedWarehouse.location || '', [Validators.maxLength(256)]],
        });
    }

    save() {
        if (this.form.invalid) {
            return;
        }

        const request = this.selectedWarehouse.id
            ? this.inventoryService.updateWarehouse(this.selectedWarehouse.id, this.form.value)
            : this.inventoryService.createWarehouse(this.form.value);

        request.subscribe(() => {
            this.isModalOpen = false;
            this.form.reset();
            this.list.get();
        });
    }

    delete(id: string) {
        // InventoryAppService missing Delete method? 
        // I need to check InventoryAppService.cs again.
        // It has Create, Update, GetList. No Delete.
        // I will add ICrudAppService or Delete method to Backend.
        this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.inventoryService.deleteWarehouse(id).subscribe(() => this.list.get());
            }
        });
    }
}
