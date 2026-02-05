import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FullCalendarModule, FullCalendarComponent } from '@fullcalendar/angular';
import { CalendarOptions, DateSelectArg, EventClickArg } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import listPlugin from '@fullcalendar/list';
import arLocale from '@fullcalendar/core/locales/ar';

import { AppointmentService } from '../../proxy/appointments/appointment.service';
import { LookupDto, CreateAppointmentDto, AppointmentDto } from '../../proxy/appointments/dtos/models';
import { AppointmentType } from '../../proxy/appointments/appointment-type.enum';
import { ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { LocalizationModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { DoctorScheduleService } from '../../proxy/appointments/doctor-schedule.service';
import { DoctorScheduleDto } from '../../proxy/appointments/models';
import { NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PatientService } from '../../proxy/patients/patient.service';
import { PatientLookupDto } from '../../proxy/patients/models';
import { Subject, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, catchError } from 'rxjs/operators';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, FormsModule, FullCalendarModule, LocalizationModule, NgbModule, ThemeSharedModule],
  templateUrl: './booking.html',
  styleUrls: ['./booking.scss'],
  encapsulation: ViewEncapsulation.None
})
export class BookingComponent implements OnInit {
  @ViewChild('calendar') calendarComponent: FullCalendarComponent;

  clinics: LookupDto<string>[] = [];
  doctors: LookupDto<string>[] = [];

  selectedClinicId: string = '';
  selectedDoctorId: string = '';

  patients: PatientLookupDto[] = [];
  patientSearch$ = new Subject<string>();
  isSearchingPatients = false;

