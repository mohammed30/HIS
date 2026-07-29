import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule, PagedResultDto } from '@abp/ng.core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { InsuranceReportService } from '../../proxy/insurance/insurance-report.service';
import { InsuranceCompanyService } from '../../proxy/insurance/insurance-company.service';
import { InsurancePlanService } from '../../proxy/insurance/insurance-plan.service';
import { InsuranceSummaryDto, InsuranceDetailedClaimDto, GetInsuranceReportInput, LookupDto } from '../../proxy/insurance/models';
import { jsPDF } from 'jspdf';
import autoTable from 'jspdf-autotable';

@Component({
  selector: 'app-insurance-reports',
  standalone: true,
  imports: [CommonModule, FormsModule, ThemeSharedModule, CoreModule, NgbModule],
  templateUrl: './insurance-reports.component.html'
})
export class InsuranceReportsComponent implements OnInit {
  private reportService = inject(InsuranceReportService);
  private companyService = inject(InsuranceCompanyService);
  private planService = inject(InsurancePlanService);

  activeTab = 'summary';
  filters: GetInsuranceReportInput = {
    maxResultCount: 10,
    skipCount: 0,
    fromDate: new Date().toISOString().split('T')[0],
    toDate: new Date().toISOString().split('T')[0],
    insuranceCompanyId: null,
    insurancePlanId: null
  };

  summaryData: InsuranceSummaryDto[] = [];
  detailedData: PagedResultDto<InsuranceDetailedClaimDto> = { items: [], totalCount: 0 };
  companies: LookupDto[] = [];
  plans: LookupDto[] = [];
  page = 1;

  ngOnInit(): void {
    this.loadCompanies();
    this.refresh();
  }

  loadCompanies() {
    this.companyService.getLookup().subscribe(res => {
      this.companies = res;
    });
  }

  onCompanyChange() {
    this.filters.insurancePlanId = null;
    this.plans = [];
    if (this.filters.insuranceCompanyId) {
      this.planService.getLookup(this.filters.insuranceCompanyId).subscribe(res => {
        this.plans = res;
      });
    }
    this.refresh();
  }

  refresh() {
    if (this.activeTab === 'summary') {
      this.loadSummary();
    } else {
      this.loadDetailed();
    }
  }

  loadSummary() {
    this.reportService.getSummaryReport(this.filters).subscribe(res => {
      this.summaryData = res;
    });
  }

  loadDetailed() {
    this.filters.skipCount = (this.page - 1) * this.filters.maxResultCount;
    this.reportService.getDetailedClaimsReport(this.filters).subscribe(res => {
      this.detailedData = res;
    });
  }

  onTabChange(tabId: string) {
    this.activeTab = tabId;
    this.page = 1;
    this.refresh();
  }

  exportToPdf() {
    const doc = new jsPDF('l', 'mm', 'a4');
    
    // Add font for Arabic support (using a basic approach, ideally use a custom font)
    // doc.addFont('Amiri-Regular.ttf', 'Amiri', 'normal');
    // doc.setFont('Amiri');
    
    doc.text('Insurance Report', 14, 15);
    
    if (this.activeTab === 'summary') {
      const headers = [['Insurance Company', 'Invoice Count', 'Total Billed', 'Insurance Share', 'Patient Share']];
      const data = this.summaryData.map(item => [
        item.insuranceCompanyName,
        item.invoiceCount,
        item.totalBilled,
        item.totalInsuranceShare,
        item.totalPatientShare
      ]);
      
      autoTable(doc, {
        head: headers,
        body: data,
        startY: 25,
      });
    } else {
      const headers = [['Date', 'Invoice #', 'Patient', 'Plan', 'Total', 'Insurance', 'Patient', 'Status']];
      const data = this.detailedData.items.map(item => [
        item.invoiceDate ? item.invoiceDate.split('T')[0] : '',
        item.invoiceNumber,
        item.patientName,
        item.insurancePlanName,
        item.totalAmount,
        item.insuranceShare,
        item.patientShare,
        item.status
      ]);
      
      autoTable(doc, {
        head: headers,
        body: data,
        startY: 25,
      });
    }
    
    doc.save(`insurance_report_${new Date().getTime()}.pdf`);
  }
}
