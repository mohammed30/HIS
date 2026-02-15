import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../../proxy/accounting/account.service';
import { DailyAccountsReportDto, DateRangeDto } from '../../../proxy/accounting/dtos/models';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-daily-accounts-report',
  standalone: true,
  imports: [CommonModule, CoreModule, ThemeSharedModule, NgbDatepickerModule, FormsModule],
  templateUrl: './daily-accounts-report.html',
  styleUrl: './daily-accounts-report.scss'
})
export class DailyAccountsReport implements OnInit {
  private readonly accountService = inject(AccountService);

  report: DailyAccountsReportDto = { transactions: [], totalReceipts: 0, totalPayments: 0 };
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

    this.accountService.getDailyAccountsReport(input).subscribe({
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
