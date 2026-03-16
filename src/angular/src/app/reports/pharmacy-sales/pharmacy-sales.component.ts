import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule, PagedResultDto } from '@abp/ng.core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ReportService, PharmacySalesDto, GetPharmacySalesInput } from '../../proxy/reports/report.service';

@Component({
  selector: 'app-pharmacy-sales',
  standalone: true,
  imports: [CommonModule, FormsModule, ThemeSharedModule, CoreModule, NgbModule],
  templateUrl: './pharmacy-sales.component.html',
  styleUrls: ['./pharmacy-sales.component.scss']
})
export class PharmacySalesComponent implements OnInit {
  data: PagedResultDto<PharmacySalesDto> = { items: [], totalCount: 0 };
  filters: GetPharmacySalesInput = { 
    maxResultCount: 10, 
    skipCount: 0,
    fromDate: null,
    toDate: null
  };
  page = 1;
  totalSum = 0;

  constructor(private reportService: ReportService) {}

  ngOnInit(): void {
    const today = new Date();
    const localToday = new Date(today.getTime() - today.getTimezoneOffset() * 60000).toISOString().split('T')[0];
    this.filters.fromDate = localToday;
    this.filters.toDate = localToday;
    this.loadData();
  }

  loadData() {
    this.filters.skipCount = (this.page - 1) * this.filters.maxResultCount;
    this.reportService.getPharmacySales(this.filters).subscribe(response => {
      this.data = response;
      // Sum the totalAmount from each item. ABP proxies use camelCase for properties.
      this.totalSum = this.data.items.reduce((acc, item) => acc + (item.totalAmount || 0), 0);
    });
  }

  exportPdf() {
    const url = this.reportService.getPharmacySalesPdf(this.filters);
    window.open(url, '_blank');
  }
}
