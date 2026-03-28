import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule, PagedResultDto } from '@abp/ng.core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { InsuranceReportService } from '../../proxy/insurance/insurance-report.service';
import { InsuranceCompanyService } from '../../proxy/insurance/insurance-company.service';
import { InsuranceSummaryDto, InsuranceDetailedClaimDto, GetInsuranceReportInput, LookupDto } from '../../proxy/insurance/models';

@Component({
  selector: 'app-insurance-reports',
  standalone: true,
  imports: [CommonModule, FormsModule, ThemeSharedModule, CoreModule, NgbModule],
  templateUrl: './insurance-reports.component.html'
})
export class InsuranceReportsComponent implements OnInit {
  private reportService = inject(InsuranceReportService);
  private companyService = inject(InsuranceCompanyService);

  activeTab = 'summary';
  filters: GetInsuranceReportInput = {
    maxResultCount: 10,
    skipCount: 0,
    fromDate: new Date().toISOString().split('T')[0],
    toDate: new Date().toISOString().split('T')[0],
    insuranceCompanyId: null
  };

  summaryData: InsuranceSummaryDto[] = [];
  detailedData: PagedResultDto<InsuranceDetailedClaimDto> = { items: [], totalCount: 0 };
  companies: LookupDto[] = [];
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
}
