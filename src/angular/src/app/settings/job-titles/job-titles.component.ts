import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { JobTitleService } from '../../proxy/settings/job-title.service';
import { JobTitleDto, CreateUpdateJobTitleDto } from '../../proxy/settings/dtos/job-title-dto';
import { DepartmentService } from '../../proxy/settings/department.service';
import { LookupDto } from '../../proxy/settings/models';

@Component({
    selector: 'app-job-titles',
    standalone: true,
    imports: [CommonModule, FormsModule, NgbPaginationModule, ThemeSharedModule],
    templateUrl: './job-titles.component.html'
})
export class JobTitlesComponent implements OnInit {
    private service = inject(JobTitleService);
    private deptService = inject(DepartmentService);
    private confirmation = inject(ConfirmationService);

    items: JobTitleDto[] = [];
    departments: LookupDto[] = []; // simple list for dropdown

    totalCount = 0;
    page = 1;
    pageSize = 10;

    showForm = false;
    isSaving = false;
    editingItem: JobTitleDto | null = null;
    formData: CreateUpdateJobTitleDto = { nameAr: '', nameEn: '', description: '', departmentId: null };

    ngOnInit() {
        this.loadData();
        this.loadDepartments();
    }

    loadData() {
        const skipCount = (this.page - 1) * this.pageSize;
        this.service.getList({ skipCount, maxResultCount: this.pageSize, sorting: 'nameAr asc' }).subscribe({
            next: (res) => {
                this.items = res.items;
                this.totalCount = res.totalCount;
            },
            error: (err) => console.error(err)
        });
    }

    loadDepartments() {
        this.deptService.getLookup().subscribe(res => {
            this.departments = res;
        });
    }

    onPageChange(page: number) {
        this.page = page;
        this.loadData();
    }

    create() {
        this.editingItem = null;
        this.formData = { nameAr: '', nameEn: '', description: '', departmentId: null };
        this.showForm = true;
    }

    edit(item: JobTitleDto) {
        this.editingItem = item;
        this.formData = {
            nameAr: item.nameAr,
            nameEn: item.nameEn,
            description: item.description,
            departmentId: item.departmentId
        };
        this.showForm = true;
    }

    save() {
        this.isSaving = true;
        const req = this.editingItem
            ? this.service.update(this.editingItem.id, this.formData)
            : this.service.create(this.formData);

        req.subscribe({
            next: () => {
                this.isSaving = false;
                this.showForm = false;
                this.loadData();
            },
            error: (err) => {
                this.isSaving = false;
                console.error(err);
            }
        });
    }

    delete(id: string) {
        this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.service.delete(id).subscribe(() => this.loadData());
            }
        });
    }
}
