import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EmployeeDto } from '../../proxy/hr/models';
import { HRService } from '../../proxy/hr/hr.service';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { JobTitleService } from '../../proxy/settings/job-title.service';
import { JobTitleDto } from '../../proxy/settings/models';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, ReactiveFormsModule, CommonModule, NgbDropdownModule],
  templateUrl: './employees.html',
  styleUrls: ['./employees.scss'],
  providers: [ListService],
})
export class Employees implements OnInit {
  employeeList: PagedResultDto<EmployeeDto> = { items: [], totalCount: 0 };
  jobTitles: JobTitleDto[] = [];
  selectedEmployee = {} as EmployeeDto;
  isModalOpen = false;
  form: FormGroup;

  constructor(
    public readonly list: ListService,
    private hrService: HRService,
    private jobTitleService: JobTitleService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    const streamCreator = (query) => this.hrService.getEmployees(query);
    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.employeeList = response;
    });

    this.jobTitleService.getList({ maxResultCount: 1000 } as any).subscribe((result) => {
      this.jobTitles = result.items;
    });
  }

  createEmployee() {
    this.selectedEmployee = {} as EmployeeDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  editEmployee(id: string) {
    this.hrService.getEmployee(id).subscribe((employee) => {
      this.selectedEmployee = employee;
      this.buildForm();
      this.isModalOpen = true;
    });
  }

  buildForm() {
    this.form = this.fb.group({
      employeeNumber: [this.selectedEmployee.employeeNumber || '', Validators.required],
      nameAr: [this.selectedEmployee.nameAr || '', Validators.required],
      nameEn: [this.selectedEmployee.nameEn || '', Validators.required],
      phone: [this.selectedEmployee.phone || ''],
      jobTitleId: [this.selectedEmployee.jobTitleId || null],
      jobTitle: [this.selectedEmployee.jobTitle || ''],
      hireDate: [
        this.selectedEmployee.hireDate ? new Date(this.selectedEmployee.hireDate).toISOString().split('T')[0] : null,
      ],
      isActive: [this.selectedEmployee.isActive ?? true],
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }

    const request = this.selectedEmployee.id
      ? this.hrService.updateEmployee(this.selectedEmployee.id, this.form.value)
      : this.hrService.createEmployee(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.form.reset();
      this.list.get();
    });
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.hrService.deleteEmployee(id).subscribe(() => this.list.get());
      }
    });
  }
}
