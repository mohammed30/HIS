import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit, inject } from '@angular/core';
import { ReceiptVoucherService } from '../../../proxy/accounting/receipt-voucher.service';
import { ReceiptVoucherDto } from '../../../proxy/accounting/dtos/models';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import { NgbDateNativeAdapter, NgbDateAdapter, NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-receipt-vouchers',
  standalone: true,
  imports: [
    ThemeSharedModule,
    CoreModule,
    NgbDatepickerModule,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './receipt-vouchers.html',
  styleUrl: './receipt-vouchers.scss',
  providers: [ListService, { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
})
export class ReceiptVouchers implements OnInit {
  items: ReceiptVoucherDto[] = [];
  form: FormGroup;
  isModalOpen = false;

  private readonly list = inject(ListService);
  private readonly service = inject(ReceiptVoucherService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationService);

  ngOnInit() {
    const stream = (query: any) => this.service.getList(query);

    this.list.hookToQuery(stream).subscribe((response: PagedResultDto<ReceiptVoucherDto>) => {
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
      patientId: [null],
      paymentMethodId: [null],
      payerName: [''],
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
