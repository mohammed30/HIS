import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../../proxy/accounting/account.service';
import { DiscountsReportDto, DateRangeDto } from '../../../proxy/accounting/dtos/models';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-discounts-report',
  standalone: true,
  imports: [CommonModule, CoreModule, ThemeSharedModule, FormsModule],
  templateUrl: './discounts-report.html',
  styleUrl: './discounts-report.scss'
})
export class DiscountsReport implements OnInit {
  private readonly accountService = inject(AccountService);

  report: DiscountsReportDto = { lines: [], totalDiscounts: 0 };
  startDate = new Date().toISOString().split('T')[0];
  endDate = new Date().toISOString().split('T')[0];
  loading = false;

  ngOnInit() {
    this.fetchReport();
  }

  fetchReport() {
    this.loading = true;
    const input: DateRangeDto = {
      startDate: this.startDate,
      endDate: this.endDate
    };

    this.accountService.getDiscountsReport(input).subscribe({
      next: (res) => {
        this.report = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }
}
