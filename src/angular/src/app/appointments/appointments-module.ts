import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';
import { NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap'; // Useful for calendar

import { AppointmentsRoutingModule } from './appointments-routing-module';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ThemeSharedModule,
    AppointmentsRoutingModule
  ]
})
export class AppointmentsModule { }
