import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LabService } from '../../proxy/laboratory/lab.service';
import { LabAppointmentDto, CreateLabAppointmentDto, UpdateLabAppointmentDto } from '../../proxy/laboratory/dtos/models';
import { LabAppointmentStatus } from '../../proxy/laboratory/lab-appointment-status.enum';
import { ListService, PagedResultDto, LocalizationModule } from '@abp/ng.core';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ThemeSharedModule, ToasterService, ConfirmationService } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-lab-appointments',
    standalone: true,
    imports: [CommonModule, FormsModule, LocalizationModule, NgbPaginationModule, ThemeSharedModule],
    providers: [ListService],
    templateUrl: './lab-appointments.component.html'
})
export class LabAppointmentsComponent implements OnInit {
    labService = inject(LabService);
    list = inject(ListService);
    toaster = inject(ToasterService);
    confirmation = inject(ConfirmationService);

    data: PagedResultDto<LabAppointmentDto> = { items: [], totalCount: 0 };

    isModalOpen = false;
    selectedAppointment: Partial<CreateLabAppointmentDto> = {};
    selectedId?: string;
    isEditing = false;

    statuses = LabAppointmentStatus;

    ngOnInit() {
        this.list.hookToQuery(query => this.labService.getAppointments(query)).subscribe(res => {
            this.data = res;
        });
    }

    createAppointment() {
        this.selectedAppointment = {
            appointmentDate: new Date().toISOString().split('T')[0],
            isFasting: false
        };
        this.selectedId = undefined;
        this.isEditing = false;
        this.isModalOpen = true;
    }

    editAppointment(appointment: LabAppointmentDto) {
        this.selectedAppointment = {
            patientId: appointment.patientId,
            serviceItemId: appointment.serviceItemId,
            appointmentDate: appointment.appointmentDate.split('T')[0],
            preferredTime: appointment.preferredTime,
            notes: appointment.notes,
            isFasting: appointment.isFasting
        };
        this.selectedId = appointment.id;
        this.isEditing = true;
        this.isModalOpen = true;
    }

    save() {
        if (this.selectedId) {
            const updateDto: UpdateLabAppointmentDto = {
                serviceItemId: this.selectedAppointment.serviceItemId,
                appointmentDate: this.selectedAppointment.appointmentDate!,
                preferredTime: this.selectedAppointment.preferredTime,
                notes: this.selectedAppointment.notes,
                isFasting: this.selectedAppointment.isFasting ?? false
            };
            this.labService.updateAppointment(this.selectedId, updateDto).subscribe(() => {
                this.toaster.success('تم تحديث الحجز بنجاح');
                this.isModalOpen = false;
                this.list.get();
            });
        } else {
            this.labService.createAppointment(this.selectedAppointment as CreateLabAppointmentDto).subscribe(() => {
                this.toaster.success('تم إنشاء الحجز بنجاح');
                this.isModalOpen = false;
                this.list.get();
            });
        }
    }

    confirmAppointment(id: string) {
        this.labService.confirmAppointment(id).subscribe(() => {
            this.toaster.success('تم تأكيد الحجز');
            this.list.get();
        });
    }

    checkInAppointment(id: string) {
        this.labService.checkInAppointment(id).subscribe(() => {
            this.toaster.success('تم تسجيل وصول المريض');
            this.list.get();
        });
    }

    completeAppointment(id: string) {
        this.labService.completeAppointment(id).subscribe(() => {
            this.toaster.success('تم إكمال الموعد');
            this.list.get();
        });
    }

    cancelAppointment(id: string) {
        this.confirmation.warn('هل أنت متأكد من إلغاء هذا الموعد؟', 'تأكيد الإلغاء').subscribe(status => {
            if (status === 'confirm') {
                this.labService.cancelAppointment(id).subscribe(() => {
                    this.toaster.info('تم إلغاء الموعد');
                    this.list.get();
                });
            }
        });
    }

    getStatusClass(status: LabAppointmentStatus): string {
        switch (status) {
            case LabAppointmentStatus.Scheduled: return 'bg-secondary';
            case LabAppointmentStatus.Confirmed: return 'bg-primary';
            case LabAppointmentStatus.CheckedIn: return 'bg-info text-dark';
            case LabAppointmentStatus.SampleCollecting: return 'bg-warning text-dark';
            case LabAppointmentStatus.Completed: return 'bg-success';
            case LabAppointmentStatus.Cancelled: return 'bg-danger';
            default: return 'bg-secondary';
        }
    }

    getStatusLabel(status: LabAppointmentStatus): string {
        switch (status) {
            case LabAppointmentStatus.Scheduled: return 'مجدول';
            case LabAppointmentStatus.Confirmed: return 'مؤكد';
            case LabAppointmentStatus.CheckedIn: return 'وصول';
            case LabAppointmentStatus.SampleCollecting: return 'جمع عينة';
            case LabAppointmentStatus.Completed: return 'مكتمل';
            case LabAppointmentStatus.Cancelled: return 'ملغي';
            default: return 'غير معروف';
        }
    }
}
