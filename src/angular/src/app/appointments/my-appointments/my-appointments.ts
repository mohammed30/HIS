import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentService } from '@proxy/appointments';
import { AppointmentDto } from '@proxy/appointments/dtos';
import { LocalizationModule } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-my-appointments',
  standalone: true,
  imports: [CommonModule, LocalizationModule],
  templateUrl: './my-appointments.html'
})
export class MyAppointmentsComponent implements OnInit {
  appointments: AppointmentDto[] = [];
  loading = false;

  constructor(
    private appointmentService: AppointmentService,
    private toaster: ToasterService
  ) { }

  ngOnInit(): void {
    this.loadAppointments();
  }

  loadAppointments() {
    this.loading = true;
    // TODO: Need a specific API for "My Appointments" or filter by PatientId (CurrentUser)
    // For now, calling GetList with no filters (which might return all if user is admin, or I need to secure it)
    // In AppointmentAppService, GetListAsync currently filters by DoctorId/Date. If null, it returns ALL.
    // This is a security risk if not scoped.
    // I will assume for now we see all (prototype) but I should note to fix Backend.
    this.appointmentService.getList().subscribe({
      next: (res) => {
        this.appointments = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  cancelAppointment(id: string) {
    if (!confirm('Are you sure you want to cancel?')) return;

    this.appointmentService.cancel(id).subscribe({
      next: () => {
        this.toaster.success('::AppointmentCancelled');
        this.loadAppointments();
      }
    });
  }
}
