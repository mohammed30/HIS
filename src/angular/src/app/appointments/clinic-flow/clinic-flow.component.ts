import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AppointmentService } from '../../proxy/appointments/appointment.service';
import { LookupDto, AppointmentDto } from '../../proxy/appointments/dtos/models';
import { AppointmentStatus } from '../../proxy/appointments/appointment-status.enum';
import { LocalizationModule } from '@abp/ng.core';
import { ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-clinic-flow',
    standalone: true,
    imports: [CommonModule, FormsModule, LocalizationModule],
    templateUrl: './clinic-flow.component.html'
})
export class ClinicFlowComponent implements OnInit {
    appointmentService = inject(AppointmentService);
    toaster = inject(ToasterService);
    confirmation = inject(ConfirmationService);

    clinics: LookupDto<string>[] = [];
    doctors: LookupDto<string>[] = [];
    selectedClinicId: string = '';
    selectedDoctorId: string = '';

    appointments: AppointmentDto[] = [];
    loading = false;

    today = new Date(); // Defaults to today

    ngOnInit() {
        this.loadClinics();
    }

    loadClinics() {
        this.appointmentService.getClinicLookup().subscribe(res => {
            this.clinics = res as any;
        });
    }

    onClinicChange() {
        this.doctors = [];
        this.selectedDoctorId = '';
        this.appointments = [];
        if (this.selectedClinicId) {
            this.appointmentService.getDoctorLookup(this.selectedClinicId).subscribe(res => {
                this.doctors = res as any;
            });
        }
    }

    loadFlow() {
        if (!this.selectedDoctorId) return;

        this.loading = true;
        const start = new Date(this.today);
        start.setHours(0, 0, 0, 0);
        const end = new Date(this.today);
        end.setHours(23, 59, 59, 999);

        this.appointmentService.getList(this.selectedDoctorId, start.toISOString(), end.toISOString()).subscribe({
            next: (res) => {
                this.appointments = res;
                this.loading = false;
            },
            error: () => this.loading = false
        });
    }

    getStatusBadge(status: number) {
        switch (status) {
            case AppointmentStatus.Scheduled: return 'bg-secondary';
            case AppointmentStatus.Confirmed: return 'bg-primary';
            case AppointmentStatus.CheckedIn: return 'bg-info text-dark';
            case AppointmentStatus.InConsultation: return 'bg-warning text-dark';
            case AppointmentStatus.Completed: return 'bg-success';
            case AppointmentStatus.Cancelled: return 'bg-danger';
            case AppointmentStatus.NoShow: return 'bg-dark';
            default: return 'bg-secondary';
        }
    }

    getStatusLabel(status: number) {
        return AppointmentStatus[status];
    }

    // Actions
    checkIn(id: string) {
        this.appointmentService.checkIn(id).subscribe(() => {
            this.toaster.success('::PatientCheckedIn');
            this.loadFlow();
        });
    }

    startConsultation(id: string) {
        this.appointmentService.startConsultation(id).subscribe(() => {
            this.toaster.success('::ConsultationStarted');
            this.loadFlow();
        });
    }

    completeConsultation(id: string) {
        this.appointmentService.completeConsultation(id).subscribe(() => {
            this.toaster.success('::ConsultationCompleted');
            this.loadFlow();
        });
    }

    cancelAppointment(id: string) {
        this.confirmation.warn('::AreYouSure', '::CancelAppointment').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.appointmentService.cancel(id).subscribe(() => {
                    this.toaster.info('::AppointmentCancelled');
                    this.loadFlow();
                });
            }
        });
    }
}
