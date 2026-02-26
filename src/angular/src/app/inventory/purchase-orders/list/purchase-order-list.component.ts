import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { Router, RouterModule } from '@angular/router';
import { ThemeSharedModule, ConfirmationService, Confirmation, ToasterService } from '@abp/ng.theme.shared';
import { PurchaseOrderService } from '../../../proxy/inventory/purchase-order.service';
import { PurchaseOrderDto } from '../../../proxy/inventory/dtos/models';
import { PurchaseOrderStatus } from '../../../proxy/inventory/purchase-order-status.enum';

@Component({
    selector: 'app-purchase-order-list',
    standalone: true,
    imports: [CommonModule, NgbPaginationModule, RouterModule, ThemeSharedModule],
    templateUrl: './purchase-order-list.component.html'
})
export class PurchaseOrderListComponent implements OnInit {
    private service = inject(PurchaseOrderService);
    private confirmation = inject(ConfirmationService);
    private toaster = inject(ToasterService);
    private router = inject(Router);

    items: PurchaseOrderDto[] = [];
    totalCount = 0;
    page = 1;
    pageSize = 10;

    PurchaseOrderStatus = PurchaseOrderStatus; // For enum access in template

    ngOnInit() {
        this.loadData();
    }

    loadData() {
        const skipCount = (this.page - 1) * this.pageSize;
        this.service.getList({ skipCount, maxResultCount: this.pageSize, sorting: 'orderDate desc' }).subscribe({
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
        this.router.navigate(['/inventory/purchase-orders/create']);
    }

    edit(id: string) {
        this.router.navigate(['/inventory/purchase-orders/edit', id]);
    }

    delete(id: string) {
        this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.service.delete(id).subscribe(() => {
                    this.toaster.success('::SuccessfullyDeleted');
                    this.loadData();
                });
            }
        });
    }
}
