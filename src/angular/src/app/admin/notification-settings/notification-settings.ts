import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '../../proxy/notifications/notification.service';
import { ToasterService } from '@abp/ng.theme.shared';
import { ModuleSubscriptionDto, UpdateModuleSubscriptionDto } from '../../proxy/notifications/models';
import { LookupDto } from '../../proxy/appointments/dtos/models';

@Component({
  selector: 'app-notification-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './notification-settings.html',
  styleUrl: './notification-settings.scss'
})
export class NotificationSettingsComponent implements OnInit {
  modules: ModuleSubscriptionDto[] = [];
  users: LookupDto<string>[] = [];
  filteredUsers: LookupDto<string>[] = [];
  
  isSaving = false;
  selectedUserId: string = '';
  searchTerm: string = '';
  showToast: boolean = true;

  constructor(
    private notificationService: NotificationService,
    private toasterService: ToasterService
  ) {}

  ngOnInit() {
    this.loadData();
    const toastSetting = localStorage.getItem('Notifications.ShowToast');
    this.showToast = toastSetting !== 'false';
  }

  saveToastSetting() {
    localStorage.setItem('Notifications.ShowToast', this.showToast.toString());
  }

  testToast() {
    this.toasterService.success('هذه رسالة تجريبية للتأكد من عمل النوافذ المنبثقة بنجاح!', 'تجربة التنبيه');
  }

  loadData() {
    this.notificationService.getUserLookup().subscribe((users: any) => {
      this.users = users;
      this.filteredUsers = [...this.users];
      this.notificationService.getModuleSubscriptions().subscribe((modules: ModuleSubscriptionDto[]) => {
        this.modules = modules;
      });
    });
  }

  filterUsers() {
    if (!this.searchTerm) {
      this.filteredUsers = [...this.users];
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredUsers = this.users.filter(u => u.name?.toLowerCase().includes(term));
    }
  }

  get selectedUser() {
    return this.users.find(u => u.id === this.selectedUserId);
  }

  toggleUserSubscription(module: ModuleSubscriptionDto, userId: string, event: any) {
    const isChecked = event.target.checked;
    if (isChecked) {
      if (!module.subscribedUserIds.includes(userId)) {
        module.subscribedUserIds.push(userId);
      }
    } else {
      module.subscribedUserIds = module.subscribedUserIds.filter(id => id !== userId);
    }
  }

  isUserSubscribed(module: ModuleSubscriptionDto, userId: string): boolean {
    return module.subscribedUserIds.includes(userId);
  }

  selectAllForUser() {
    if (!this.selectedUserId) return;
    this.modules.forEach(m => {
      if (!m.subscribedUserIds.includes(this.selectedUserId)) {
        m.subscribedUserIds.push(this.selectedUserId);
      }
    });
  }

  deselectAllForUser() {
    if (!this.selectedUserId) return;
    this.modules.forEach(m => {
      m.subscribedUserIds = m.subscribedUserIds.filter(id => id !== this.selectedUserId);
    });
  }

  save() {
    this.isSaving = true;
    localStorage.setItem('Notifications.ShowToast', this.showToast.toString());
    const input: UpdateModuleSubscriptionDto[] = this.modules.map(m => ({
      moduleName: m.moduleName,
      subscribedUserIds: m.subscribedUserIds
    }));

    this.notificationService.updateModuleSubscriptions(input).subscribe({
      next: () => {
        this.isSaving = false;
        alert('تم حفظ الإعدادات بنجاح');
      },
      error: () => {
        this.isSaving = false;
        alert('حدث خطأ أثناء الحفظ');
      }
    });
  }
}
