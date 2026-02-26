import { ListService, PagedResultDto, RestService } from '@abp/ng.core';
import { Component, OnInit, inject } from '@angular/core';
import { PaymentVoucherService } from '../../../proxy/accounting/payment-voucher.service';
import { PaymentVoucherDto } from '../../../proxy/accounting/dtos/models';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import { NgbDateNativeAdapter, NgbDateAdapter, NgbDatepickerModule, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { AccountService } from '../../../proxy/accounting/account.service';
import { PaymentMethodService } from '../../../proxy/general/payment-method.service';
import { AccountDto } from '../../../proxy/accounting/dtos/models';
import { PaymentMethodDto } from '../../../proxy/general/dtos/models';

@Component({
  selector: 'app-payment-vouchers',
  standalone: true,
  imports: [
    ThemeSharedModule,
    CoreModule,
    NgbDatepickerModule,
    NgbDropdownModule,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './payment-vouchers.html',
  styleUrl: './payment-vouchers.scss',
  providers: [ListService, { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
})
export class PaymentVouchers implements OnInit {
  items: PaymentVoucherDto[] = [];
  accounts: AccountDto[] = [];
  paymentMethods: PaymentMethodDto[] = [];
  form: FormGroup;
  isModalOpen = false;

  private readonly list = inject(ListService);
  private readonly service = inject(PaymentVoucherService);
  private readonly accountService = inject(AccountService);
  private readonly paymentMethodService = inject(PaymentMethodService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationService);
  private readonly restService = inject(RestService);

  ngOnInit() {
    const stream = (query: any) => this.service.getList(query);

    this.list.hookToQuery(stream).subscribe((response: PagedResultDto<PaymentVoucherDto>) => {
      this.items = response.items;
    });

    this.accountService.getList({ maxResultCount: 1000 } as any).subscribe(res => {
      this.accounts = res.items;
    });

    this.paymentMethodService.getList({ maxResultCount: 1000 } as any).subscribe(res => {
      this.paymentMethods = res.items;
    });
  }

  create() {
    this.buildForm();
    this.isModalOpen = true;
  }

  edit(id: string) {
    this.service.get(id).subscribe((res) => {
      this.buildForm();
      const patchData = {
        ...res,
        date: res.date ? new Date(res.date) : null,
        accountId: res.lines?.length ? res.lines[0].accountId : null
      };
      this.form.patchValue(patchData);
      this.isModalOpen = true;
    });
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.service.delete(id).subscribe(() => this.list.get());
      }
    });
  }

  buildForm() {
    this.form = this.fb.group({
      id: [null],
      date: [new Date(), Validators.required],
      amount: [0, [Validators.required, Validators.min(0.01)]],
      description: ['', Validators.required],
      supplierId: [null],
      paymentMethodId: [null, Validators.required],
      accountId: [null, Validators.required],
      payeeName: [''],
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }

    const value = { ...this.form.value };
    if (value.accountId) {
      value.lines = [
        { accountId: value.accountId, amount: value.amount, description: value.description }
      ];
    } else {
      value.lines = [];
    }

    const request = value.id
      ? this.service.update(value.id, value)
      : this.service.create(value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.form.reset();
      this.list.get();
    });
  }

  print(id: string) {
    this.restService.request<any, Blob>({
      method: 'GET',
      url: `/api/app/payment-voucher/pdf/${id}`,
      responseType: 'blob'
    }).subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      window.open(url, '_blank');
    });
  }
}
