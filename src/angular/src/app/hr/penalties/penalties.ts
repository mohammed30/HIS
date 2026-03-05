import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PenaltyDto, EmployeeLookupDto } from '../../proxy/hr/models';
import { HRService } from '../../proxy/hr/hr.service';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { penaltyTypeOptions } from '../../proxy/hr/enums/penalty-type.enum';

@Component({
  selector: 'app-penalties',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, ReactiveFormsModule, CommonModule, NgbDropdownModule],
  templateUrl: './penalties.html',
  styleUrls: ['./penalties.scss'],
  providers: [ListService],
})
export class Penalties implements OnInit {
  penalties: PagedResultDto<PenaltyDto> = { items: [], totalCount: 0 };
  selectedPenalty = {} as PenaltyDto;
  isModalOpen = false;
  form: FormGroup;

  // Lookups
  employeeLookup: EmployeeLookupDto[] = [];
  typeOptions = penaltyTypeOptions;

  constructor(
    public readonly list: ListService,
    private hrService: HRService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    this.loadLookups();

    const streamCreator = (query) => this.hrService.getPenalties(query);
    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.penalties = response;
    });
  }

  loadLookups() {
    this.hrService.getEmployeeLookup().subscribe((res) => this.employeeLookup = res);
  }

  createPenalty() {
    this.selectedPenalty = {} as PenaltyDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  buildForm() {
    this.form = this.fb.group({
      employeeId: [this.selectedPenalty.employeeId || null, Validators.required],
      penaltyType: [this.selectedPenalty.penaltyType || null, Validators.required],
      description: [this.selectedPenalty.description || ''],
      amount: [this.selectedPenalty.amount || 0],
      suspensionDays: [this.selectedPenalty.suspensionDays || 0],
      date: [
        this.selectedPenalty.date ? new Date(this.selectedPenalty.date).toISOString().split('T')[0] : new Date().toISOString().split('T')[0],
        Validators.required
      ],
      notes: [this.selectedPenalty.notes || ''],
    });
  }

  save() {
    if (this.form.invalid) return;

    this.hrService.createPenalty(this.form.value).subscribe(() => {
      this.isModalOpen = false;
      this.list.get();
    });
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.hrService.deletePenalty(id).subscribe(() => this.list.get());
      }
    });
  }
}
