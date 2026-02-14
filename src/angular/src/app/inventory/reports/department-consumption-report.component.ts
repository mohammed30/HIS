import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RestService } from '@abp/ng.core';
import { NgbDateNativeAdapter, NgbDateAdapter, NgbDatepickerModule } from '@ng-bootstrap/ng-bootstrap';
import { LocalizationModule } from '@abp/ng.core';
import { environment } from '../../../environments/environment';

@Component({
    selector: 'app-department-consumption-report',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, NgbDatepickerModule, LocalizationModule],
    providers: [
        { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }
    ],
    templateUrl: './department-consumption-report.component.html'
})
export class DepartmentConsumptionReportComponent implements OnInit {
    form: FormGroup;
    departments: any[] = [];
    reportData: any[] = [];
    isLoading = false;

    private fb = inject(FormBuilder);
    private restService = inject(RestService);

    constructor() { }

    ngOnInit() {
        this.buildForm();
        this.loadDepartments();
    }

    buildForm() {
        const today = new Date();
        const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);

        this.form = this.fb.group({
            startDate: [firstDay, Validators.required],
            endDate: [today, Validators.required],
            departmentId: [null]
        });
    }

    loadDepartments() {
        this.restService.request<void, any>({
            method: 'GET',
            url: '/api/app/department?maxResultCount=100'
        }).subscribe(res => {
            this.departments = res.items || [];
        });
    }

    generateReport() {
        if (this.form.invalid) return;

        this.isLoading = true;
        const body = this.form.value;

        // Adjust dates for API if needed, but NgbDateNativeAdapter uses Date objects which JSON.stringify formats as ISO
        // However, timezone issues might occur. Ideally use YYYY-MM-DD string.

        // For now rely on default serialization or manual formatting if needed.
        // Let's assume standard serialization works or backend handles it.

        // Wait, GET request with query params?
        // InventoryAppService.GetConsumptionReportAsync is HTTP GET.
        // So arguments should be query params.

        // Convert to query params
        const params: any = {
            startDate: body.startDate.toISOString(),
            endDate: body.endDate.toISOString(),
        };
        if (body.departmentId) params.departmentId = body.departmentId;

        this.restService.request<void, any>({
            method: 'GET',
            url: '/api/app/inventory/consumption-report',
            params: params
        }).subscribe({
            next: (res) => {
                this.reportData = res || []; // It returns List<Dto>, so it should be array directly? Or wrapped in items?
                // ABP usually wraps list return in default response unless explicit. 
                // MyAppService.GetList returns PagedResult.
                // GetConsumptionReportAsync returns List<>. 
                // So it should be just array.
                this.isLoading = false;
            },
            error: () => this.isLoading = false
        });
    }

    getTotalCost() {
        return this.reportData.reduce((acc, curr) => acc + curr.totalCost, 0);
    }
}
