import { NgModule } from '@angular/core';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule, NgbDatepickerModule, NgbTypeaheadModule } from '@ng-bootstrap/ng-bootstrap';
import { ReservationsRoutingModule } from './reservations-routing-module';
import { ReservationListComponent } from './reservation-list/reservation-list';
import { ReservationDetailComponent } from './reservation-detail/reservation-detail';
import { FullCalendarModule } from '@fullcalendar/angular';

@NgModule({
  declarations: [],
  imports: [
    CoreModule,
    ThemeSharedModule,
    NgbDropdownModule,
    NgbDatepickerModule,
    NgbTypeaheadModule,
    ReservationsRoutingModule,
    FullCalendarModule
  ]
})
export class ReservationsModule { }
