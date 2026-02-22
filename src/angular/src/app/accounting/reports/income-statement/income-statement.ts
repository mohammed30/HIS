import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule } from '@angular/forms';
import * as xlsx from 'xlsx';

@Component({
    selector: 'app-income-statement',
    standalone: true,
    imports: [CommonModule, CoreModule, ThemeSharedModule, NgbDatepickerModule, FormsModule],
    templateUrl: './income-statement.html',
    styleUrl: './income-statement.scss'
})
export class IncomeStatementComponent implements OnInit {
    startDate = new Date(new Date().getFullYear(), 0, 1).toISOString().split('T')[0];
    endDate = new Date().toISOString().split('T')[0];
    loading = false;

    // Mock data matching the PRD categories
    revenue = {
        grossSales: 1500000,
        salesReturns: 25000,
        netSales: 1475000
    };

    cogs = {
        beginningInv: 200000,
        netPurchases: 800000,
        goodsAvailable: 1000000,
        endingInv: 150000,
        totalCogs: 850000
    };

    profitability = {
        grossProfit: 625000,
        sellingExpenses: 120000,
        adminExpenses: 180000,
        operatingIncome: 325000
    };

    otherItems = {
        otherRevenue: 15000,
        otherExpenses: 5000,
        incomeBeforeTax: 335000,
        incomeTax: 50250,
        netIncome: 284750
    };

    comprehensiveIncome = {
        foreignExchange: 2000,
        hedgingFairValue: -1500,
        actuarialLosses: -500,
        reclassifiedHedging: 0,
        totalOCI: 0,
        totalComprehensiveIncome: 284750
    };

    constructor() {
        this.calculateTotals();
    }

    ngOnInit() {
        this.fetchReport();
    }

    calculateTotals() {
        this.comprehensiveIncome.totalOCI =
            this.comprehensiveIncome.foreignExchange +
            this.comprehensiveIncome.hedgingFairValue +
            this.comprehensiveIncome.actuarialLosses +
            this.comprehensiveIncome.reclassifiedHedging;

        this.comprehensiveIncome.totalComprehensiveIncome =
            this.otherItems.netIncome + this.comprehensiveIncome.totalOCI;
    }

    fetchReport() {
        this.loading = true;
        // Simulate API call
        setTimeout(() => {
            this.loading = false;
        }, 500);
    }

    print() {
        window.print();
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
