import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LabService } from '../../proxy/laboratory/lab.service';
import { LabTestDto, CreateUpdateLabTestDto } from '../../proxy/laboratory/dtos/models';
import { ListService, PagedResultDto } from '@abp/ng.core';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
import { LocalizationModule } from '@abp/ng.core';

@Component({
    selector: 'app-lab-catalog',
    standalone: true,
    imports: [CommonModule, FormsModule, LocalizationModule, NgbPaginationModule, ThemeSharedModule],
    providers: [ListService],
    templateUrl: './lab-catalog.component.html'
})
export class LabCatalogComponent implements OnInit {
    labService = inject(LabService);
    list = inject(ListService);
    toaster = inject(ToasterService);

    data: PagedResultDto<LabTestDto> = { items: [], totalCount: 0 };

    isModalOpen = false;
    selectedTest: Partial<CreateUpdateLabTestDto> = {};
    selectedId?: string;

    ngOnInit() {
        this.list.hookToQuery(query => this.labService.getTests(query)).subscribe(res => {
            this.data = res;
        });
    }

    createTest() {
        this.selectedTest = { isActive: true };
        this.selectedId = undefined;
        this.isModalOpen = true;
    }

    editTest(id: string) {
        this.labService.getTests({ maxResultCount: 1 }).subscribe(() => {
            // ideally fetch single, but utilizing getTests for MVP or assuming we have data
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
                this.isModalOpen = true;
            }
        });
    }

    deleteTest(id: string) {
        if (confirm('Are you sure?')) {
            this.labService.deleteTest(id).subscribe(() => {
                this.toaster.info('::DeletedSuccessfully');
                this.list.get();
            });
        }
    }

    save() {
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
}
