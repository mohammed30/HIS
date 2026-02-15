import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit, inject } from '@angular/core';
import { ContractClaimService } from '../../../proxy/accounting/contract-claim.service';
import { ContractClaimDto } from '../../../proxy/accounting/dtos/models';
import { claimStatusOptions } from '../../../proxy/accounting/claim-status.enum';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import { NgbDateNativeAdapter, NgbDateAdapter, NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-contract-claims',
  standalone: true,
  imports: [
    ThemeSharedModule,
    CoreModule,
    NgbDatepickerModule,
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './contract-claims.html',
  styleUrl: './contract-claims.scss',
  providers: [ListService, { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
})
export class ContractClaims implements OnInit {
  items: ContractClaimDto[] = [];
  form: FormGroup;
  isModalOpen = false;
  claimStatusOptions = claimStatusOptions;

  private readonly list = inject(ListService);
  private readonly service = inject(ContractClaimService);
  private readonly fb = inject(FormBuilder);
  private readonly confirmation = inject(ConfirmationService);

  ngOnInit() {
    const stream = (query: any) => this.service.getList(query);

    this.list.hookToQuery(stream).subscribe((response: PagedResultDto<ContractClaimDto>) => {
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
      contractId: [null], // Needs lookup
      amount: [0, Validators.required],
      status: [0, Validators.required],
      remarks: [''],
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
