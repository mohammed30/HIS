import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import * as xlsx from 'xlsx';
import { AccountService } from '../../../proxy/accounting/account.service';
import { environment } from '../../../../environments/environment';

@Component({
    selector: 'app-balance-sheet',
    standalone: true,
    imports: [CommonModule, CoreModule, ThemeSharedModule, FormsModule],
    templateUrl: './balance-sheet.html',
    styleUrl: './balance-sheet.scss'
})
export class BalanceSheetComponent implements OnInit {
    private accountService = inject(AccountService);
    private http = inject(HttpClient);

    // Default: first day of current year → today (as-of date)
    startDate = new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0];
    endDate = new Date().toISOString().split('T')[0];

    loading = false;
    searchText = '';

    // Data from API
    assetLines: any[] = [];
    liabilityLines: any[] = [];
    equityLines: any[] = [];

    // Totals
    totalAssets = 0;
    totalLiabilities = 0;
    totalEquity = 0;
    previousYearEquity = 0;

    // Filtered lines
    get filteredAssetLines() { return this.filterLines(this.assetLines); }
    get filteredLiabilityLines() { return this.filterLines(this.liabilityLines); }
    get filteredEquityLines() { return this.filterLines(this.equityLines); }

    filterLines(lines: any[]): any[] {
        if (!this.searchText.trim()) return lines;
        const term = this.searchText.toLowerCase();
        return lines.filter(l =>
            (l.accountName || '').toLowerCase().includes(term) ||
            (l.accountCode || '').toLowerCase().includes(term)
        );
    }

    get hasSearchFilter(): boolean { return !!this.searchText.trim(); }

    get totalLiabilitiesAndEquity() { return this.totalLiabilities + this.totalEquity; }

    get isBalanced() {
        return Math.abs(this.totalAssets - this.totalLiabilitiesAndEquity) < 0.01;
    }

    ngOnInit() { this.fetchReport(); }

    fetchReport() {
        this.loading = true;
        // Use the AccountService's restService pattern, but balanceSheet is not in proxy yet
        // Make a direct HTTP call like the financial-reports component does
        const url = `${environment.apis.default.url}/api/app/account/balance-sheet?startDate=${this.startDate}&endDate=${this.endDate}`;
        this.http.get<any>(url).subscribe({
            next: (data) => {
                this.assetLines = data.assetLines || [];
                this.liabilityLines = data.liabilityLines || [];
                this.equityLines = data.equityLines || [];
                this.totalAssets = data.totalAssets || 0;
                this.totalLiabilities = data.totalLiabilities || 0;
                this.totalEquity = data.totalEquity || 0;
                this.previousYearEquity = data.previousYearEquity || 0;
                this.loading = false;
            },
            error: (err) => {
                console.error('Error loading balance sheet:', err);
                this.loading = false;
            }
        });
    }

    /** Calculates the max row count (assets vs liabilities+equity combined) */
    maxRows(): number {
        const liabEquityCount = this.liabilityLines.length + 1 + this.equityLines.length; // +1 for equity header
        return Math.max(this.assetLines.length, liabEquityCount);
    }

    /** Returns a merged item from liabilities then equity (with a header between them) */
    liabilityLine(idx: number): any {
        if (idx < this.liabilityLines.length) {
            return this.liabilityLines[idx];
        }
        const equityStart = this.liabilityLines.length;
        if (idx === equityStart) {
            return { accountName: 'حقوق الملكية / Equity', accountCode: '', amount: 0, isSectionHeader: true };
        }
        const equityIdx = idx - equityStart - 1;
        return this.equityLines[equityIdx] || null;
    }

    print() {
        const apiUrl = `${environment.apis.default.url}/api/app/account/balance-sheet-pdf?startDate=${this.startDate}&endDate=${this.endDate}`;
        this.http.get(apiUrl, { responseType: 'blob' }).subscribe({
            next: (blob) => {
                const fileURL = URL.createObjectURL(blob);
                window.open(fileURL, '_blank');
            },
            error: () => {
                // Fallback to browser print if PDF endpoint not available
                window.print();
            }
        });
    }

    exportToExcel() {
        const tableElement = document.querySelector('.balance-sheet-table');
        if (tableElement) {
            const ws: xlsx.WorkSheet = xlsx.utils.table_to_sheet(tableElement);
            const wb: xlsx.WorkBook = xlsx.utils.book_new();
            xlsx.utils.book_append_sheet(wb, ws, 'Balance Sheet');
            xlsx.writeFile(wb, `BalanceSheet_${this.endDate}.xlsx`);
        }
    }
}