  calendarOptions: CalendarOptions = {
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin, listPlugin],
    initialView: 'timeGridWeek',
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'timeGridWeek,timeGridDay'
    },
    weekends: true,
    editable: false,
    selectable: true,
    selectMirror: true,
    dayMaxEvents: true,
    slotMinTime: '08:00:00',
    slotMaxTime: '22:00:00',
    allDaySlot: false,
    direction: 'rtl',
    locale: 'ar',
    locales: [arLocale],
    select: this.handleDateSelect.bind(this),
    eventClick: this.handleEventClick.bind(this),
    events: this.fetchEvents.bind(this),
    businessHours: [] // Will be populated dynamically
  };

  currentEvents: any[] = [];
  isModalOpen = false;
  bookingData: Partial<CreateAppointmentDto> = {};

  appointmentTypes = Object.keys(AppointmentType)
    .filter(k => !isNaN(Number(k)))
    .map(k => ({ value: Number(k), label: AppointmentType[k] }));

  constructor(
    private appointmentService: AppointmentService,
    private doctorScheduleService: DoctorScheduleService,
    private patientService: PatientService,
    private toaster: ToasterService,
    private confirmation: ConfirmationService
  ) {
    this.setupPatientSearch();
  }

  setupPatientSearch() {
    this.patientSearch$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(text => {
        if (!text || text.length < 2) return of([]);
        this.isSearchingPatients = true;
        return this.patientService.search(text).pipe(
          catchError(() => of([]))
        );
      })
    ).subscribe(res => {
      this.patients = res;
      this.isSearchingPatients = false;
    });
  }

  ngOnInit(): void {
    this.loadClinics();
  }

  loadClinics() {
    this.appointmentService.getClinicLookup().subscribe(res => {
      this.clinics = res as any; // Temporary cast due to proxy issue
    });
  }

  onClinicChange() {
    this.selectedDoctorId = '';
    this.doctors = [];
    this.resetCalendar();

    if (this.selectedClinicId) {
      this.appointmentService.getDoctorLookup(this.selectedClinicId).subscribe(res => {
        this.doctors = res as any;
      });
    }
  }

  onDoctorChange() {
    if (this.selectedDoctorId) {
      this.updateBusinessHours();
      this.calendarComponent.getApi().refetchEvents();
    } else {
      this.resetCalendar();
    }
  }

  resetCalendar() {
    this.calendarOptions.businessHours = []; // Clear
    // We can't easily trigger re-render of options without replacement in some wrappers, 
    // but let's try assuming FullCalendar angular handles input changes or we use setOption.
    if (this.calendarComponent) {
      this.calendarComponent.getApi().setOption('businessHours', []);
      this.calendarComponent.getApi().removeAllEvents();
    }
  }

  updateBusinessHours() {
    this.doctorScheduleService.getList({ maxResultCount: 100 }).subscribe(res => {
      const schedules = res.items.filter(x => x.doctorId === this.selectedDoctorId && x.isActive);
      const businessHours = schedules.map(s => ({
        daysOfWeek: [s.dayOfWeek], // 0=Sunday
        startTime: s.startTime,
        endTime: s.endTime
      }));

      if (this.calendarComponent) {
        this.calendarComponent.getApi().setOption('businessHours', businessHours);
      }
    });
  }

  typeColors: { [key: number]: string } = {
    0: '#0d6efd', // FirstVisit - Blue
    1: '#198754', // FollowUp - Green
    2: '#dc3545', // Emergency - Red
    3: '#fd7e14', // Consultation - Orange
    4: '#6f42c1', // Other - Purple
  };

  getTypeColor(type: number): string {
    return this.typeColors[type] || '#6c757d'; // Default Gray
  }

  fetchEvents(fetchInfo: any, successCallback: any, failureCallback: any) {
    if (!this.selectedDoctorId) {
      successCallback([]);
      return;
    }

    // Fetch existing appointments
    // We pass the start date of the view. The API takes 'date'. 
    // We assume backend filters broadly or we pass start.
    // We pass the start and end of the view range
    const startDate = fetchInfo.start.toISOString();
    const endDate = fetchInfo.end.toISOString();

    this.appointmentService.getList(this.selectedDoctorId, startDate, endDate).subscribe(res => {
      const events = res.map(appt => ({
        id: appt.id,
        title: 'Reserved', // Don't show patient name for privacy if public
        start: appt.appointmentDate, // Ensure this is ISO string
        // Calculate end based on duration if not stored? 
        // For now assume 30 mins or provided by backend?
        // If appointment doesn't have EndTime, FullCalendar defaults to 1h
        color: this.getTypeColor(appt.type),
        textColor: '#ffffff', // White text for visibility
        display: 'background' // Or block
      }));
      successCallback(events);
    });
  }

  handleDateSelect(selectInfo: DateSelectArg) {
    if (!this.selectedDoctorId) {
      this.toaster.warn('::SelectDoctorFirst');
      return;
    }

    // Optional: Check if selected slot is within business hours (FullCalendar select constraint can handle this too)
    // For now we allow clicking anywhere, but backend validates.

    this.bookingData = {
      doctorId: this.selectedDoctorId,
      clinicId: this.selectedClinicId,
      appointmentDate: selectInfo.startStr, // ISO string with timezone
      patientId: '',
      type: AppointmentType.NewVisit,
      isWalkIn: false,
      notes: ''
    };

    this.isModalOpen = true;
  }

  handleEventClick(clickInfo: EventClickArg) {
    const event = clickInfo.event;
    // Map event extendedProps if available or assume we have what we need. 
    // Ideally we should full fetch or store data in fetchEvents
    // But let's rely on what we put in fetchEvents: id, start, type (need to put type in extendedProps)

    // We need to fetch full details or rely on props.
    // Let's assume we can GET it or use local cache.
    // For now, let's just use what we have and maybe fetch if needed.
    // Actually we need 'notes' which isn't in event.propes usually.

    this.appointmentService.get(event.id).subscribe(appt => {
      this.bookingData = {
        id: appt.id, // Keep ID for update check
        doctorId: appt.doctorId,
        clinicId: appt.clinicId,
        patientId: appt.patientId,
        appointmentDate: appt.appointmentDate,
        type: appt.type,
        isWalkIn: appt.isWalkIn,
        notes: appt.notes
      } as any;

      // Ensure the patient is in the list for selection display
      if (appt.patientId) {
        this.patients = [{
          id: appt.patientId,
          fullNameAr: appt.patientName,
          mrn: '' // We don't have MRN here but ID/Name is enough for the selection
        }];
      }

      this.selectedDoctorId = appt.doctorId; // Ensure ctx matches
      this.isModalOpen = true;
    });
  }

  saveBooking() {
    if (!this.bookingData.doctorId) return;
    if (!this.bookingData.patientId) {
      this.toaster.warn('Please select a patient first.');
      return;
    }

    if ((this.bookingData as any).id) {
      // Update
      this.appointmentService.update((this.bookingData as any).id, this.bookingData as CreateAppointmentDto).subscribe({
        next: () => {
          this.toaster.success('::AppointmentUpdatedSuccessfully');
          this.isModalOpen = false;
          this.calendarComponent.getApi().refetchEvents();
        },
        error: (err) => {
          this.toaster.error('::UpdateFailed');
          console.error(err);
        }
      });
    } else {
      // Create
      this.appointmentService.create(this.bookingData as CreateAppointmentDto).subscribe({
        next: () => {
          this.toaster.success('::AppointmentBookedSuccessfully');
          this.isModalOpen = false;
          this.calendarComponent.getApi().refetchEvents();
        },
        error: (err) => {
          this.toaster.error('::BookingFailed');
          console.error(err);
        }
      });
    }
  }

  cancelBooking() {
    if (!(this.bookingData as any).id) return;

    this.confirmation.warn('::AreYouSure', '::CancelAppointment').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.appointmentService.cancel((this.bookingData as any).id).subscribe(() => {
          this.toaster.success('::AppointmentCancelled');
          this.isModalOpen = false;
          this.calendarComponent.getApi().refetchEvents();
        });
      }
    });
  }
}
