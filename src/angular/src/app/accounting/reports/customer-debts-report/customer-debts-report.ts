import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../../proxy/accounting/account.service';
import { CustomerDebtsReportDto } from '../../../proxy/accounting/dtos/models';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-customer-debts-report',
  standalone: true,
  imports: [CommonModule, CoreModule, ThemeSharedModule],
  templateUrl: './customer-debts-report.html',
  styleUrl: './customer-debts-report.scss'
})
export class CustomerDebtsReport implements OnInit {
  private readonly accountService = inject(AccountService);

  report: CustomerDebtsReportDto = { debts: [], totalOverallDebt: 0 };
  loading = false;

  ngOnInit() {
    this.fetchReport();
  }

  fetchReport() {
    this.loading = true;
    this.accountService.getCustomerDebtsReport().subscribe({
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
