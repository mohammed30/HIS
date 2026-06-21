import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SupplierService } from '../../../proxy/inventory/supplier.service';
import { SupplierDto } from '../../../proxy/inventory/dtos/models';
import { NgbModal, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { SupplierDetailComponent } from '../supplier-detail/supplier-detail';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';

@Component({
  selector: 'app-supplier-list',
  standalone: true,
  imports: [CommonModule, NgbPaginationModule, ThemeSharedModule, CoreModule],
  // Note: SupplierDetailComponent in imports only if it's standalone and used in template. 
  // If used via NgbModal.open, it might not need to be in imports if not used in template directly.
  // Actually, NgbModal.open needs the component class.
  templateUrl: './supplier-list.html',
  styleUrls: ['./supplier-list.scss']
})
export class SupplierListComponent implements OnInit {
  private supplierService = inject(SupplierService);
  private modalService = inject(NgbModal);
  private confirmation = inject(ConfirmationService);

  items: SupplierDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 10;

  // Search
  searchText = '';

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    const skipCount = (this.page - 1) * this.pageSize;
    // SupplierService getList takes PagedAndSortedResultRequestDto
    // We can cast our params or use an object
    this.supplierService.getList({
      skipCount,
      maxResultCount: this.pageSize,
      sorting: 'name asc' // Default sort
    }).subscribe({
      next: (res) => {
        this.items = res.items;
        this.totalCount = res.totalCount;
      },
      error: (err) => console.error(err)
    });
  }

  onPageChange(page: number) {
    this.page = page;
    this.loadData();
  }

  create() {
    this.openModal();
  }

  edit(id: string) {
    this.openModal(id);
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.supplierService.delete(id).subscribe(() => this.loadData());
      }
    });
  }

  private openModal(id?: string) {
    const modalRef = this.modalService.open(SupplierDetailComponent, { size: 'lg' });
    modalRef.componentInstance.id = id; // Pass ID to detail component

    modalRef.result.then((result) => {
      if (result) {
        this.loadData();
      }
    }, () => { });
  }
}
