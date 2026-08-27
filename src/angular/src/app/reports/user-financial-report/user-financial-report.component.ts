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

  cashTotalIncoming = 0;
  cashTotalOutgoing = 0;
  cashNetTotal = 0;

  bankTotalIncoming = 0;
  bankTotalOutgoing = 0;
  bankNetTotal = 0;

  cashItems: UserFinancialTransactionDto[] = [];
  bankItems: UserFinancialTransactionDto[] = [];
  
  // Grouped items for the print summary view
  groupedCashItems: { moduleName: string; total: number }[] = [];
  groupedBankItems: { moduleName: string; total: number }[] = [];

  isTableLoading = false;
  printDate = new Date();
  selectedUserName = '';

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
    // For print reports, we usually want all transactions in the period, 
    // so we override maxResultCount if we want everything, but let's stick to 1000 for safety.
    const queryFilter = { ...this.filter, maxResultCount: 1000 };
    
    this.reportService.getList(queryFilter).subscribe((res) => {
      this.items = res.items;
      this.totalCount = res.totalCount;
      
      // Update selected user name for print header
      if (this.filter.userId) {
        const u = this.users.find(x => x.id === this.filter.userId);
        this.selectedUserName = u ? (u.name || u.userName) : 'غير معروف';
      } else {
        this.selectedUserName = 'الكل';
      }

      this.processData();
      this.isTableLoading = false;
    });
  }

  processData() {
    this.cashItems = this.items.filter(x => x.paymentCategory === 'Cash');
    this.bankItems = this.items.filter(x => x.paymentCategory === 'Bank');

    // Totals - Overall
    this.totalIncoming = this.items.filter(x => x.amount > 0).reduce((sum, current) => sum + current.amount, 0);
    this.totalOutgoing = Math.abs(this.items.filter(x => x.amount < 0).reduce((sum, current) => sum + current.amount, 0));
    this.netTotal = this.totalIncoming - this.totalOutgoing;

    // Totals - Cash
    this.cashTotalIncoming = this.cashItems.filter(x => x.amount > 0).reduce((sum, current) => sum + current.amount, 0);
    this.cashTotalOutgoing = Math.abs(this.cashItems.filter(x => x.amount < 0).reduce((sum, current) => sum + current.amount, 0));
    this.cashNetTotal = this.cashTotalIncoming - this.cashTotalOutgoing;

    // Totals - Bank
    this.bankTotalIncoming = this.bankItems.filter(x => x.amount > 0).reduce((sum, current) => sum + current.amount, 0);
    this.bankTotalOutgoing = Math.abs(this.bankItems.filter(x => x.amount < 0).reduce((sum, current) => sum + current.amount, 0));
    this.bankNetTotal = this.bankTotalIncoming - this.bankTotalOutgoing;

    // Grouping for Summary Table
    this.groupedCashItems = this.groupItemsByModule(this.cashItems);
    this.groupedBankItems = this.groupItemsByModule(this.bankItems);
  }
  
  private groupItemsByModule(itemsToGroup: UserFinancialTransactionDto[]) {
    const map = new Map<string, number>();
    itemsToGroup.forEach(item => {
        const mod = item.transactionType || 'أخرى';
        const current = map.get(mod) || 0;
        map.set(mod, current + item.amount);
    });
    return Array.from(map.entries()).map(([moduleName, total]) => ({ moduleName, total }));
  }

  onPageChange(page: number) {
    this.filter.skipCount = page * this.filter.maxResultCount;
    this.search();
  }

  reset() {
    this.filter = {
      maxResultCount: 1000,
      skipCount: 0,
      moduleName: '',
      userId: null,
      startDate: new Date().toISOString().split('T')[0],
      endDate: new Date().toISOString().split('T')[0]
    };
    this.search();
  }

  printReport() {
    if (!this.filter.userId) {
      alert('الرجاء اختيار مستخدم محدد لطباعة عهدته');
      return;
    }

    const apiUrl = '/api/app/user-financial-report/print-document';
    
    // Using native fetch for easier Blob handling since ABP RestService defaults to JSON
    let queryParams = new URLSearchParams();
    if (this.filter.userId) queryParams.append('userId', this.filter.userId);
    if (this.filter.moduleName) queryParams.append('moduleName', this.filter.moduleName);
    if (this.filter.startDate) queryParams.append('startDate', this.filter.startDate);
    if (this.filter.endDate) queryParams.append('endDate', this.filter.endDate);

    fetch(`${apiUrl}?${queryParams.toString()}`, {
      method: 'GET',
    })
    .then(response => {
        if (!response.ok) throw new Error("فشل في تحميل التقرير");
        return response.blob();
    })
    .then(blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Custody_Report_${this.selectedUserName}_${new Date().getTime()}.pdf`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    })
    .catch(error => {
        console.error('Error downloading report:', error);
        alert('حدث خطأ أثناء إنشاء تقرير الطباعة');
    });
  }
}
