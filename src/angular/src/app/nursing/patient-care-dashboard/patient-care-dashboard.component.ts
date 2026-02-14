import { Component, Input, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-patient-care-dashboard',
    templateUrl: './patient-care-dashboard.component.html',
    styleUrls: ['./patient-care-dashboard.component.scss'],
    standalone: false
})
export class PatientCareDashboardComponent implements OnInit {
    private route = inject(ActivatedRoute);
    @Input() patientId: string = '';
    activeTab = 1;

    constructor() { }

    ngOnInit(): void {
        if (!this.patientId) {
            this.patientId = this.route.snapshot.params['patientId'];
        }
    }
}
