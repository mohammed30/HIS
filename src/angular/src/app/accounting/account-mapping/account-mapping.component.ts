import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { AccountMappingService, AccountMappingDto } from '../../proxy/accounting/account-mapping.service';
import { AccountService } from '../../proxy/accounting/account.service';
import { AccountDto } from '../../proxy/accounting/dtos/models';

declare var abp: any;

@Component({
  selector: 'app-account-mapping',
  templateUrl: './account-mapping.component.html',
  styleUrls: ['./account-mapping.component.scss'],
  standalone: true,
  imports: [CommonModule, CoreModule, ThemeSharedModule, ReactiveFormsModule, FormsModule]
})
export class AccountMappingComponent implements OnInit {
  mappings: AccountMappingDto[] = [];
  accounts: AccountDto[] = [];
  editingMapping: AccountMappingDto | null = null;
  form: FormGroup;
  isSaving = false;
  isLoading = false;

  typeTranslations: Record<string, string> = {
    'SalesRevenue': 'إيرادات المبيعات',
    'CashAccount': 'حساب النقدية',
    'VATOutput': 'ضريبة المخرجات',
    'VATInput': 'ضريبة المدخلات',
    'Inventory': 'المخزون',
    'COGS': 'تكلفة المبيعات'
  };

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

    this.accountService.getList({ maxResultCount: 1000 }).subscribe({
      next: (response) => {
        // Only allow active leaf accounts (or all active accounts)
        this.accounts = response.items.filter(x => x.isActive);
      }
    });
  }

  editMapping(mapping: AccountMappingDto): void {
    this.editingMapping = mapping;
    this.form.patchValue({
      accountId: mapping.accountId || null
    });
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
