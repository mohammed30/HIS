import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit, inject } from '@angular/core';
import { BankTransactionService } from '../../../proxy/accounting/bank-transaction.service';
import { BankTransactionDto } from '../../../proxy/accounting/dtos/models';
import { bankTransactionTypeOptions } from '../../../proxy/accounting/bank-transaction-type.enum';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { AccountService } from '../../../proxy/accounting/account.service';
import { AccountDto } from '../../../proxy/accounting/dtos/models';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import {
  NgbDateNativeAdapter,
  NgbDateAdapter,
  NgbDatepickerModule,
} from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-bank-transactions',
  standalone: true,
  imports: [ThemeSharedModule, CoreModule, NgbDatepickerModule, ReactiveFormsModule, CommonModule],
  templateUrl: './bank-transactions.html',
  styleUrl: './bank-transactions.scss',
  providers: [ListService, { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
})
export class BankTransactions implements OnInit {
  items: BankTransactionDto[] = [];
  form: FormGroup;
  isModalOpen = false;
  bankTransactionTypeOptions = bankTransactionTypeOptions;
  leafAccounts: AccountDto[] = [];

  private readonly list = inject(ListService);
  private readonly service = inject(BankTransactionService);
  private readonly accountService = inject(AccountService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationService);

  ngOnInit() {
    const stream = (query: any) => this.service.getList(query);

    this.list.hookToQuery(stream).subscribe((response: PagedResultDto<BankTransactionDto>) => {
      this.items = response.items;
    });

    this.accountService.getLookup().subscribe((res: any) => {
      this.leafAccounts = res || [];
    });
  }

  create() {
    this.buildForm();
    this.isModalOpen = true;
  }

  edit(id: string) {
    this.service.get(id).subscribe(res => {
      this.buildForm();
      this.form.patchValue(res);
      
      // Set the text fields for the datalist based on the IDs
      const bAcc = this.leafAccounts.find(a => a.id === (res as any).bankAccountId);
      if (bAcc) this.form.patchValue({ bankAccountText: bAcc.code + ' - ' + (bAcc.nameAr || bAcc.name) });
      
      const oAcc = this.leafAccounts.find(a => a.id === (res as any).oppositeAccountId);
      if (oAcc) this.form.patchValue({ oppositeAccountText: oAcc.code + ' - ' + (oAcc.nameAr || oAcc.name) });

      this.isModalOpen = true;
    });
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe(status => {
      if (status === Confirmation.Status.confirm) {
        this.service.delete(id).subscribe(() => this.list.get());
      }
    });
  }

  buildForm() {
    this.form = this.fb.group({
      id: [null],
      date: [new Date(), Validators.required],
      referenceNumber: [''],
      amount: [0, Validators.required],
      transactionType: [0, Validators.required],
      bankAccountId: [null, Validators.required],
      bankAccountText: ['', Validators.required],
      oppositeAccountId: [null, Validators.required],
      oppositeAccountText: ['', Validators.required],
      description: ['', Validators.required],
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }

    const request = this.form.value.id
      ? this.service.update(this.form.value.id, this.form.value)
      : this.service.create(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.form.reset();
      this.list.get();
    });
  }

  onBankAccountChange(event: any) {
    const val = event.target.value;
    const matched = this.leafAccounts.find(a => (a.code + ' - ' + (a.nameAr || a.name)) === val);
    if (matched) {
      this.form.patchValue({ bankAccountId: matched.id });
    } else {
      this.form.patchValue({ bankAccountId: null });
    }
  }

  onOppositeAccountChange(event: any) {
    const val = event.target.value;
    const matched = this.leafAccounts.find(a => (a.code + ' - ' + (a.nameAr || a.name)) === val);
    if (matched) {
      this.form.patchValue({ oppositeAccountId: matched.id });
    } else {
      this.form.patchValue({ oppositeAccountId: null });
    }
  }
}
