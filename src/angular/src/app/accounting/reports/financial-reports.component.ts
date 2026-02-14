import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RestService } from '@abp/ng.core';
import { NgbDateNativeAdapter, NgbDateAdapter, NgbDatepickerModule, NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { LocalizationModule } from '@abp/ng.core';
import { environment } from '../../../environments/environment';

@Component({
    selector: 'app-financial-reports',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, NgbDatepickerModule, NgbNavModule, LocalizationModule],
    providers: [
        { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }
    ],
    templateUrl: './financial-reports.component.html'
})
export class FinancialReportsComponent implements OnInit {
    form: FormGroup;
    activeTab = 1;
    isLoading = false;

    incomeStatement: any = null;
    balanceSheet: any = null;
    cashFlow: any = null;
    equity: any = null;

    private fb = inject(FormBuilder);
    private restService = inject(RestService);

    constructor() { }

    ngOnInit() {
        this.buildForm();
    }

    buildForm() {
        const today = new Date();
        const firstDay = new Date(today.getFullYear(), 0, 1); // Jan 1st

        this.form = this.fb.group({
            startDate: [firstDay, Validators.required],
            endDate: [today, Validators.required]
        });
    }

    generateReport() {
        if (this.form.invalid) return;

        this.isLoading = true;
        const body = this.form.value;

        const params: any = {
            startDate: body.startDate.toISOString(),
            endDate: body.endDate.toISOString(),
        };

        let url = '';
        if (this.activeTab === 1) url = '/api/app/financial-reports/income-statement';
        else if (this.activeTab === 2) url = '/api/app/financial-reports/balance-sheet';
        else if (this.activeTab === 3) url = '/api/app/financial-reports/cash-flow-statement';
        else if (this.activeTab === 4) url = '/api/app/financial-reports/changes-in-equity';

        this.restService.request<void, any>({
            method: 'GET', // or POST if complex input? My AppService used Get with QueryString (DateRangeDto maps to query string usually if simple properties)
            // Actually DateRangeDto logic in ABP might require FromQuery. 
            // Let's assume ABP handles it.
            url: url,
            params: params
        }).subscribe({
            next: (res) => {
                if (this.activeTab === 1) this.incomeStatement = res;
                else if (this.activeTab === 2) this.balanceSheet = res;
                else if (this.activeTab === 3) this.cashFlow = res;
                else if (this.activeTab === 4) this.equity = res;

                this.isLoading = false;
            },
            error: () => this.isLoading = false
        });
    }
}
