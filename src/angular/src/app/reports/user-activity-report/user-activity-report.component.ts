import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { IdentityUserService, IdentityUserDto } from '@abp/ng.identity/proxy';
import { RestService } from '@abp/ng.core';

export interface UserActivityFrequencyDto {
  userId?: string;
  userName?: string;
  module?: string;
  entityType?: string;
  action?: string;
  date: string;
  lastAccessTime: string;
  frequencyCount: number;
}

export interface GetUserActivityFrequencyInput {
  maxResultCount: number;
  skipCount: number;
  userId?: string | null;
  module?: string | null;
  startDate?: string | null;
  endDate?: string | null;
}

@Component({
  selector: 'app-user-activity-report',
  standalone: true,
  imports: [CommonModule, FormsModule, ThemeSharedModule],
  templateUrl: './user-activity-report.component.html',
  styleUrls: ['./user-activity-report.component.scss']
})
export class UserActivityReportComponent implements OnInit {
  items: UserActivityFrequencyDto[] = [];
  totalCount = 0;
  users: IdentityUserDto[] = [];
  isTableLoading = false;

  filter: GetUserActivityFrequencyInput = {
    maxResultCount: 10,
    skipCount: 0,
    userId: null,
    module: '',
    startDate: new Date().toISOString().split('T')[0],
    endDate: new Date().toISOString().split('T')[0]
  };

  modules = [
    { value: '', label: 'الكل' },
    { value: 'Patient', label: 'المرضى (Patients)' },
    { value: 'Pharmacy', label: 'الصيدلية (Pharmacy)' },
    { value: 'Appointment', label: 'المواعيد (Appointments)' },
    { value: 'Billing', label: 'الفواتير والحسابات (Billing)' },
    { value: 'Inventory', label: 'المخزون (Inventory)' },
    { value: 'Radiology', label: 'الأشعة (Radiology)' },
    { value: 'Laboratory', label: 'المختبر (Laboratory)' },
    { value: 'HR', label: 'الموارد البشرية (HR)' }
  ];

  constructor(
    private restService: RestService,
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
    this.restService.request<any, { items: UserActivityFrequencyDto[], totalCount: number }>({
      method: 'GET',
      url: '/api/app/user-activity-report',
      params: {
        maxResultCount: this.filter.maxResultCount,
        skipCount: this.filter.skipCount,
        userId: this.filter.userId,
        module: this.filter.module,
        startDate: this.filter.startDate,
        endDate: this.filter.endDate
      }
    }).subscribe({
      next: (res) => {
        this.items = res.items;
        this.totalCount = res.totalCount;
        this.isTableLoading = false;
      },
      error: () => {
        this.isTableLoading = false;
      }
    });
  }

  onPageChange(page: number) {
    this.filter.skipCount = page * this.filter.maxResultCount;
    this.search();
  }

  reset() {
    this.filter = {
      maxResultCount: 10,
      skipCount: 0,
      userId: null,
      module: '',
      startDate: new Date().toISOString().split('T')[0],
      endDate: new Date().toISOString().split('T')[0]
    };
    this.search();
  }
}
