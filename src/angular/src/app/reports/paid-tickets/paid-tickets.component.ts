import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule, PagedResultDto } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ReportService, PaidTicketDto, GetPaidTicketsInput } from '../../proxy/reports/report.service';

@Component({
  selector: 'app-paid-tickets',
  standalone: true,
  imports: [CommonModule, FormsModule, ThemeSharedModule, CoreModule, NgbModule],
  templateUrl: './paid-tickets.component.html',
  styleUrls: ['./paid-tickets.component.scss']
})
export class PaidTicketsComponent implements OnInit {
  data: PagedResultDto<PaidTicketDto> = { items: [], totalCount: 0 };
  filters: GetPaidTicketsInput = { 
    maxResultCount: 10, 
    skipCount: 0,
    fromDate: null,
    toDate: null,
    creatorUser: ''
  };
  page = 1;

  constructor(
    private reportService: ReportService,
    private toaster: ToasterService
  ) {}

  ngOnInit(): void {
    const today = new Date().toISOString().split('T')[0];
    this.filters.fromDate = today;
    this.filters.toDate = today;
    this.loadData();
  }

  loadData() {
    this.filters.skipCount = (this.page - 1) * this.filters.maxResultCount;
    this.reportService.getPaidTickets(this.filters).subscribe(response => {
      this.data = response;
    });
  }

  reprint(appointmentId: string) {
    // Navigate to reprint URL (assuming an existing endpoint)
    window.open(`/api/app/appointment/${appointmentId}/ticket-pdf`, '_blank');
  }

  refund(appointmentId: string) {
    if (confirm('هل أنت متأكد من رغبتك في إرجاع هذه التذكرة؟ سيتم إنشاء قيد عكسي.')) {
      this.reportService.refundTicket(appointmentId).subscribe(() => {
        this.toaster.success('تم إرجاع التذكرة بنجاح', 'عملية ناجحة');
        this.loadData();
      });
    }
  }

  exportPdf() {
    const url = this.reportService.getPaidTicketsPdf(this.filters);
    window.open(url, '_blank');
  }
}
