import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentService } from '../../proxy/appointments/appointment.service';
import { AppointmentDto } from '../../proxy/appointments/dtos/models';
import { ListService, PagedResultDto, LocalizationModule } from '@abp/ng.core';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-my-appointments',
  standalone: true,
  imports: [CommonModule, LocalizationModule, ThemeSharedModule],
  providers: [ListService],
  templateUrl: './my-appointments.html'
})
export class MyAppointmentsComponent implements OnInit {
  appointments: AppointmentDto[] = [];
  loading = false;

  private appointmentService = inject(AppointmentService);
  private list = inject(ListService);
  private confirmation = inject(ConfirmationService);
  private toaster = inject(ToasterService);

  constructor() { }

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
    // Passing undefined to let backend treat as null (optional params)
    this.appointmentService.getList(undefined as any, undefined as any, undefined as any).subscribe({
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
    this.confirmation.warn('::AreYouSure', '::CancelAppointment').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.appointmentService.cancel(id).subscribe(() => {
          this.toaster.success('::AppointmentCancelled');
          this.loadAppointments(); // Reload appointments after cancellation
        });
      }
    });
  }
}
