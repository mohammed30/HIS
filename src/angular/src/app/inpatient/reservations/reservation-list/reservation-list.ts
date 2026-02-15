import { Component, OnInit, inject } from '@angular/core';
import { CalendarOptions, EventClickArg } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin, { DateClickArg } from '@fullcalendar/interaction';
import listPlugin from '@fullcalendar/list';
import arLocale from '@fullcalendar/core/locales/ar';
import { ReservationService } from '../../../proxy/inpatient';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ReservationDetailComponent } from '../reservation-detail/reservation-detail';
import { ToasterService } from '@abp/ng.theme.shared';

import { CoreModule, PermissionService } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { FullCalendarModule } from '@fullcalendar/angular';

@Component({
  selector: 'app-reservation-list',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, FullCalendarModule],
  templateUrl: './reservation-list.html',
  styleUrls: ['./reservation-list.scss']
})
export class ReservationListComponent implements OnInit {
  reservationService = inject(ReservationService);
  modalService = inject(NgbModal);
  toaster = inject(ToasterService);
  permissionService = inject(PermissionService);

  calendarOptions: CalendarOptions = {
    initialView: 'dayGridMonth',
    plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin, listPlugin],
    headerToolbar: {
      left: 'prev,next today',
      center: 'title',
      right: 'dayGridMonth,timeGridWeek,timeGridDay,listWeek'
    },
    editable: false,
    selectable: true,
    selectMirror: true,
    dayMaxEvents: true,
    locale: arLocale,
    direction: 'rtl',
    dateClick: this.handleDateClick.bind(this),
    eventClick: this.handleEventClick.bind(this),
    events: this.loadEvents.bind(this)
  };

  ngOnInit() {
  }

  loadEvents(arg: any, successCallback: any, failureCallback: any) {
    this.reservationService.getList({
      maxResultCount: 1000,
      fromDate: arg.startStr,
      toDate: arg.endStr
    }).subscribe(res => {
      const events = res.items.map(r => ({
        id: r.id,
        title: `${r.patientName} (${r.roomNumber}${r.bedNumber ? '-' + r.bedNumber : ''})`,
        start: r.startDate,
        end: r.endDate,
        color: this.getEventColor(r.status)
      }));
      successCallback(events);
    });
  }

  handleDateClick(arg: DateClickArg) {
    if (this.permissionService.getGrantedPolicy('HIS.Inpatient.Reservations.Create')) {
      this.openModal(null, arg.dateStr);
    } else {
      this.toaster.error('::Permission:Denied');
    }
  }

  handleEventClick(arg: EventClickArg) {
    this.openModal(arg.event.id);
  }

  openModal(id?: string, date?: string) {
    const modal = this.modalService.open(ReservationDetailComponent, { size: 'lg' });
    modal.componentInstance.selectedId = id;
    if (date) {
      modal.componentInstance.selectedDate = date;
    }
    modal.result.then((result) => {
      if (result) {
        this.toaster.success(id ? 'Successfully Updated' : 'Successfully Created');
        // Refresh calendar? The events function is called automatically on view change, 
        // to force refresh we might need a ref to calendar api, but simple page reload or navigation works for MVP.
        // Better:
        // this.calendarComponent.getApi().refetchEvents();
      }
    }, () => { });
  }

  getEventColor(status: number): string {
    switch (status) {
      case 0: return '#ffc107'; // Pending - Warning
      case 1: return '#28a745'; // Confirmed - Success
      case 2: return '#17a2b8'; // CheckIn - Info
      case 3: return '#dc3545'; // Cancelled - Danger
      default: return '#6c757d'; // Gray
    }
  }
}
