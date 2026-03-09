import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SalarySetupDto, PayrollRunDto, EmployeeLookupDto, CompensationItemDto, JobGradeDto } from '../../proxy/hr/models';
import { HRService } from '../../proxy/hr/hr.service';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule, NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { payrollRunStatusOptions } from '../../proxy/hr/enums/payroll-run-status.enum';

@Component({
  selector: 'app-payroll',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, ReactiveFormsModule, CommonModule, NgbDropdownModule, NgbNavModule],
  templateUrl: './payroll.html',
  styleUrls: ['./payroll.scss'],
  providers: [ListService],
})
export class Payroll implements OnInit {
  // Salary Setup
  setups: PagedResultDto<SalarySetupDto> = { items: [], totalCount: 0 };
  selectedSetup = {} as SalarySetupDto;
  isSetupModalOpen = false;
  setupForm: FormGroup;

  // Payroll Runs
  runs: PagedResultDto<PayrollRunDto> = { items: [], totalCount: 0 };
  isProcessModalOpen = false;
  processForm: FormGroup;

  // Pay Slip Modal
  isPaySlipModalOpen = false;
  runEmployees: EmployeeLookupDto[] = [];
  selectedRunId = '';

  // Lookups
  employeeLookup: EmployeeLookupDto[] = [];
  compensationItems: CompensationItemDto[] = [];
  jobGrades: JobGradeDto[] = [];
  departments: any[] = []; // Reusing existing departments
  statusOptions = payrollRunStatusOptions;

  activeTab = 1; // 1 for processing, 2 for setup

  constructor(
    public readonly list: ListService,
    private hrService: HRService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    this.loadLookups();

    const streamCreator = (query) => {
      if (this.activeTab === 1) {
        return this.hrService.getPayrollRuns(query);
      } else {
        return this.hrService.getSalarySetups(query);
      }
    };

    this.list.hookToQuery(streamCreator).subscribe((response) => {
      if (this.activeTab === 1) {
        this.runs = response as PagedResultDto<PayrollRunDto>;
      } else {
        this.setups = response as PagedResultDto<SalarySetupDto>;
      }
    });
  }

  refreshList() {
    this.list.get();
  }

  loadLookups() {
    this.hrService.getEmployeeLookup().subscribe((res) => this.employeeLookup = res);
    this.hrService.getCompensationItems().subscribe((res) => this.compensationItems = res);
    this.hrService.getJobGrades().subscribe((res) => this.jobGrades = res);
    // Note: Department list would normally come from SettingsService, 
    // but we can often just search or assume it's loaded elsewhere.
  }

  // ===== Salary Setup Methods =====

  createSetup() {
    this.selectedSetup = {} as SalarySetupDto;
    this.buildSetupForm();
    this.isSetupModalOpen = true;
  }

  editSetup(id: string) {
    const setup = this.setups.items.find((x) => x.id === id);
    if (setup) {
      this.selectedSetup = { ...setup };
      this.buildSetupForm();
      this.isSetupModalOpen = true;
    }
  }

  buildSetupForm() {
    this.setupForm = this.fb.group({
      employeeId: [this.selectedSetup.employeeId || null, Validators.required],
      compensationItemId: [this.selectedSetup.compensationItemId || null, Validators.required],
      amount: [this.selectedSetup.amount || 0, [Validators.required, Validators.min(0)]],
      isRecurring: [this.selectedSetup.isRecurring ?? true],
      startDate: [
        this.selectedSetup.startDate ? new Date(this.selectedSetup.startDate).toISOString().split('T')[0] : null,
        Validators.required
      ],
      isActive: [this.selectedSetup.isActive ?? true],
    });
  }

  saveSetup() {
    if (this.setupForm.invalid) return;

    const request = this.selectedSetup.id
      ? this.hrService.updateSalarySetup(this.selectedSetup.id, this.setupForm.value)
      : this.hrService.createSalarySetup(this.setupForm.value);

    request.subscribe(() => {
      this.isSetupModalOpen = false;
      this.refreshList();
    });
  }

  deleteSetup(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.hrService.deleteSalarySetup(id).subscribe(() => this.refreshList());
      }
    });
  }

  // ===== Payroll Process Methods =====

  createProcess() {
    this.buildProcessForm();
    this.isProcessModalOpen = true;
  }

  buildProcessForm() {
    this.processForm = this.fb.group({
      periodStart: [null, Validators.required],
      periodEnd: [null, Validators.required],
      departmentId: [null],
      jobGradeId: [null],
    });
  }

  runProcess() {
    if (this.processForm.invalid) return;

    this.hrService.processPayroll(this.processForm.value).subscribe(() => {
      this.isProcessModalOpen = false;
      this.refreshList();
    });
  }

  onTabChange() {
    this.refreshList();
  }

  viewPaySlip(runId: string) {
    this.selectedRunId = runId;
    this.hrService.getPayrollRunEmployees(runId).subscribe((res) => {
      this.runEmployees = res;
      this.isPaySlipModalOpen = true;
    });
  }

  printPaySlip(employeeId: string) {
    this.hrService.downloadPaySlipPdf(this.selectedRunId, employeeId).subscribe((blob: Blob) => {
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
