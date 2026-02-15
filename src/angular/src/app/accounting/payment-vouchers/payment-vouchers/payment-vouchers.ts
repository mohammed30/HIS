import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit, inject } from '@angular/core';
import { PaymentVoucherService } from '../../../proxy/accounting/payment-voucher.service';
import { PaymentVoucherDto } from '../../../proxy/accounting/dtos/models';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import { NgbDateNativeAdapter, NgbDateAdapter, NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-payment-vouchers',
  standalone: true,
  imports: [
    ThemeSharedModule,
    CoreModule,
    NgbDatepickerModule,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './payment-vouchers.html',
  styleUrl: './payment-vouchers.scss',
  providers: [ListService, { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
})
export class PaymentVouchers implements OnInit {
  items: PaymentVoucherDto[] = [];
  form: FormGroup;
  isModalOpen = false;

  private readonly list = inject(ListService);
  private readonly service = inject(PaymentVoucherService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationService);

  ngOnInit() {
    const stream = (query: any) => this.service.getList(query);

    this.list.hookToQuery(stream).subscribe((response: PagedResultDto<PaymentVoucherDto>) => {
      this.items = response.items;
    });
  }

  create() {
    this.buildForm();
    this.isModalOpen = true;
  }

  edit(id: string) {
    this.service.get(id).subscribe((res) => {
      this.buildForm();
      this.form.patchValue(res);
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
      amount: [0, Validators.required],
      description: ['', Validators.required],
      supplierId: [null],
      paymentMethodId: [null],
      payeeName: [''],
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
}
