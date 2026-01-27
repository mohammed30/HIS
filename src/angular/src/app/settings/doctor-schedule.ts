import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NgbDateAdapter, NgbDateNativeAdapter, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DoctorScheduleService } from '@proxy/appointments';
import { DoctorScheduleDto } from '@proxy/appointments';
import { ListService, PagedResultDto, CoreModule, LocalizationModule } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { DoctorService } from '@proxy/settings';
import { DoctorDto } from '@proxy/settings';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-doctor-schedule',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NgbModule,
    CoreModule,
    LocalizationModule,
    ThemeSharedModule,
    NgxDatatableModule
  ],
  providers: [ListService, { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
  templateUrl: './doctor-schedule.html',
  styleUrls: ['./doctor-schedule.scss']
})
export class DoctorScheduleComponent implements OnInit {
  data: PagedResultDto<DoctorScheduleDto> = { items: [], totalCount: 0 };
  form: FormGroup;
  selectedSchedule = {} as DoctorScheduleDto;
  isModalOpen = false;
  doctors: DoctorDto[] = [];

  daysOfWeek = [
    { value: 0, label: 'Sunday' },
    { value: 1, label: 'Monday' },
    { value: 2, label: 'Tuesday' },
    { value: 3, label: 'Wednesday' },
    { value: 4, label: 'Thursday' },
    { value: 5, label: 'Friday' },
    { value: 6, label: 'Saturday' }
  ];

  constructor(
    public readonly list: ListService,
    private doctorScheduleService: DoctorScheduleService,
    private doctorService: DoctorService,
    private fb: FormBuilder,
    private toaster: ToasterService,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    const scheduleStreamCreator = (query) => this.doctorScheduleService.getList(query);

    this.list.hookToQuery(scheduleStreamCreator).subscribe((response) => {
      this.data = response;
    });

    this.loadDoctors();
  }

  loadDoctors() {
    this.doctorService.getList({ maxResultCount: 100 }).subscribe(res => {
      this.doctors = res.items;
    });
  }

  isDaySelected(dayValue: number): boolean {
    const selectedDays = this.form?.get('selectedDays')?.value as number[];
    return selectedDays ? selectedDays.includes(dayValue) : false;
  }

  onDayChange(event: any, dayValue: number) {
    const isChecked = event.target.checked;
    const currentDays = this.form.get('selectedDays')?.value as number[] || [];

    if (isChecked) {
      if (!currentDays.includes(dayValue)) {
        this.form.patchValue({ selectedDays: [...currentDays, dayValue] });
      }
    } else {
      this.form.patchValue({ selectedDays: currentDays.filter(d => d !== dayValue) });
    }
    this.form.get('selectedDays')?.markAsTouched();
  }

  createSchedule() {
    this.selectedSchedule = {} as DoctorScheduleDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  editSchedule(id: string) {
    this.doctorScheduleService.get(id).subscribe((schedule) => {
      this.selectedSchedule = schedule;
      this.buildForm();
      this.isModalOpen = true;
    });
  }

  buildForm() {
    // If editing, we have a single dayOfWeek. If creating, we want to support multiple.
    // For simplicity, let's use 'selectedDays' array in form.
    // When editing, we initialize it with [dayOfWeek].

    const initialDays = this.selectedSchedule.id && this.selectedSchedule.dayOfWeek !== undefined
      ? [this.selectedSchedule.dayOfWeek]
      : [];

    this.form = this.fb.group({
      doctorId: [this.selectedSchedule.doctorId || null, Validators.required],
      selectedDays: [initialDays, Validators.required], // Changed from dayOfWeek
      startTime: [this.selectedSchedule.startTime || '', Validators.required],
      endTime: [this.selectedSchedule.endTime || '', Validators.required],
      slotDuration: [this.selectedSchedule.slotDuration || 30, [Validators.required, Validators.min(5)]],
      isActive: [this.selectedSchedule.isActive !== false]
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }

    const formValue = this.form.value;
    const days: number[] = formValue.selectedDays;

    if (!days || days.length === 0) return;

    if (this.selectedSchedule.id) {
      const singleDay = days[0];
      const updateDto = { ...formValue, dayOfWeek: singleDay };
      delete updateDto.selectedDays;

      this.doctorScheduleService.update(this.selectedSchedule.id, updateDto).subscribe(() => {
        this.finalizeSave();
      });
    } else {
      const observables = days.map(day => {
        const createDto = { ...formValue, dayOfWeek: day };
        delete createDto.selectedDays;
        return this.doctorScheduleService.create(createDto);
      });

      forkJoin(observables).subscribe({
        next: () => {
          this.finalizeSave();
        },
        error: (err) => {
          console.error(err);
          this.toaster.error('::BookingFailed'); // Or generic error
        }
      });
    }
  }

  finalizeSave() {
    this.isModalOpen = false;
    this.form.reset();
    this.list.get();
    this.toaster.success('::SuccessfullySaved');
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.doctorScheduleService.delete(id).subscribe(() => {
          this.list.get();
          this.toaster.success('::SuccessfullyDeleted');
        });
      }
    });
  }

  getDayName(val: number) {
    return this.daysOfWeek.find(d => d.value === val)?.label || val;
  }

  getDoctorName(id: string) {
    const doc = this.doctors.find(d => d.id === id);
    return doc ? doc.nameAr : id;
  }
}
