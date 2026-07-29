import { Component, OnInit, inject, OnDestroy } from '@angular/core';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';
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
import { ReferenceRange, ResultStatus } from '../catalog/lab-catalog.component';


@Component({
    selector: 'app-lab-requests',
    standalone: true,
    imports: [CommonModule, FormsModule, LocalizationModule, NgbPaginationModule, ThemeSharedModule],
    providers: [ListService],
    templateUrl: './lab-requests.component.html'
})
export class LabRequestsComponent implements OnInit, OnDestroy {
    labService = inject(LabService);
    patientService = inject(PatientService);
    doctorService = inject(DoctorService);
    list = inject(ListService);
    toaster = inject(ToasterService);

    data: PagedResultDto<LabRequestDto> = { items: [], totalCount: 0 };
    searchText = '';
    private searchSubject = new Subject<string>();
    private destroy$ = new Subject<void>();

    // Filters
    filterByDate = true;
    fromDate = new Date().toISOString().split('T')[0];
    toDate = new Date().toISOString().split('T')[0];
    selectedStatus: LabRequestStatus | null = null;
    LabRequestStatus = LabRequestStatus;

    // ─── Reference Range Helpers ─────────────────────────────────────────────
    parseReferenceRanges(raw?: string | null): ReferenceRange[] {
        if (!raw) return [];
        try {
            const parsed = JSON.parse(raw);
            if (Array.isArray(parsed)) return parsed as ReferenceRange[];
        } catch {
            const match = raw.match(/^(\d+\.?\d*)\s*[-–]\s*(\d+\.?\d*)$/);
            if (match) {
                return [{ label: 'المرجع الطبيعي', min: parseFloat(match[1]), max: parseFloat(match[2]), criticalMin: null, criticalMax: null, unit: '' }];
            }
        }
        return [];
    }

    hasStructuredRanges(raw?: string | null): boolean {
        if (!raw) return false;
        try { const p = JSON.parse(raw); return Array.isArray(p) && p.length > 0; }
        catch { return false; }
    }

    getResultStatus(resultValue: string, raw?: string | null): ResultStatus {
        const value = parseFloat(resultValue);
        if (isNaN(value)) return 'unknown';
        const ranges = this.parseReferenceRanges(raw);
        if (!ranges.length) return 'unknown';
        const r = ranges[0];
        const min = r.min ?? -Infinity;
        const max = r.max ?? Infinity;
        const cMin = r.criticalMin ?? -Infinity;
        const cMax = r.criticalMax ?? Infinity;
        if (value >= min && value <= max) return 'normal';
        if (value >= cMin && value <= cMax) return 'warning';
        return 'danger';
    }

    getStatusBadgeClass(status: ResultStatus): string {
        return { normal: 'bg-success', warning: 'bg-warning text-dark', danger: 'bg-danger', unknown: 'bg-secondary' }[status] || 'bg-secondary';
    }

    getOverallResultStatus(): ResultStatus {
        if (!this.selectedRequest) return 'unknown';
        
        if (!this.isStructuredRange) {
             return this.getResultStatus(this.resultData.result, this.selectedRequest.referenceRange);
        }

        let hasWarning = false;
        let hasDanger = false;
        let hasNormal = false;

        for (const r of this.parsedRanges) {
            const val = this.structuredResults[r.label];
            if (val === null || val === undefined || Number.isNaN(val)) continue;
            
            const min = r.min ?? -Infinity;
            const max = r.max ?? Infinity;
            let cMin = r.criticalMin ?? -Infinity;
            let cMax = r.criticalMax ?? Infinity;
            
            // Smart protection: ignore invalid critical thresholds caused by typos
            if (cMax <= max) cMax = Infinity; 
            if (cMin >= min) cMin = -Infinity;
            
            if (val < cMin || val > cMax) {
                hasDanger = true;
            } else if (val < min || val > max) {
                hasWarning = true;
            } else {
                hasNormal = true;
            }
        }

        if (hasDanger) return 'danger';
        if (hasWarning) return 'warning';
        if (hasNormal) return 'normal';
        
        return 'unknown';
    }

    getSingleResultStatus(r: ReferenceRange, val: any): ResultStatus {
        if (val === null || val === undefined || val === '' || Number.isNaN(Number(val))) return 'unknown';
        const numVal = Number(val);
        
        const min = r.min ?? -Infinity;
        const max = r.max ?? Infinity;
        let cMin = r.criticalMin ?? -Infinity;
        let cMax = r.criticalMax ?? Infinity;
        
        // Smart protection: ignore invalid critical thresholds caused by typos
        if (cMax <= max) cMax = Infinity; 
        if (cMin >= min) cMin = -Infinity;
        
        if (numVal < cMin || numVal > cMax) return 'danger';
        if (numVal < min || numVal > max) return 'warning';
        return 'normal';
    }

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
    parsedRanges: ReferenceRange[] = [];
    isStructuredRange = false;
    structuredResults: { [key: string]: number | null } = {};

    // Print
    printData?: LabRequestDto;

