import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { LabService } from '../../proxy/laboratory/lab.service';
import { LabTestDto, CreateUpdateLabTestDto } from '../../proxy/laboratory/dtos/models';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { NgbPaginationModule, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { ThemeSharedModule, ToasterService, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { LocalizationModule } from '@abp/ng.core';

export interface TestParameter {
    label: string;
    type: 'Numeric' | 'Text';
    operator: 'Between' | 'LessThan' | 'GreaterThan' | 'Text';
    min: number | null;
    max: number | null;
    criticalMin: number | null;
    criticalMax: number | null;
    normalString: string | null;
    unit: string;
    targetGender: number | null;
    minAgeDays: number | null;
    maxAgeDays: number | null;
    ageUnit?: 'Days' | 'Months' | 'Years';
    minAgeUI?: number | null;
    maxAgeUI?: number | null;
}


export type ResultStatus = 'normal' | 'warning' | 'danger' | 'unknown';

@Component({
    selector: 'app-lab-catalog',
    standalone: true,
    imports: [CommonModule, FormsModule, LocalizationModule, NgbPaginationModule, NgbDropdownModule, ThemeSharedModule],
    providers: [ListService],
    templateUrl: './lab-catalog.component.html',
    styleUrls: ['./lab-catalog.component.scss']
})
export class LabCatalogComponent implements OnInit {
    labService = inject(LabService);
    list = inject(ListService);
    toaster = inject(ToasterService);
    confirmation = inject(ConfirmationService);

    data: PagedResultDto<LabTestDto> = { items: [], totalCount: 0 };
    allItems: LabTestDto[] = [];   // full list for client-side filter
    filteredItems: LabTestDto[] = [];

    searchText = '';
    private searchSubject = new Subject<string>();

    isModalOpen = false;
    selectedTest: Partial<CreateUpdateLabTestDto> = {};
    selectedId?: string;

    // Dynamic test parameters
    testParameters: TestParameter[] = [];

    ngOnInit() {
        this.list.hookToQuery(query => this.labService.getTests({ ...query, maxResultCount: 1000 } as any)).subscribe(res => {
            this.data = res;
            this.allItems = res.items || [];
            this.applyFilter();
        });

        this.searchSubject.pipe(
            debounceTime(250),
            distinctUntilChanged()
        ).subscribe(() => this.applyFilter());
    }

    onSearchChange() {
        this.searchSubject.next(this.searchText);
    }

    applyFilter() {
        const q = this.searchText.trim().toLowerCase();
        if (!q) {
            this.filteredItems = [...this.allItems];
        } else {
            this.filteredItems = this.allItems.filter(t =>
                (t.name && t.name.toLowerCase().includes(q)) ||
                (t.code && String(t.code).toLowerCase().includes(q))
            );
        }
    }

    createTest() {
        this.selectedTest = { isActive: true };
        this.selectedId = undefined;
        this.testParameters = [];
        this.isModalOpen = true;
    }

    editTest(id: string) {
        const item = this.data.items.find(x => x.id === id);
        if (item) {
            this.selectedTest = {
                code: item.code,
                name: item.name,
                price: item.price,
                instructions: item.instructions,
                referenceRange: item.referenceRange,
                unit: item.unit,
                isActive: item.isActive,
                machine: item.machine,
                turnaroundTime: item.turnaroundTime
            };
            this.selectedId = id;
            this.testParameters = this.parseTestParameters(item.referenceRange);
            this.isModalOpen = true;
        }
    }

    addTestParameter() {
        this.testParameters.push({
            label: '',
            type: 'Numeric',
            operator: 'Between',
            min: null,
            max: null,
            criticalMin: null,
            criticalMax: null,
            normalString: null,
            unit: this.selectedTest.unit || '',
            targetGender: null,
            minAgeDays: null,
            maxAgeDays: null,
            ageUnit: 'Years',
            minAgeUI: null,
            maxAgeUI: null
        });
    }

    onAgeChange(r: TestParameter) {
        if (!r.ageUnit) r.ageUnit = 'Years';
        const multiplier = r.ageUnit === 'Years' ? 365 : (r.ageUnit === 'Months' ? 30 : 1);
        r.minAgeDays = (r.minAgeUI !== null && r.minAgeUI !== undefined) ? r.minAgeUI * multiplier : null;
        r.maxAgeDays = (r.maxAgeUI !== null && r.maxAgeUI !== undefined) ? r.maxAgeUI * multiplier : null;
    }

    removeTestParameter(index: number) {
        this.testParameters.splice(index, 1);
    }

    onTypeChange(r: TestParameter) {
        if (r.type === 'Text') {
            r.operator = 'Text';
            r.min = null; r.max = null; r.criticalMin = null; r.criticalMax = null;
        } else {
            r.operator = 'Between';
            r.normalString = null;
        }
    }

    deleteTest(id: string) {
        this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.labService.deleteTest(id).subscribe(() => {
                    this.toaster.info('::DeletedSuccessfully');
                    this.list.get();
                });
            }
        });
    }

    save() {
        // Serialize testParameters to JSON if any, else keep as text
        if (this.testParameters.length > 0) {
            const valid = this.testParameters.every(r => r.label && r.label.trim() !== '');
            if (!valid) {
                this.toaster.warn('يجب إدخال أسماء جميع المكونات', 'تنبيه');
                return;
            }
            this.selectedTest.referenceRange = JSON.stringify(this.testParameters);
            
            // Map to new backend NormalRanges
            this.selectedTest.normalRanges = this.testParameters.map(p => ({
                targetGender: p.targetGender,
                minAgeDays: p.minAgeDays,
                maxAgeDays: p.maxAgeDays,
                resultType: p.type === 'Text' ? 1 : 0, // 0 = Numeric, 1 = Text
                minValue: p.min,
                maxValue: p.max,
                normalStringValue: p.normalString
            }));
        } else {
            this.selectedTest.referenceRange = null;
            this.selectedTest.normalRanges = [];
        }

        if (this.selectedId) {
            this.labService.updateTest(this.selectedId, this.selectedTest as CreateUpdateLabTestDto).subscribe(() => {
                this.toaster.success('::UpdatedSuccessfully');
                this.isModalOpen = false;
                this.list.get();
            });
        } else {
            this.labService.createTest(this.selectedTest as CreateUpdateLabTestDto).subscribe(() => {
                this.toaster.success('::SavedSuccessfully');
                this.isModalOpen = false;
                this.list.get();
            });
        }
    }

    // --- Helpers ---

    parseTestParameters(raw?: string | null): TestParameter[] {
        if (!raw) return [];
        try {
            const parsed = JSON.parse(raw);
            if (Array.isArray(parsed)) {
                // Map old ReferenceRange format to new TestParameter format for backward compatibility
                return parsed.map((p: any) => {
                    const r = {
                        ...p,
                        targetGender: p.targetGender ?? null,
                        minAgeDays: p.minAgeDays ?? null,
                        maxAgeDays: p.maxAgeDays ?? null,
                        ageUnit: p.ageUnit || 'Years',
                        minAgeUI: p.minAgeUI ?? null,
                        maxAgeUI: p.maxAgeUI ?? null
                    };
                    if (!p.type) {
                        r.type = 'Numeric';
                        r.operator = 'Between';
                        r.normalString = null;
                    }
                    return r;
                });
            }
        } catch {
            // Legacy plain text like "70-100" - migrate to a single range
            const match = raw.match(/^(\d+\.?\d*)\s*[-]\s*(\d+\.?\d*)$/);
            if (match) {
                return [{
                    label: 'المرجع الطبيعي',
                    type: 'Numeric',
                    operator: 'Between',
                    min: parseFloat(match[1]),
                    max: parseFloat(match[2]),
                    criticalMin: null,
                    criticalMax: null,
                    normalString: null,
                    unit: '',
                    targetGender: null, minAgeDays: null, maxAgeDays: null
                }];
            } else {
                return [{
                    label: 'المرجع الطبيعي',
                    type: 'Text',
                    operator: 'Text',
                    min: null, max: null, criticalMin: null, criticalMax: null,
                    normalString: raw,
                    unit: '',
                    targetGender: null, minAgeDays: null, maxAgeDays: null
                }];
            }
        }
        return [];
    }

    getResultStatus(value: number, ranges: TestParameter[]): ResultStatus {
        if (!ranges || ranges.length === 0) return 'unknown';
        // Simplified evaluation
        const r = ranges[0];
        if (r.type === 'Text') return 'normal';
        
        const min = r.min ?? -Infinity;
        const max = r.max ?? Infinity;
        const cMin = r.criticalMin ?? -Infinity;
        const cMax = r.criticalMax ?? Infinity;

        if (value >= min && value <= max) return 'normal';
        if (value >= cMin && value <= cMax) return 'warning';
        return 'danger';
    }

    getStatusColor(status: ResultStatus): string {
        switch (status) {
            case 'normal':  return '#28a745';
            case 'warning': return '#fd7e14';
            case 'danger':  return '#dc3545';
            default:        return '#6c757d';
        }
    }

    getRangeDisplay(raw?: string | null): string {
        if (!raw) return '-';
        const params = this.parseTestParameters(raw);
        if (params.length === 0) return raw;
        return params.map(r => {
            const parts = [r.label];
            if (r.type === 'Text') {
                if (r.normalString) parts.push(r.normalString);
            } else {
                if (r.operator === 'Between' && r.min !== null && r.max !== null) parts.push(`${r.min}-${r.max}`);
                else if (r.operator === 'LessThan' && r.max !== null) parts.push(`< ${r.max}`);
                else if (r.operator === 'GreaterThan' && r.min !== null) parts.push(`> ${r.min}`);
            }
            if (r.unit) parts.push(r.unit);
            return parts.join(' ');
        }).join(' | ');
    }

    hasStructuredRanges(raw?: string | null): boolean {
        if (!raw) return false;
        try { const p = JSON.parse(raw); return Array.isArray(p) && p.length > 0; }
        catch { return false; }
    }
}
