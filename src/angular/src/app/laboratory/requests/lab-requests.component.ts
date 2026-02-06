import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LabService } from '../../proxy/laboratory/lab.service';
import { LabRequestDto, UpdateLabResultDto, CreateLabRequestDto, LabTestDto } from '../../proxy/laboratory/dtos/models';
import { LabRequestStatus } from '../../proxy/laboratory/lab-request-status.enum';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
import { LocalizationModule } from '@abp/ng.core';
import { PatientService } from '../../proxy/patients/patient.service';
import { DoctorService } from '../../proxy/settings/doctor.service';

@Component({
    selector: 'app-lab-requests',
    standalone: true,
    imports: [CommonModule, FormsModule, LocalizationModule, NgbPaginationModule, ThemeSharedModule],
    providers: [ListService],
    templateUrl: './lab-requests.component.html'
})
export class LabRequestsComponent implements OnInit {
    labService = inject(LabService);
    patientService = inject(PatientService);
    doctorService = inject(DoctorService);
    list = inject(ListService);
    toaster = inject(ToasterService);

    data: PagedResultDto<LabRequestDto> = { items: [], totalCount: 0 };
    searchText = '';

    // Create modal
    isCreateModalOpen = false;
    newRequest = { patientId: '', doctorId: '', serviceItemId: '', notes: '' };
    patients: any[] = [];
    doctors: any[] = [];
    labTests: any[] = [];

    // Result modal
    isResultModalOpen = false;
    selectedRequest?: LabRequestDto;
    resultData: UpdateLabResultDto = { result: '', notes: '' };

    // Print
    printData?: LabRequestDto;

    statuses = LabRequestStatus;

    ngOnInit() {
        this.list.hookToQuery(query => this.labService.getRequests({ ...query, searchText: this.searchText })).subscribe(res => {
            this.data = res;
        });
        this.loadDropdownData();
    }

    loadDropdownData() {
        // Load patients
        this.patientService.getList({ maxResultCount: 1000 }).subscribe(res => {
            this.patients = res.items;
        });
        // Load doctors
        this.doctorService.getList({ maxResultCount: 1000 }).subscribe(res => {
            this.doctors = res.items;
        });
        // Load lab tests
        this.labService.getTests({ maxResultCount: 1000 }).subscribe(res => {
            this.labTests = res.items;
        });
    }

    openCreateModal() {
        this.newRequest = { patientId: '', doctorId: '', serviceItemId: '', notes: '' };
        this.isCreateModalOpen = true;
    }

    saveRequest() {
        if (!this.newRequest.patientId || !this.newRequest.doctorId || !this.newRequest.serviceItemId) {
            this.toaster.error('يرجى ملء جميع الحقول المطلوبة');
            return;
        }
        this.labService.createRequest(this.newRequest).subscribe(() => {
            this.toaster.success('تم إنشاء طلب التحليل بنجاح');
            this.isCreateModalOpen = false;
            this.list.get();
        });
    }

    collectSample(id: string) {
        this.labService.collectSample(id).subscribe(() => {
            this.toaster.success('تم جمع العينة');
            this.list.get();
        });
    }

    openResultModal(request: LabRequestDto) {
        this.selectedRequest = request;
        this.resultData = {
            result: request.result || '',
            notes: request.notes || ''
        };
        this.isResultModalOpen = true;
    }

    saveResult() {
        if (this.selectedRequest) {
            this.labService.completeRequest(this.selectedRequest.id, this.resultData).subscribe(() => {
                this.toaster.success('تم حفظ النتيجة');
                this.isResultModalOpen = false;
                this.list.get();
            });
        }
    }

    printResult(request: LabRequestDto) {
        this.labService.getResultPdf(request.id).subscribe((blob: Blob) => {
            const url = window.URL.createObjectURL(blob);
            window.open(url, '_blank');
        }, err => {
            console.error(err);
            this.toaster.error('حدث خطأ أثناء تحميل ملف الطباعة');
        });
    }

    getStatusClass(status: number) {
        switch (status) {
            case LabRequestStatus.Requested: return 'bg-warning text-dark';
            case LabRequestStatus.SampleCollected: return 'bg-info text-dark';
            case LabRequestStatus.Completed: return 'bg-success';
            default: return 'bg-secondary';
        }
    }

    getStatusLabel(status: number) {
        const labels: { [key: number]: string } = {
            0: 'مطلوب',
            1: 'تم جمع العينة',
            2: 'قيد المعالجة',
            3: 'مكتمل'
        };
        return labels[status] || 'غير معروف';
    }
}
