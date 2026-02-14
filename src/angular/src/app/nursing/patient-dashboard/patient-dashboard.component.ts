import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PatientService } from '../../proxy/patients/patient.service';
import { PatientDto } from '../../proxy/patients/models';

@Component({
    selector: 'app-patient-dashboard',
    templateUrl: './patient-dashboard.component.html',
    styleUrls: ['./patient-dashboard.component.scss'],
    standalone: false
})
export class PatientDashboardComponent implements OnInit {
    patientId: string;
    patient: PatientDto;
    activeTab = 1;

    constructor(
        private route: ActivatedRoute,
        private patientService: PatientService
    ) { }

    ngOnInit(): void {
        this.route.params.subscribe((params) => {
            this.patientId = params['patientId'];
            if (this.patientId) {
                this.getPatient();
            }
        });
    }

    getPatient() {
        this.patientService.get(this.patientId).subscribe((res) => {
            this.patient = res;
        });
    }
}