    statuses = LabRequestStatus;

    ngOnInit() {
        this.list.hookToQuery(query => {
            const params: any = { 
                ...query, 
                filter: this.searchText,
                status: this.selectedStatus
            };
            if (this.filterByDate) {
                params.fromDate = this.fromDate;
                params.toDate = this.toDate;
            }
            return this.labService.getRequests(params);
        }).subscribe(res => {
            this.data = res;
        });

        this.searchSubject.pipe(
            debounceTime(500),
            distinctUntilChanged(),
            takeUntil(this.destroy$)
        ).subscribe(() => {
            this.list.get();
        });

        this.loadDropdownData();
    }

    ngOnDestroy() {
        this.destroy$.next();
        this.destroy$.complete();
    }

    onSearch(value: string | any) {
        this.searchText = typeof value === 'string' ? value : value.target.value;
        this.searchSubject.next(this.searchText);
    }

    onFilterChange() {
        this.list.get();
    }

    toggleDateFilter() {
        this.filterByDate = !this.filterByDate;
        this.list.get();
    }

    filterByStatus(status: LabRequestStatus | null) {
        this.selectedStatus = status;
        this.list.page = 0;
        this.list.get();
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
            this.filteredLabTests = [...this.labTests];
        });
    }

    refresh() {
        this.list.get();
    }

    openCreateModal() {
        this.newRequest = { patientId: '', doctorId: '', serviceItemId: '', notes: '' };
        this.labTestSearch = '';
        this.filteredLabTests = [...this.labTests];
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
        this.parsedRanges = this.parseReferenceRanges(request.referenceRange);
        this.isStructuredRange = this.hasStructuredRanges(request.referenceRange);
        
        this.structuredResults = {};
        if (this.isStructuredRange && request.result) {
            if (this.parsedRanges.length === 1) {
                const val = parseFloat(request.result);
                if (!isNaN(val)) this.structuredResults[this.parsedRanges[0].label] = val;
            } else {
                const lines = request.result.split('\n');
                lines.forEach(line => {
                    const match = line.match(/^(.*?):\s*([\d.]+)/);
                    if (match && match[1] && match[2]) {
                        this.structuredResults[match[1].trim()] = parseFloat(match[2]);
                    }
                });
            }
        }
        
        this.isResultModalOpen = true;
    }

    updateCombinedResult() {
        if (!this.isStructuredRange) return;
        
        if (this.parsedRanges.length === 1) {
            const val = this.structuredResults[this.parsedRanges[0].label];
            this.resultData.result = val !== null && val !== undefined ? val.toString() : '';
        } else {
            this.resultData.result = this.parsedRanges
                .map(r => {
                    const val = this.structuredResults[r.label];
                    if (val !== null && val !== undefined && !Number.isNaN(val)) {
                        return `${r.label}: ${val} ${r.unit || ''}`.trim();
                    }
                    return null;
                })
                .filter(Boolean)
                .join('\n');
        }
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

    printBarcode(id: string) {
        this.labService.getSampleBarcodePdf(id).subscribe((blob: Blob) => {
            const url = window.URL.createObjectURL(blob);
            window.open(url, '_blank');
        }, err => {
            console.error(err);
            this.toaster.error('حدث خطأ أثناء تحميل ملصق الباركود');
        });
    }

    getStatusClass(status: number) {
        switch (status) {
            case LabRequestStatus.Requested: return 'bg-warning text-dark';
            case LabRequestStatus.SampleCollected: return 'bg-info text-dark';
            case LabRequestStatus.Completed: return 'bg-success';
            case LabRequestStatus.Cancelled: return 'bg-danger';
            default: return 'bg-secondary';
        }
    }

    getStatusLabel(status: number) {
        if (status === undefined || status === null) return '::Enum:LabRequestStatus.Unknown';
        return '::Enum:LabRequestStatus.' + status;
    }

    // Custom searchable dropdown for Lab Tests
    labTestSearch = '';
    isLabTestDropdownOpen = false;
    filteredLabTests: any[] = [];

    filterLabTests() {
        if (!this.labTestSearch) {
            this.filteredLabTests = [...this.labTests];
            return;
        }
        const term = this.labTestSearch.toLowerCase();
        this.filteredLabTests = this.labTests.filter(t => 
            (t.name && t.name.toLowerCase().includes(term)) || 
            (t.code && t.code.toLowerCase().includes(term))
        );
    }

    selectLabTest(test: any) {
        this.newRequest.serviceItemId = test.id;
        this.labTestSearch = `[${test.code}] ${test.name}`;
        this.isLabTestDropdownOpen = false;
    }

    closeLabTestDropdown() {
        setTimeout(() => {
            this.isLabTestDropdownOpen = false;
            const selected = this.labTests.find(t => t.id === this.newRequest.serviceItemId);
            if (selected) {
                 this.labTestSearch = `[${selected.code}] ${selected.name}`;
            } else {
                 this.labTestSearch = '';
                 this.newRequest.serviceItemId = '';
            }
        }, 200);
    }
}
