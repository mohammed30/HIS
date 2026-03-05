import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { Attendance } from './attendance/attendance';
import { CompensationItems } from './compensation-items/compensation-items';
import { Employees } from './employees/employees';
import { Leaves } from './leaves/leaves';
import { Loans } from './loans/loans';
import { Payroll } from './payroll/payroll';
import { Penalties } from './penalties/penalties';
import { Reports } from './reports/reports';

const routes: Routes = [
  { path: 'employees', component: Employees },
  { path: 'attendance', component: Attendance },
  { path: 'compensation-items', component: CompensationItems },
  { path: 'leaves', component: Leaves },
  { path: 'loans', component: Loans },
  { path: 'payroll', component: Payroll },
  { path: 'penalties', component: Penalties },
  { path: 'reports', component: Reports },
  { path: '', redirectTo: 'employees', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HrRoutingModule { }
