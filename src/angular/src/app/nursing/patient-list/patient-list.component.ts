import { Component, OnInit } from '@angular/core';
import { PagedResultDto, ListService } from '@abp/ng.core';
import { AdmissionService } from '../../proxy/inpatient/admission.service';
import { AdmissionDto } from '../../proxy/inpatient/models';
import { Router } from '@angular/router';

@Component({
    selector: 'app-patient-list',
    templateUrl: './patient-list.component.html',
    styleUrls: ['./patient-list.component.scss'],
    providers: [ListService],
    standalone: false
})
export class PatientListComponent implements OnInit {
    admissions = { items: [], totalCount: 0 } as PagedResultDto<AdmissionDto>;

    constructor(
        public readonly list: ListService,
        private admissionService: AdmissionService,
        private router: Router
    ) { }

    ngOnInit(): void {
        const streamCreator = (query) => this.admissionService.getList({ ...query, maxResultCount: 100 }); // Pagination handling can be improved

        this.list.hookToQuery(streamCreator).subscribe((response) => {
            this.admissions = response;
        });
    }

    viewChart(admission: AdmissionDto) {
        this.router.navigate(['/nursing/patient', admission.patientId]);
    }
}
