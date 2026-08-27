import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { InsuranceClaimReportService } from '@proxy/reports';
import { InsuranceClaimReportDto, GetInsuranceClaimsInput } from '@proxy/reports/models';
import { PagedResultDto, ListService, CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { NgbPaginationModule, NgbAccordionModule } from '@ng-bootstrap/ng-bootstrap';
import { InsuranceCompanyService } from '@proxy/insurance';
import { InsuranceCompanyDto } from '@proxy/insurance/models';

interface CompanyGroup {
  companyName: string;
  totalAmount: number;
  totalCoPay: number;
  totalInsurance: number;
  claims: InsuranceClaimReportDto[];
}

@Component({
  selector: 'app-insurance-claims-report',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, ReactiveFormsModule, CommonModule, NgbPaginationModule, NgbAccordionModule],
  providers: [ListService],
  templateUrl: './insurance-claims-report.component.html',
  styleUrls: ['./insurance-claims-report.component.scss']
})
export class InsuranceClaimsReportComponent implements OnInit {
  items: InsuranceClaimReportDto[] = [];
  companyGroups: CompanyGroup[] = [];
  
  // Overall Totals
  grandTotalAmount = 0;
  grandTotalCoPay = 0;
  grandTotalInsurance = 0;
  totalClaimsCount = 0;

  companies: InsuranceCompanyDto[] = [];
  form: FormGroup;
  isDownloading = false;

  private fb = inject(FormBuilder);
  private reportService = inject(InsuranceClaimReportService);
  private companyService = inject(InsuranceCompanyService);
  public list = inject(ListService);

  ngOnInit() {
    this.form = this.fb.group({
      startDate: [null],
      endDate: [null],
      insuranceCompanyId: [null]
    });

    this.companyService.getList({ maxResultCount: 1000 }).subscribe(res => {
      this.companies = res.items;
    });

    const streamCreator = (query: GetInsuranceClaimsInput) => {
      const filters = this.form.value;
      return this.reportService.getList({ ...query, ...filters });
    };

    this.list.hookToQuery(streamCreator).subscribe((response: PagedResultDto<InsuranceClaimReportDto>) => {
      this.items = response.items;
      this.processData();
    });
  }

  processData() {
    this.companyGroups = [];
    this.grandTotalAmount = 0;
    this.grandTotalCoPay = 0;
    this.grandTotalInsurance = 0;
    this.totalClaimsCount = this.items.length;

    const groupMap = new Map<string, CompanyGroup>();

    this.items.forEach(claim => {
      const cName = claim.insuranceCompanyName || 'غير محدد';
      if (!groupMap.has(cName)) {
        groupMap.set(cName, {
          companyName: cName,
          totalAmount: 0,
          totalCoPay: 0,
          totalInsurance: 0,
          claims: []
        });
      }

      const group = groupMap.get(cName)!;
      group.claims.push(claim);
      group.totalAmount += claim.totalInvoiceAmount || 0;
      group.totalCoPay += claim.totalPatientAmount || 0;
      group.totalInsurance += claim.totalInsuranceAmount || 0;

      this.grandTotalAmount += claim.totalInvoiceAmount || 0;
      this.grandTotalCoPay += claim.totalPatientAmount || 0;
      this.grandTotalInsurance += claim.totalInsuranceAmount || 0;
    });

    this.companyGroups = Array.from(groupMap.values());
  }

  search() {
    this.list.get();
  }

  printReport() {
    this.isDownloading = true;
    const filters = this.form.value;
    const input: GetInsuranceClaimsInput = {
      maxResultCount: 1000,
      skipCount: 0,
      ...filters
    };

    this.reportService.getPrintDocument(input).subscribe({
      next: (response: any) => {
        let blob: Blob;
        
        if (typeof response === 'string') {
          // Backend returned Base64 string
          const byteString = atob(response);
          const arrayBuffer = new ArrayBuffer(byteString.length);
          const int8Array = new Uint8Array(arrayBuffer);
          for (let i = 0; i < byteString.length; i++) {
            int8Array[i] = byteString.charCodeAt(i);
          }
          blob = new Blob([int8Array], { type: 'application/pdf' });
        } else {
          // Backend returned array of numbers
          const uint8Array = new Uint8Array(response);
          blob = new Blob([uint8Array], { type: 'application/pdf' });
        }
        
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Insurance_Claims_${new Date().toISOString().split('T')[0]}.pdf`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        
        this.isDownloading = false;
      },
      error: () => {
        this.isDownloading = false;
      }
    });
  }
}
