import { CoreModule } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HRService } from '../../proxy/hr/hr.service';
import { PaySlipDto, EmployeeLookupDto, PayrollRunDto } from '../../proxy/hr/models';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbNavModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, ReactiveFormsModule, CommonModule, NgbNavModule],
  templateUrl: './reports.html',
  styleUrls: ['./reports.scss'],
})
export class Reports implements OnInit {
  paySlip: PaySlipDto = null;
  searchForm: FormGroup;

  employeeLookup: EmployeeLookupDto[] = [];
  payrollRuns: PayrollRunDto[] = [];

  activeTab = 1;

  constructor(
    private hrService: HRService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.loadLookups();
    this.buildSearchForm();
  }

  loadLookups() {
    this.hrService.getEmployeeLookup().subscribe((res) => this.employeeLookup = res);
    this.hrService.getPayrollRuns({ maxResultCount: 100 }).subscribe((res) => this.payrollRuns = res.items);
  }

  buildSearchForm() {
    this.searchForm = this.fb.group({
      payrollRunId: [null, Validators.required],
      employeeId: [null, Validators.required],
    });
  }

  viewPaySlip() {
    if (this.searchForm.invalid) return;

    const { payrollRunId, employeeId } = this.searchForm.value;
    this.hrService.getPaySlip(payrollRunId, employeeId).subscribe((res) => {
      this.paySlip = res;
    });
  }

  printPaySlip() {
    if (this.searchForm.invalid) return;

    const { payrollRunId, employeeId } = this.searchForm.value;
    this.hrService.getPaySlipPdf(payrollRunId, employeeId).subscribe((blob: Blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `PaySlip_${employeeId}.pdf`;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    });
  }
}
