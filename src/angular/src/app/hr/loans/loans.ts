import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EmployeeLoanDto, EmployeeLookupDto, CompensationItemDto } from '../../proxy/hr/models';
import { HRService } from '../../proxy/hr/hr.service';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { loanStatusOptions } from '../../proxy/hr/enums/loan-status.enum';

@Component({
  selector: 'app-loans',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, ReactiveFormsModule, CommonModule, NgbDropdownModule],
  templateUrl: './loans.html',
  styleUrls: ['./loans.scss'],
  providers: [ListService],
})
export class Loans implements OnInit {
  loans: PagedResultDto<EmployeeLoanDto> = { items: [], totalCount: 0 };
  selectedLoan = {} as EmployeeLoanDto;
  isModalOpen = false;
  form: FormGroup;

  // Lookups
  employeeLookup: EmployeeLookupDto[] = [];
  deductionItems: CompensationItemDto[] = [];
  statusOptions = loanStatusOptions;

  constructor(
    public readonly list: ListService,
    private hrService: HRService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    this.loadLookups();

    const streamCreator = (query) => this.hrService.getEmployeeLoans(query);
    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.loans = response;
    });
  }

  loadLookups() {
    this.hrService.getEmployeeLookup().subscribe((result) => {
      this.employeeLookup = result;
    });

    this.hrService.getCompensationItems().subscribe((result) => {
      // Filter for items that are deductions (nature 2) or credit method? 
      // Actually, any item can be selected, but usually it's a deduction category.
      this.deductionItems = result;
    });
  }

  createLoan() {
    this.selectedLoan = {} as EmployeeLoanDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  buildForm() {
    this.form = this.fb.group({
      employeeId: [this.selectedLoan.employeeId || null, Validators.required],
      compensationItemId: [this.selectedLoan.compensationItemId || null, Validators.required],
      amount: [this.selectedLoan.amount || 0, [Validators.required, Validators.min(1)]],
      installments: [this.selectedLoan.installments || 1, [Validators.required, Validators.min(1)]],
      startDate: [
        this.selectedLoan.startDate ? new Date(this.selectedLoan.startDate).toISOString().split('T')[0] : null,
        Validators.required
      ],
      notes: [this.selectedLoan.notes || ''],
    });
  }

  save() {
    if (this.form.invalid) return;

    this.hrService.createEmployeeLoan(this.form.value).subscribe(() => {
      this.isModalOpen = false;
      this.list.get();
    });
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.hrService.deleteEmployeeLoan(id).subscribe(() => this.list.get());
      }
    });
  }
}
