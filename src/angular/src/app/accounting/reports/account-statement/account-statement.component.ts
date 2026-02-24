import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RestService } from '@abp/ng.core';
import { NgbDateNativeAdapter, NgbDateAdapter, NgbDatepickerModule, NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { LocalizationModule } from '@abp/ng.core';
import { FormsModule } from '@angular/forms';

interface AccountLookup {
    id: string;
    code: string;
    name: string;
    nameAr: string;
    parentId: string | null;
}

interface StatementLine {
    date: string;
    referenceNumber: string;
    description: string;
    debit: number;
    credit: number;
    runningBalance: number;
}

interface AccountStatement {
    accountCode: string;
    accountName: string;
    openingBalance: number;
    totalDebit: number;
    totalCredit: number;
    closingBalance: number;
    lines: StatementLine[];
}

interface AccountSummary {
    accountId: string;
    accountCode: string;
    accountName: string;
    accountType: number;
    isParent: boolean;
    totalDebit: number;
    totalCredit: number;
    balance: number;
    children: AccountSummary[];
    expanded?: boolean;
}

@Component({
    selector: 'app-account-statement',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule, NgbDatepickerModule, NgbNavModule, LocalizationModule],
    providers: [
        { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }
    ],
    templateUrl: './account-statement.component.html',
    styleUrls: ['./account-statement.component.scss']
})
export class AccountStatementComponent implements OnInit {
    form: FormGroup;
    activeTab = 1;
    isLoading = false;

    accounts: AccountLookup[] = [];
    leafAccounts: AccountLookup[] = [];
    selectedAccountId: string = '';

    statement: AccountStatement | null = null;
    summary: AccountSummary[] = [];

    private fb = inject(FormBuilder);
    private restService = inject(RestService);

    ngOnInit() {
        this.buildForm();
        this.loadAccounts();
    }

    buildForm() {
        const today = new Date();
        const firstDay = new Date(today.getFullYear(), 0, 1);

        this.form = this.fb.group({
            startDate: [firstDay, Validators.required],
            endDate: [today, Validators.required]
        });
    }

    loadAccounts() {
        this.restService.request<void, any>({
            method: 'GET',
            url: '/api/app/account',
            params: { maxResultCount: '1000' }
        }).subscribe({
            next: (res) => {
                this.accounts = res.items || res;
                // Leaf accounts = accounts that are NOT parents of any other account
                const parentIds = new Set(this.accounts.filter(a => a.parentId).map(a => a.parentId));
                this.leafAccounts = this.accounts.filter(a => !parentIds.has(a.id));
            }
        });
    }

    generate() {
        if (this.form.invalid) return;

        this.isLoading = true;
        const { startDate, endDate } = this.form.value;

        if (this.activeTab === 1) {
            this.generateDetailedStatement(startDate, endDate);
        } else {
            this.generateSummary(startDate, endDate);
        }
    }

    private generateDetailedStatement(startDate: Date, endDate: Date) {
        if (!this.selectedAccountId) {
            this.isLoading = false;
            return;
        }

        this.restService.request<void, AccountStatement>({
            method: 'GET',
            url: '/api/app/account/account-statement',
            params: {
                accountId: this.selectedAccountId,
                startDate: startDate.toISOString(),
                endDate: endDate.toISOString()
            }
        }).subscribe({
            next: (res) => {
                this.statement = res;
                this.isLoading = false;
            },
            error: () => this.isLoading = false
        });
    }

    private generateSummary(startDate: Date, endDate: Date) {
        this.restService.request<void, AccountSummary[]>({
            method: 'GET',
            url: '/api/app/account/account-summary',
            params: {
                startDate: startDate.toISOString(),
                endDate: endDate.toISOString()
            }
        }).subscribe({
            next: (res) => {
                this.summary = res;
                this.isLoading = false;
            },
            error: () => this.isLoading = false
        });
    }

    toggleExpand(item: AccountSummary) {
        item.expanded = !item.expanded;
    }
}
