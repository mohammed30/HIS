import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmergencyService } from '../../proxy/emergency/emergency.service';
import { EmergencyVisitDto, CreateEmergencyVisitDto, TriageDto } from '../../proxy/emergency/dtos/models';
import { EmergencySeverity, EmergencyVisitStatus } from '../../proxy/emergency';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ThemeSharedModule, ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { LocalizationModule } from '@abp/ng.core';
import { AppointmentService } from '../../proxy/appointments/appointment.service'; // Reusing for Patient Lookup if needed, or build generic one

@Component({
    selector: 'app-emergency-dashboard',
    standalone: true,
    imports: [CommonModule, FormsModule, LocalizationModule, NgbPaginationModule, ThemeSharedModule],
    providers: [ListService],
    templateUrl: './emergency-dashboard.component.html',
    styles: [`
      .blinking-badge { animation: blinker 1.5s linear infinite; }
      @keyframes blinker { 50% { opacity: 0; } }
    `]
})
export class EmergencyDashboardComponent implements OnInit {
    emergencyService = inject(EmergencyService);
    list = inject(ListService);
    toaster = inject(ToasterService);
    confirmation = inject(ConfirmationService);

    data: PagedResultDto<EmergencyVisitDto> = { items: [], totalCount: 0 };

    // Modals
    isRegisterModalOpen = false;
    isTriageModalOpen = false;

    // Data Models
    registerData: CreateEmergencyVisitDto = { patientId: '', chiefComplaint: '' };
    triageData: TriageDto = {
        severity: EmergencySeverity.NonUrgent,
        bloodPressure: '', heartRate: 0, temperature: 0, respiratoryRate: 0, oxygenSaturation: 0, notes: ''
    };
    selectedVisitId = '';
    selectedVisit?: EmergencyVisitDto;

    severities = EmergencySeverity;

    // Mock patients for MVP - In real app, use a searchable dropdown
    // Or reuse existing Patient Lookup Service if available
    // Making a simple input for PatientId is hard, so let's assume we copy ID or implement a basic fetch.
    // For MVP: I'll use a text input for ID. 

    ngOnInit() {
        this.list.hookToQuery(query => this.emergencyService.getActiveVisits(query)).subscribe(res => {
            this.data = res;
        });

        // Auto refresh every 30 sec
        setInterval(() => this.list.get(), 30000);
    }

    // Actions
    openRegister() {
        this.registerData = { patientId: '', chiefComplaint: '' };
        this.isRegisterModalOpen = true;
    }

    saveRegister() {
        this.emergencyService.register(this.registerData).subscribe(() => {
            this.toaster.success('Patient Registered');
            this.isRegisterModalOpen = false;
            this.list.get();
        });
    }

    openTriage(visit: EmergencyVisitDto) {
        this.selectedVisitId = visit.id;
        this.selectedVisit = visit;
        this.triageData = {
            severity: visit.severity,
            bloodPressure: visit.bloodPressure,
            heartRate: visit.heartRate,
            temperature: visit.temperature,
            respiratoryRate: visit.respiratoryRate,
            oxygenSaturation: visit.oxygenSaturation,
            notes: visit.notes
        };
        this.isTriageModalOpen = true;
    }

    saveTriage() {
        if (this.selectedVisitId) {
            this.emergencyService.performTriage(this.selectedVisitId, this.triageData).subscribe(() => {
                this.toaster.success('Triage Updated');
                this.isTriageModalOpen = false;
                this.list.get();
            });
        }
    }

    discharge(id: string) {
        this.confirmation.warn('::AreYouSureToDischarge', '::AreYouSure').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.emergencyService.updateStatus(id, { status: EmergencyVisitStatus.Discharged, notes: '' }).subscribe(() => {
                    this.toaster.success('Discharged');
                    this.list.get();
                });
            }
        });
    }

    // Helpers
    getSeverityBadge(severity: number) {
        switch (severity) {
            case EmergencySeverity.Resuscitation: return 'bg-danger text-white blinking-badge'; // Level 1
            case EmergencySeverity.Emergent: return 'bg-warning text-dark'; // Level 2
            case EmergencySeverity.Urgent: return 'bg-primary'; // Level 3
            default: return 'bg-secondary';
        }
    }
}
