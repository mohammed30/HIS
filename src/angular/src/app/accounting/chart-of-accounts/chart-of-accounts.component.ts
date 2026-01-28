import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { AccountService } from '../../proxy/accounting/account.service';
import { AccountDto, AccountType, CreateUpdateAccountDto } from '../../proxy/accounting/models';

@Component({
    selector: 'app-chart-of-accounts',
    templateUrl: './chart-of-accounts.component.html',
    providers: [ListService],
    standalone: true,
    imports: [CommonModule, ThemeSharedModule, CoreModule, ReactiveFormsModule, NgxDatatableModule, NgbDropdownModule]
})
export class ChartOfAccountsComponent implements OnInit {
    data: PagedResultDto<AccountDto> = { items: [], totalCount: 0 };
    isModalOpen = false;
    form: FormGroup;
    selectedAccount: AccountDto = {} as AccountDto;

    accountTypes = [
        { value: 0, key: 'Asset' },
        { value: 1, key: 'Liability' },
        { value: 2, key: 'Equity' },
        { value: 3, key: 'Revenue' },
        { value: 4, key: 'Expense' },
    ];

    constructor(
        public readonly list: ListService,
        private accountService: AccountService,
        private fb: FormBuilder,
        private confirmation: ConfirmationService
    ) { }

    ngOnInit() {
        const streamCreator = (query) => this.accountService.getList(query);

        this.list.hookToQuery(streamCreator).subscribe((response) => {
            this.data = response;
        });
    }

    createAccount() {
        this.selectedAccount = {} as AccountDto;
        this.buildForm();
        this.isModalOpen = true;
    }

    editAccount(id: string) {
        this.accountService.get(id).subscribe((account) => {
            this.selectedAccount = account;
            this.buildForm();
            this.isModalOpen = true;
        });
    }

    buildForm() {
        this.form = this.fb.group({
            code: [this.selectedAccount.code || '', [Validators.required, Validators.maxLength(32)]],
            name: [this.selectedAccount.name || '', [Validators.required, Validators.maxLength(128)]],
            type: [this.selectedAccount.type !== undefined ? this.selectedAccount.type : null, [Validators.required]],
            parentId: [this.selectedAccount.parentId || null],
        });
    }

    save() {
        if (this.form.invalid) {
            return;
        }

        const request = this.selectedAccount.id
            ? this.accountService.update(this.selectedAccount.id, this.form.value)
            : this.accountService.create(this.form.value);

        request.subscribe(() => {
            this.isModalOpen = false;
            this.form.reset();
            this.list.get();
        });
    }

    delete(id: string) {
        this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.accountService.delete(id).subscribe(() => this.list.get());
            }
        });
    }

    getAccountType(value: number) {
        return this.accountTypes.find(x => x.value === value)?.key || value;
    }
}
