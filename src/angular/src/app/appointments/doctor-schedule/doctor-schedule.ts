import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
// For now, assuming standard CRUD for Doctor Schedules or a specialized service
// Since we didn't strictly scaffold a proxy for DoctorSchedule, we might need one or reuse AppointmentService logic if added
// Let's assume a basic placeholder implementation for now or use ListService if a proxy exists.
// Checking proxys, we have doctor-schedule.service.ts
import { DoctorScheduleService } from '../../proxy/appointments/doctor-schedule.service';
import { DoctorScheduleDto } from '../../proxy/appointments/models';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-doctor-schedule',
  templateUrl: './doctor-schedule.html',
  styleUrls: ['./doctor-schedule.scss'],
  providers: [ListService],
})
export class DoctorScheduleComponent implements OnInit {
  items: DoctorScheduleDto[] = [];
  totalCount = 0;

  isModalOpen = false;
  form: FormGroup;
  selectedItem: DoctorScheduleDto = {} as DoctorScheduleDto;

  daysOfWeek = [
    { text: 'Sunday', value: 0 },
    { text: 'Monday', value: 1 },
    { text: 'Tuesday', value: 2 },
    { text: 'Wednesday', value: 3 },
    { text: 'Thursday', value: 4 },
    { text: 'Friday', value: 5 },
    { text: 'Saturday', value: 6 },
  ];

  constructor(
    public readonly list: ListService,
    private scheduleService: DoctorScheduleService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    this.list.hookToQuery(query => this.scheduleService.getList(query)).subscribe(res => {
      this.items = res.items;
      this.totalCount = res.totalCount;
    });
  }

  create() {
    this.selectedItem = {} as DoctorScheduleDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  edit(id: string) {
    const item = this.items.find(x => x.id === id);
    this.selectedItem = item || {} as DoctorScheduleDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  buildForm() {
    this.form = this.fb.group({
      doctorId: [this.selectedItem.doctorId || '', [Validators.required]],
      dayOfWeek: [this.selectedItem.dayOfWeek || 0, [Validators.required]],
      startTime: [this.selectedItem.startTime || '09:00:00', [Validators.required]],
      endTime: [this.selectedItem.endTime || '17:00:00', [Validators.required]],
      slotDuration: [this.selectedItem.slotDuration || 15, [Validators.required, Validators.min(5)]],
      isActive: [this.selectedItem.isActive !== false]
    });
  }

  save() {
    if (this.form.invalid) return;

    // Convert string time to TimeSpan string if needed or ensure format
    // Backend expects "hh:mm:ss" usually

    const request = this.selectedItem.id
      ? this.scheduleService.update(this.selectedItem.id, this.form.value)
      : this.scheduleService.create(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.list.get();
    });
  }

  delete(id: string) {
    this.confirmation.warn('Delete this schedule?', 'Confirm').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.scheduleService.delete(id).subscribe(() => this.list.get());
      }
    });
  }
}
