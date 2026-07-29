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

export interface ReferenceRange {
    label: string;
    min: number | null;
    max: number | null;
    criticalMin: number | null;
    criticalMax: number | null;
    unit: string;
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

    // Dynamic reference ranges
    referenceRanges: ReferenceRange[] = [];

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
        this.referenceRanges = [];
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
                isActive: item.isActive
            };
            this.selectedId = id;
            this.referenceRanges = this.parseReferenceRanges(item.referenceRange);
            this.isModalOpen = true;
        }
    }

    addReferenceRange() {
        this.referenceRanges.push({
            label: '',
            min: null,
            max: null,
            criticalMin: null,
            criticalMax: null,
            unit: this.selectedTest.unit || ''
        });
    }

    removeReferenceRange(index: number) {
        this.referenceRanges.splice(index, 1);
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
        // Serialize referenceRanges to JSON if any, else keep as text
        if (this.referenceRanges.length > 0) {
            const valid = this.referenceRanges.every(r => r.label.trim() !== '');
            if (!valid) {
                this.toaster.warn('يجب إدخال تسمية لكل مرجع', 'تنبيه');
                return;
            }
            this.selectedTest.referenceRange = JSON.stringify(this.referenceRanges);
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

    // ─── Helpers ────────────────────────────────────────────────────────────────

    parseReferenceRanges(raw?: string | null): ReferenceRange[] {
        if (!raw) return [];
        try {
            const parsed = JSON.parse(raw);
            if (Array.isArray(parsed)) return parsed as ReferenceRange[];
        } catch {
            // Legacy plain text like "70-100" — migrate to a single range
            const match = raw.match(/^(\d+\.?\d*)\s*[-–]\s*(\d+\.?\d*)$/);
            if (match) {
                return [{
                    label: 'المرجع الطبيعي',
                    min: parseFloat(match[1]),
                    max: parseFloat(match[2]),
                    criticalMin: null,
                    criticalMax: null,
                    unit: ''
                }];
            }
        }
        return [];
    }

    getResultStatus(value: number, ranges: ReferenceRange[]): ResultStatus {
        if (!ranges || ranges.length === 0) return 'unknown';
        // Use the first range for simple evaluation
        const r = ranges[0];
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
        if (!raw) return '—';
        const ranges = this.parseReferenceRanges(raw);
        if (ranges.length === 0) return raw;
        return ranges.map(r => {
            const parts = [r.label];
            if (r.min !== null && r.max !== null) parts.push(`${r.min}–${r.max}`);
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
