import { Component, Input, OnInit } from '@angular/core';
import { NursingService } from '../../proxy/nursing/nursing.service';
import { DueMedicationDto, MedicationAdministrationDto, CreateMedicationAdministrationDto, AdministrationStatus } from '../../proxy/nursing/models';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-medication-administration',
    templateUrl: './medication-administration.component.html',
    styleUrls: ['./medication-administration.component.scss'],
    standalone: false
})
export class MedicationAdministrationComponent implements OnInit {
    @Input() patientId: string;

    dueMedications: DueMedicationDto[] = [];
    history: MedicationAdministrationDto[] = [];
    status = AdministrationStatus;

    constructor(
        private nursingService: NursingService,
        private toaster: ToasterService
    ) { }

    ngOnInit(): void {
        if (this.patientId) {
            this.refresh();
        }
    }

    refresh() {
        this.nursingService.getDueMedications(this.patientId).subscribe((res) => {
            this.dueMedications = res;
        });
        this.nursingService.getMedicationAdministrations(this.patientId).subscribe((res) => {
            this.history = res;
        });
    }

    administer(medication: DueMedicationDto) {
        const input: CreateMedicationAdministrationDto = {
            patientId: this.patientId,
            medicalOrderId: medication.id,
            administrationTime: new Date().toISOString(),
            status: AdministrationStatus.Given,
            dosage: medication.dosage,
            notes: ''
        };

        this.nursingService.createMedicationAdministration(input).subscribe(() => {
            this.toaster.success('::MedicationAdministered', '::Success');
            this.refresh();
        });
    }

    markAs(medication: DueMedicationDto, status: AdministrationStatus) {
        // Handling Refused/Skipped/Late - showing minimal implementation here, ideally a modal for notes
        const input: CreateMedicationAdministrationDto = {
            patientId: this.patientId,
            medicalOrderId: medication.id,
            administrationTime: new Date().toISOString(),
            status: status,
            dosage: medication.dosage,
            notes: status === AdministrationStatus.Refused ? 'Patient refused' : ''
        };

        this.nursingService.createMedicationAdministration(input).subscribe(() => {
            this.toaster.success('::StatusUpdated', '::Success');
            this.refresh();
        });
    }
}
