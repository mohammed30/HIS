import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { UserFinancialReportService } from '@proxy/reports/user-financial-report.service';
import { GetUserFinancialTransactionsInput, UserFinancialTransactionDto } from '@proxy/reports/models';
import { IdentityUserService, IdentityUserDto } from '@abp/ng.identity/proxy';

@Component({
  selector: 'app-user-financial-report',
  standalone: true,
  imports: [CommonModule, FormsModule, ThemeSharedModule],
  templateUrl: './user-financial-report.component.html',
  styleUrls: ['./user-financial-report.component.scss']
})
export class UserFinancialReportComponent implements OnInit {
  items: UserFinancialTransactionDto[] = [];
  totalCount: number = 0;
  users: IdentityUserDto[] = [];

  filter: GetUserFinancialTransactionsInput = {
    maxResultCount: 10,
    skipCount: 0,
    moduleName: '',
    userId: null,
    startDate: new Date().toISOString().split('T')[0],
    endDate: new Date().toISOString().split('T')[0]
  };

  modules = [
    { value: '', label: 'الكل' },
    { value: 'Payment', label: 'المدفوعات (فواتير)' },
    { value: 'InpatientDeposit', label: 'تأمينات التنويم' },
    { value: 'ReceiptVoucher', label: 'سندات القبض' },
    { value: 'PaymentVoucher', label: 'سندات الصرف' }
  ];

  totalIncoming = 0;
  totalOutgoing = 0;
  netTotal = 0;
  isTableLoading = false;

  constructor(
    private reportService: UserFinancialReportService,
    private userService: IdentityUserService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.search();
  }

  loadUsers() {
    this.userService.getList({ maxResultCount: 1000 }).subscribe((res) => {
      this.users = res.items.filter(u => u.userName !== 'admin');
    });
  }

  search() {
    this.isTableLoading = true;
    this.reportService.getList(this.filter).subscribe((res) => {
      this.items = res.items;
      this.totalCount = res.totalCount;
      this.calculateTotals();
      this.isTableLoading = false;
    });
  }

  calculateTotals() {
    this.totalIncoming = this.items.filter(x => x.amount > 0).reduce((sum, current) => sum + current.amount, 0);
    this.totalOutgoing = Math.abs(this.items.filter(x => x.amount < 0).reduce((sum, current) => sum + current.amount, 0));
    this.netTotal = this.totalIncoming - this.totalOutgoing;
  }

  onPageChange(page: number) {
    this.filter.skipCount = page * this.filter.maxResultCount;
    this.search();
  }

  reset() {
    this.filter = {
      maxResultCount: 10,
      skipCount: 0,
      moduleName: '',
      userId: null,
      startDate: new Date().toISOString().split('T')[0],
      endDate: new Date().toISOString().split('T')[0]
    };
    this.search();
  }
}
