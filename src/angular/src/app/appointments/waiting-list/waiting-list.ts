import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { AppointmentService } from '../../proxy/appointments/appointment.service';
import { WaitingListDto, CreateUpdateWaitingListDto } from '../../proxy/appointments/dtos/models';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { LocalizationModule } from '@abp/ng.core';

@Component({
  selector: 'app-waiting-list',
  templateUrl: './waiting-list.html',
  styleUrls: ['./waiting-list.scss'],
  standalone: true,
  imports: [CommonModule, ThemeSharedModule, ReactiveFormsModule, FormsModule, LocalizationModule],
  providers: [ListService],
})
export class WaitingListComponent implements OnInit {
  items: WaitingListDto[] = [];
  totalCount = 0;

  isModalOpen = false;
  form: FormGroup;
  selectedItem: WaitingListDto = {} as WaitingListDto;

  // Enums for UI
  priorities = [
    { text: 'Normal', value: 0 },
    { text: 'High', value: 1 },
    { text: 'Urgent', value: 2 },
  ];

  constructor(
    public readonly list: ListService,
    private appointmentService: AppointmentService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    this.list.hookToQuery(query => this.appointmentService.getWaitingList(query)).subscribe(res => {
      this.items = res.items;
      this.totalCount = res.totalCount;
    });
  }

  create() {
    this.selectedItem = {} as WaitingListDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  edit(id: string) {
    const item = this.items.find(x => x.id === id);
    this.selectedItem = item || {} as WaitingListDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  buildForm() {
    this.form = this.fb.group({
      patientId: [this.selectedItem.patientId || '', [Validators.required]],
      departmentId: [this.selectedItem.departmentId || '', [Validators.required]], // Should be select
      doctorId: [this.selectedItem.doctorId || null],
      requestDate: [this.selectedItem.requestDate || new Date().toISOString().split('T')[0], [Validators.required]],
      priority: [this.selectedItem.priority || 0, [Validators.required]],
      notes: [this.selectedItem.notes || ''],
      isResolved: [this.selectedItem.isResolved || false]
    });
  }

  save() {
    if (this.form.invalid) return;

    const request = this.selectedItem.id
      ? this.appointmentService.updateWaitingList(this.selectedItem.id, this.form.value)
      : this.appointmentService.addToWaitingList(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.list.get();
    });
  }

  delete(id: string) {
    this.confirmation.warn('Remove from waiting list?', 'Confirm').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.appointmentService.deleteFromWaitingList(id).subscribe(() => this.list.get());
      }
    });
  }
}
