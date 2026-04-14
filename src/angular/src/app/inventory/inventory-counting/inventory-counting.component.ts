import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule, PagedResultDto } from '@abp/ng.core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ToasterService } from '@abp/ng.theme.shared';
import { InventoryCountService, InventoryService, InventoryCountDto, InventoryCountItemDto, GetInventoryCountsInput, CreateInventoryCountDto, UpdateInventoryCountItemDto } from '../../proxy/inventory';

@Component({
  selector: 'app-inventory-counting',
  standalone: true,
  imports: [CommonModule, FormsModule, ThemeSharedModule, CoreModule, NgbModule],
  templateUrl: './inventory-counting.component.html'
})
export class InventoryCountingComponent implements OnInit {
  private countService = inject(InventoryCountService);
  private inventoryService = inject(InventoryService);
  private toaster = inject(ToasterService);

  list: PagedResultDto<InventoryCountDto> = { items: [], totalCount: 0 };
  filters: GetInventoryCountsInput = { maxResultCount: 10, skipCount: 0 };
  
  warehouses: any[] = [];
  selectedCount: InventoryCountDto | null = null;
  isModalOpen = false;
  isCreateMode = false;
  newCount: CreateInventoryCountDto = { warehouseId: null, countDate: new Date().toISOString().split('T')[0], notes: '' };

  ngOnInit(): void {
    this.loadList();
    this.loadWarehouses();
  }

  loadWarehouses() {
    this.inventoryService.getWarehouseLookup().subscribe(res => {
      this.warehouses = res as any; // Assert as any to handle proxy list/object discrepancy
    });
  }

  loadList() {
    this.countService.getList(this.filters).subscribe(res => {
      this.list = res;
    });
  }

  openCreateModal() {
    this.isCreateMode = true;
    this.newCount = { warehouseId: null, countDate: new Date().toISOString().split('T')[0], notes: '' };
    this.isModalOpen = true;
  }

  createCount() {
    if (!this.newCount.warehouseId) return;
    this.countService.create(this.newCount).subscribe(res => {
      this.toaster.success('تم بدء عملية الجرد بنجاح');
      this.viewCount(res.id);
      this.isModalOpen = false;
      this.loadList();
    });
  }

  viewCount(id: string) {
    this.countService.get(id).subscribe(res => {
      this.selectedCount = res;
      this.isCreateMode = false;
      this.isModalOpen = true;
    });
  }

  updateItem(item: InventoryCountItemDto) {
    const input: UpdateInventoryCountItemDto = { id: item.id, countedQuantity: item.countedQuantity, notes: item.notes };
    this.countService.updateItem(this.selectedCount.id, input).subscribe(() => {
      // Logic for inline update feedback if needed
      // Difference is updated on the client side for immediate feedback
      item.difference = item.countedQuantity - item.systemQuantity;
    });
  }

  finalize() {
    if (!confirm('هل أنت متأكد من إنهاء عملية الجرد؟ سيؤدي ذلك إلى تسوية الكميات في المخازن آلياً.')) return;
    this.countService.finalize(this.selectedCount.id).subscribe(() => {
      this.toaster.success('تمت تسوية الجرد وإغلاق العملية بنجاح');
      this.isModalOpen = false;
      this.loadList();
    });
  }

  cancel() {
    if (!confirm('هل تريد إلغاء عملية الجرد؟')) return;
    this.countService.cancel(this.selectedCount.id).subscribe(() => {
      this.isModalOpen = false;
      this.loadList();
    });
  }
}
