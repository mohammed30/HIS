import { Component, Input, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ReservationService, ReservationStatus, reservationStatusOptions } from '../../../proxy/inpatient';
import { RoomService, RoomDto, BedDto } from '../../../proxy/rooms';
import { PatientService, PatientDto } from '../../../proxy/patients';
import { ToasterService } from '@abp/ng.theme.shared';
import { map } from 'rxjs/operators';

import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-reservation-detail',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, ReactiveFormsModule],
  templateUrl: './reservation-detail.html'
})
export class ReservationDetailComponent implements OnInit {
  activeModal = inject(NgbActiveModal);
  fb = inject(FormBuilder);
  reservationService = inject(ReservationService);
  roomService = inject(RoomService);
  patientService = inject(PatientService);
  toaster = inject(ToasterService);

  @Input() selectedId: string;
  @Input() selectedDate: string;

  form: FormGroup;
  reservationStatuses = reservationStatusOptions;
  rooms: RoomDto[] = [];
  beds: BedDto[] = [];
  patients: PatientDto[] = [];

  ngOnInit() {
    this.buildForm();
    this.loadLookups();

    this.form.get('roomId').valueChanges.subscribe(val => {
      if (val) this.onRoomChange(val);
    });

    if (this.selectedId) {
      this.reservationService.get(this.selectedId).subscribe(res => {
        this.form.patchValue({
          ...res,
          startDate: this.formatDate(res.startDate),
          endDate: this.formatDate(res.endDate)
        });
        if (res.roomId) {
          this.onRoomChange(res.roomId, false);
          this.form.patchValue({ bedId: res.bedId });
        }
      });
    } else if (this.selectedDate) {
      this.form.patchValue({
        startDate: this.formatDate(this.selectedDate),
        endDate: this.formatDate(this.selectedDate)
      });
    }
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return null;
    return new Date(dateStr).toISOString().slice(0, 16);
  }

  buildForm() {
    this.form = this.fb.group({
      patientId: [null, [Validators.required]],
      roomId: [null, [Validators.required]],
      bedId: [null],
      startDate: [null, [Validators.required]],
      endDate: [null, [Validators.required]],
      status: [ReservationStatus.Pending, [Validators.required]],
      notes: ['', [Validators.maxLength(1024)]],
    });
  }

  loadLookups() {
    this.roomService.getList({ maxResultCount: 1000 }).subscribe(res => {
      this.rooms = res.items;
    });

    // For MVP, load first 100 patients. Ideally use search on typeahead.
    this.patientService.getList({ maxResultCount: 100 }).subscribe(res => {
      this.patients = res.items;
    });
  }

  onRoomChange(roomId: string, clearBed: boolean = true) {
    const room = this.rooms.find(r => r.id === roomId);
    if (room) {
      this.beds = room.beds || [];
    } else {
      this.beds = [];
    }
    if (clearBed) {
      this.form.patchValue({ bedId: null });
    }
  }

  save() {
    if (this.form.invalid) return;

    const val = this.form.value;
    // Format dates to string if needed by backend or let wrapper handle it. 
    // ABP handles Date objects to ISO string usually.

    const request = this.selectedId
      ? this.reservationService.update(this.selectedId, val)
      : this.reservationService.create(val);

    request.subscribe(() => {
      this.activeModal.close(true);
    });
  }
}
