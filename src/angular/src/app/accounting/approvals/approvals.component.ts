import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { InvoiceService } from '../../proxy/billing/invoice.service';
import { InvoiceDto } from '../../proxy/billing/models';
import { ListService, CoreModule } from '@abp/ng.core';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-approvals',
  standalone: true,
  imports: [CommonModule, ThemeSharedModule, CoreModule, NgbDropdownModule],
  templateUrl: './approvals.component.html',
  styleUrls: ['./approvals.component.scss'],
  providers: [ListService]
})
export class ApprovalsComponent implements OnInit {
  items: InvoiceDto[] = [];
  isLoading = false;
  
  isDetailsModalOpen = false;
  selectedInvoice: InvoiceDto = {} as InvoiceDto;

  constructor(
    public readonly list: ListService,
    private invoiceService: InvoiceService,
    private confirmation: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadApprovals();
  }

  loadApprovals() {
    this.isLoading = true;
    this.invoiceService.getPendingApprovals().subscribe({
      next: (res) => {
        this.items = res;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  showDetails(item: InvoiceDto) {
    this.selectedInvoice = item;
    // Fetch with items if not included
    this.invoiceService.getWithItems(item.id).subscribe((res) => {
        this.selectedInvoice = res;
        this.isDetailsModalOpen = true;
    });
  }

  approve(id: string) {
    this.confirmation.warn('::AreYouSureToApprove', '::Approve').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.invoiceService.approveInvoice(id).subscribe(() => {
          this.loadApprovals();
          this.isDetailsModalOpen = false;
        });
      }
    });
  }

  reject(id: string) {
    this.confirmation.warn('::AreYouSureToReject', '::Reject').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.invoiceService.rejectInvoice(id).subscribe(() => {
          this.loadApprovals();
          this.isDetailsModalOpen = false;
        });
      }
    });
  }
}
