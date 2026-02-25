import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDatepickerModule, NgbDateAdapter, NgbDateNativeAdapter } from '@ng-bootstrap/ng-bootstrap';
import { SurgicalOperationService } from '../../proxy/operations/surgical-operation.service';
import { SurgicalOperationDto } from '../../proxy/operations/models';
import { DoctorService } from '../../proxy/settings/doctor.service';
import { SpecialtyService } from '../../proxy/settings/specialty.service';
import { DoctorDto } from '../../proxy/settings/models';
import { SpecialtyDto } from '../../proxy/settings/models';
import { finalize } from 'rxjs/operators';

@Component({
    selector: 'app-operations-report',
    standalone: true,
    imports: [
        CoreModule,
        CommonModule,
        ReactiveFormsModule,
        ThemeSharedModule,
        NgbDatepickerModule
    ],
    providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
    templateUrl: './operations-report.component.html',
    styleUrls: ['./operations-report.component.scss']
})
export class OperationsReportComponent implements OnInit {
    filterForm: FormGroup;
    operations: SurgicalOperationDto[] = [];
    doctors: DoctorDto[] = [];
    specialties: SpecialtyDto[] = [];
    loading = false;
    totalAmount = 0;

    constructor(
        private fb: FormBuilder,
        private operationService: SurgicalOperationService,
        private doctorService: DoctorService,
        private specialtyService: SpecialtyService
    ) {
        this.buildForm();
    }

    ngOnInit(): void {
        this.loadDropdowns();
        this.fetchData();
    }

    buildForm() {
        this.filterForm = this.fb.group({
            doctorId: [null],
            specialtyId: [null],
            fromDate: [new Date(new Date().setDate(1))], // Current month start
            toDate: [new Date()]
        });
    }

    loadDropdowns() {
        this.doctorService.getList({ maxResultCount: 1000 }).subscribe(res => this.doctors = res.items);
        this.specialtyService.getList({ maxResultCount: 1000 }).subscribe(res => this.specialties = res.items);
    }

    private formatDate(date: any): string | null {
        if (!date) return null;
        const d = new Date(date);
        if (isNaN(d.getTime())) return null;
        const year = d.getFullYear();
        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    fetchData() {
        this.loading = true;
        const formValue = this.filterForm.value;
        const filter = {
            ...formValue,
            fromDate: this.formatDate(formValue.fromDate),
            toDate: this.formatDate(formValue.toDate),
            maxResultCount: 1000
        };

        console.log('Fetching operations with filter:', filter);

        this.operationService.getList(filter).pipe(finalize(() => this.loading = false))
            .subscribe(res => {
                this.operations = res.items;
                this.calculateTotals();
            });
    }

    calculateTotals() {
        this.totalAmount = this.operations.reduce((sum, op) => sum + (op.totalAmount || 0), 0);
    }

    printPdf() {
        const formValue = this.filterForm.value;
        const filter = {
            ...formValue,
            fromDate: this.formatDate(formValue.fromDate),
            toDate: this.formatDate(formValue.toDate)
        };

        this.operationService.getOperationsReportPdf(filter).subscribe(blob => {
            const url = window.URL.createObjectURL(blob as any);
            const link = document.createElement('a');
            link.href = url;
            link.download = `SurgicalOperationsReport_${new Date().toISOString().split('T')[0]}.pdf`;
            link.click();
        });
    }
}
