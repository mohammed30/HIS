import { Component, OnInit } from '@angular/core';
import { InternalRequestService, InternalRequestStatus } from '../../proxy/inventory';
import { InternalRequestDto } from '../../proxy/inventory/dtos/models';
import { PagedResultDto, CoreModule } from '@abp/ng.core';
import { finalize } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-inventory-internal-requests',
  imports: [CommonModule, FormsModule, CoreModule, ThemeSharedModule],
  templateUrl: './internal-requests.html',
  styleUrl: './internal-requests.scss'
})
export class InternalRequestsComponent implements OnInit {
  data: PagedResultDto<InternalRequestDto> = { items: [], totalCount: 0 };
  isLoading = false;
  isModalOpen = false;
  selectedRequest: InternalRequestDto | null = null;
  statusEnum = InternalRequestStatus;

  constructor(
    private internalRequestService: InternalRequestService
  ) {}

  ngOnInit(): void {
    this.getList();
  }

  getList() {
    this.isLoading = true;
    // In a real scenario, filter for requests that are Submitted or Approved to this Store
    this.internalRequestService.getList({ maxResultCount: 10, skipCount: 0, sorting: '' })
      .pipe(finalize(() => this.isLoading = false))
      .subscribe((res) => {
        this.data = res;
      });
  }

  showDetails(request: InternalRequestDto) {
    this.selectedRequest = request;
    this.isModalOpen = true;
  }

  approve(id: string) {
    this.internalRequestService.approveAndFulfill(id).subscribe(() => {
      this.isModalOpen = false;
      this.getList();
    });
  }

  getStatusText(status: number) {
    switch (status) {
      case InternalRequestStatus.Draft: return 'مسودة (التمريض)';
      case InternalRequestStatus.Submitted: return 'بانتظار الموافقة';
      case InternalRequestStatus.Approved: return 'تم الصرف';
      case InternalRequestStatus.Received: return 'مستلم (التمريض)';
      case InternalRequestStatus.Rejected: return 'مرفوض';
      default: return 'غير معروف';
    }
  }
}
