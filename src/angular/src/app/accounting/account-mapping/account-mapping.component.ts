import { Component, OnInit } from '@angular/core';
import { CommonModule, SlicePipe } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { AccountMappingService } from '../../proxy/accounting/account-mapping.service';
import { AccountService } from '../../proxy/accounting/account.service';
import { AccountDto, AccountMappingDto } from '../../proxy/accounting/dtos/models';

declare var abp: any;

@Component({
  selector: 'app-account-mapping',
  templateUrl: './account-mapping.component.html',
  styleUrls: ['./account-mapping.component.scss'],
  standalone: true,
  imports: [CommonModule, CoreModule, ThemeSharedModule, ReactiveFormsModule, FormsModule, SlicePipe]
})
export class AccountMappingComponent implements OnInit {
  mappings: AccountMappingDto[] = [];
  accounts: AccountDto[] = [];
  filteredAccounts: AccountDto[] = [];
  editingMapping: AccountMappingDto | null = null;
  form: FormGroup;
  isSaving = false;
  isLoading = false;
  searchTerm = '';
  showDropdown = false;

  typeTranslations: Record<string, string> = {
    'SalesRevenue': 'حساب إيرادات المبيعات',
    'CashAccount': 'حساب الخزينة الافتراضي',
    'VATOutput': 'ضريبة مخرجات',
    'VATInput': 'ضريبة مدخلات',
    'Inventory': 'المخزون',
    'COGS': 'تكلفة المبيعات',
    'PatientsReceivable': 'حساب ذمم العملاء / المرضى',
    'InsuranceReceivable': 'حساب ذمم شركات التأمين',
    'InsuranceDiscounts': 'خصومات وفروقات التأمين',
    'InventoryAdjustment': 'تسوية عجز وزيادة المخزون',
    'AccruedInventory': 'البضاعة المستلمة غير المفوترة',
    'CardPaymentBank': 'حساب البنك لشبكة نقاط البيع',
    'PatientDeposits': 'أمانات ودفعات مقدمة للمرضى'
  };

  typeDescriptions: Record<string, string> = {
    'SalesRevenue': 'يستخدم لتسجيل إيرادات المبيعات اليومية بشكل آلي',
    'CashAccount': 'الحساب النقدي الافتراضي لاستلام الدفعات والصرف',
    'VATOutput': 'حساب الضريبة المحصلة من العملاء (المبيعات)',
    'VATInput': 'حساب الضريبة المدفوعة للموردين (المشتريات)',
    'Inventory': 'يستخدم لتتبع قيمة البضاعة والمخزون الفعلي',
    'COGS': 'يسجل تكلفة البضاعة المباعة عند صرفها من المخزون',
    'PatientsReceivable': 'يمثل المبالغ المستحقة على المرضى غير المسددة',
    'InsuranceReceivable': 'يمثل مطالبات المستشفى لدى شركات التأمين',
    'InsuranceDiscounts': 'يستخدم لتسجيل الخصومات الممنوحة حسب عقود التأمين',
    'InventoryAdjustment': 'يستخدم لتسوية الفروقات عند الجرد (عجز أو زيادة)',
    'AccruedInventory': 'يسجل قيمة البضاعة المستلمة التي لم تصل فواتيرها بعد',
    'CardPaymentBank': 'يمثل حساب البنك المربوط بأجهزة الدفع الإلكتروني (الشبكة)',
    'PatientDeposits': 'يسجل المبالغ المدفوعة مقدماً من المرضى قبل تقديم الخدمة'
  };

  getMappingTypeTranslation(typeName: string): string {
    if (!typeName) return '';
    const key = typeName.trim();
    if (this.typeTranslations[key]) return this.typeTranslations[key];
    
    const lowerKey = key.toLowerCase();
    const foundKey = Object.keys(this.typeTranslations).find(k => k.toLowerCase() === lowerKey);
    return foundKey ? this.typeTranslations[foundKey] : key;
  }

  getMappingDescriptionTranslation(typeName: string): string {
    if (!typeName) return '';
    const key = typeName.trim();
    if (this.typeDescriptions[key]) return this.typeDescriptions[key];
    
    const lowerKey = key.toLowerCase();
    const foundKey = Object.keys(this.typeDescriptions).find(k => k.toLowerCase() === lowerKey);
    return foundKey ? this.typeDescriptions[foundKey] : '';
  }

  constructor(
    private mappingService: AccountMappingService,
    private accountService: AccountService,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      accountId: [null]
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;
    this.mappingService.getList().subscribe({
      next: (response) => {
        this.mappings = response.items;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });

    this.accountService.getList({ maxResultCount: 5000, skipCount: 0 }).subscribe({
      next: (response) => {
        // Load all active accounts across all levels
        this.accounts = response.items
          .filter(x => x.isActive)
          .sort((a, b) => (a.code || '').localeCompare(b.code || ''));
        this.filteredAccounts = [...this.accounts];
      }
    });
  }

  editMapping(mapping: AccountMappingDto): void {
    this.editingMapping = mapping;
    this.form.patchValue({
      accountId: mapping.accountId || null
    });
    this.searchTerm = '';
    this.filteredAccounts = [...this.accounts];
    this.showDropdown = false;
  }

  onSearchChange(): void {
    const term = this.searchTerm.trim().toLowerCase();
    if (!term) {
      this.filteredAccounts = [...this.accounts];
    } else {
      this.filteredAccounts = this.accounts.filter(acc =>
        (acc.code || '').toLowerCase().includes(term) ||
        (acc.nameAr || '').toLowerCase().includes(term) ||
        (acc.name || '').toLowerCase().includes(term)
      );
    }
    this.showDropdown = true;
  }

  selectAccount(acc: AccountDto): void {
    this.form.patchValue({ accountId: acc.id });
    this.searchTerm = `[${acc.code}] - ${acc.nameAr || acc.name}`;
    this.showDropdown = false;
  }

  clearAccount(): void {
    this.form.patchValue({ accountId: null });
    this.searchTerm = '';
    this.filteredAccounts = [...this.accounts];
    this.showDropdown = false;
  }

  getSelectedAccountLabel(): string {
    const id = this.form.value.accountId;
    if (!id) return '';
    const acc = this.accounts.find(a => a.id === id);
    return acc ? `[${acc.code}] - ${acc.nameAr || acc.name}` : '';
  }

  onSearchFocus(): void {
    this.showDropdown = true;
    if (!this.searchTerm) {
      this.filteredAccounts = [...this.accounts];
    }
  }

  closeDropdown(): void {
    setTimeout(() => { this.showDropdown = false; }, 200);
  }

  cancelEdit(): void {
    this.editingMapping = null;
  }

  onModalClick(event: MouseEvent): void {
    if ((event.target as HTMLElement).classList.contains('modal')) {
      this.cancelEdit();
    }
  }

  saveMapping(): void {
    if (!this.editingMapping) return;

    this.isSaving = true;
    const accountId = this.form.value.accountId;

    this.mappingService.update(this.editingMapping.id, { accountId: accountId || undefined }).subscribe({
      next: (updated) => {
        const index = this.mappings.findIndex(x => x.id === updated.id);
        if (index > -1) {
          this.mappings[index] = updated;
        }
        this.editingMapping = null;
        this.isSaving = false;
        abp.notify.success('تمت عملية التحديث بنجاح', 'تحديث التوجيه');
      },
      error: () => {
        this.isSaving = false;
      }
    });
  }
}
