import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { FormsModule } from '@angular/forms';
import * as xlsx from 'xlsx';
import { AccountService } from '../../../proxy/accounting/account.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Component({
    selector: 'app-income-statement',
    standalone: true,
    imports: [CommonModule, CoreModule, ThemeSharedModule, FormsModule],
    templateUrl: './income-statement.html',
    styleUrl: './income-statement.scss'
})
export class IncomeStatementComponent implements OnInit {
    private accountService = inject(AccountService);
    private http = inject(HttpClient);

    startDate = new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0];
    endDate = new Date().toISOString().split('T')[0];
    loading = false;
    searchText = '';

    // Data from API
    revenueLines: any[] = [];
    costOfSalesLines: any[] = [];
    gaExpenseLines: any[] = [];
    otherRevenueLines: any[] = [];
    otherExpenseLines: any[] = [];

    // Totals from API
    totalRevenue = 0;
    totalCostOfSales = 0;
    totalGaExpenses = 0;
    totalOtherRevenue = 0;
    totalOtherExpenses = 0;

    // Calculated
    get grossProfit() { return this.totalRevenue - this.totalCostOfSales; }
    get operatingIncome() { return this.grossProfit - this.totalGaExpenses; }
    get netIncome() { return this.operatingIncome + this.totalOtherRevenue - this.totalOtherExpenses; }

    // Filtered lines for search
    get filteredRevenueLines() { return this.filterLines(this.revenueLines); }
    get filteredCostLines() { return this.filterLines(this.costOfSalesLines); }
    get filteredGaLines() { return this.filterLines(this.gaExpenseLines); }
    get filteredOtherRevenueLines() { return this.filterLines(this.otherRevenueLines); }
    get filteredOtherExpenseLines() { return this.filterLines(this.otherExpenseLines); }

    filterLines(lines: any[]): any[] {
        if (!this.searchText.trim()) return lines;
        const term = this.searchText.toLowerCase();
        return lines.filter(l =>
            (l.accountName || '').toLowerCase().includes(term) ||
            (l.accountCode || '').toLowerCase().includes(term)
        );
    }

    get hasSearchFilter(): boolean {
        return !!this.searchText.trim();
    }

    ngOnInit() {
        this.fetchReport();
    }

    fetchReport() {
        this.loading = true;
        this.accountService.getIncomeStatement({
            startDate: this.startDate,
            endDate: this.endDate
        }).subscribe({
            next: (data: any) => {
                this.revenueLines = data.revenueLines || [];
                this.costOfSalesLines = data.costOfSalesLines || [];
                this.gaExpenseLines = data.generalAndAdminExpenseLines || [];
                this.otherRevenueLines = data.otherRevenueLines || [];
                this.otherExpenseLines = data.otherExpenseLines || [];

                this.totalRevenue = data.totalRevenue || 0;
                this.totalCostOfSales = data.totalCostOfSales || 0;
                this.totalGaExpenses = data.totalGeneralAndAdminExpenses || 0;
                this.totalOtherRevenue = data.totalOtherRevenues || 0;
                this.totalOtherExpenses = data.totalOtherExpenses || 0;

                this.loading = false;
            },
            error: (err) => {
                console.error('Error loading income statement:', err);
                this.loading = false;
            }
        });
    }

    print() {
        const apiUrl = `${environment.apis.default.url}/api/app/account/income-statement-pdf?startDate=${this.startDate}&endDate=${this.endDate}`;
        this.http.get(apiUrl, { responseType: 'blob' }).subscribe({
            next: (blob) => {
                const fileURL = URL.createObjectURL(blob);
                window.open(fileURL, '_blank');
            },
            error: (err) => {
                console.error('Error generating PDF:', err);
                alert('حدث خطأ أثناء توليد ملف الطباعة');
            }
        });
    }

    exportToExcel() {
        const tableElement = document.querySelector('.income-statement-table');
        if (tableElement) {
            const ws: xlsx.WorkSheet = xlsx.utils.table_to_sheet(tableElement);
            const wb: xlsx.WorkBook = xlsx.utils.book_new();
            xlsx.utils.book_append_sheet(wb, ws, 'Income Statement');
            xlsx.writeFile(wb, `Income_Statement_${this.startDate}_to_${this.endDate}.xlsx`);
        }
    }
}
