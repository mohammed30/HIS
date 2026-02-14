import { Component, Input, OnInit } from '@angular/core';
import { MedicalRecordService } from '../../proxy/medical-records/medical-record.service';
import { VitalSignDto, CreateUpdateVitalSignDto } from '../../proxy/medical-records/models';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
    selector: 'app-vital-signs',
    templateUrl: './vital-signs.component.html',
    styleUrls: ['./vital-signs.component.scss'],
    standalone: false
})
export class VitalSignsComponent implements OnInit {
    @Input() patientId: string;

    vitalSigns: VitalSignDto[] = [];
    newVital: CreateUpdateVitalSignDto = {} as CreateUpdateVitalSignDto;
    isAdding = false;

    constructor(
        private medicalRecordService: MedicalRecordService,
        private toaster: ToasterService
    ) { }

    ngOnInit(): void {
        if (this.patientId) {
            this.getVitalSigns();
        }
    }

    getVitalSigns() {
        this.medicalRecordService.getVitalSigns(this.patientId).subscribe((res) => {
            this.vitalSigns = res.items;
        });
    }

    toggleAdd() {
        this.isAdding = !this.isAdding;
        if (this.isAdding) {
            this.newVital = { patientId: this.patientId, recordedAt: new Date().toISOString() } as CreateUpdateVitalSignDto;
        }
    }

    save() {
        this.newVital.recordedAt = new Date().toISOString();
        this.medicalRecordService.createVitalSign(this.newVital).subscribe(() => {
            this.toaster.success('::SuccessfullySaved', '::Success');
            this.isAdding = false;
            this.getVitalSigns();
        });
    }

    cancel() {
        this.isAdding = false;
    }
}
