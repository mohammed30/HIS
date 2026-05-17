import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RadiologyRequestDto } from '../../proxy/radiology/models';
import { RadiologyRequestStatus } from '../../proxy/radiology/radiology-request-status.enum';
import { RadiologyService } from '../../proxy/radiology/radiology.service';
import { ConfirmationService, ThemeSharedModule } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';

@Component({
  selector: 'app-radiology-requests',
  templateUrl: './radiology-requests.component.html',
  standalone: true,
  imports: [
    CommonModule,
    CoreModule,
    ThemeSharedModule,
    ReactiveFormsModule,
    NgxDatatableModule
  ],
  providers: [ListService],
})
export class RadiologyRequestsComponent implements OnInit {
  items: PagedResultDto<RadiologyRequestDto> = { items: [], totalCount: 0 };
  
  isModalOpen = false;
  form: FormGroup;
  selectedItem = {} as RadiologyRequestDto;

  statusEnum = RadiologyRequestStatus;
  
  // Filters
  filterText = '';
  selectedStatus: number | null = null;

  constructor(
    public readonly list: ListService,
    private radiologyService: RadiologyService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) {}

  ngOnInit() {
    const streamCreator = (query) => this.radiologyService.getList({
      ...query,
      filter: this.filterText,
      status: this.selectedStatus as any
    });

    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.items = response;
    });
  }

  changeStatus(status: number | null) {
    this.selectedStatus = status;
    this.list.get();
  }

  search() {
    this.list.get();
  }

  refresh() {
    this.list.get();
  }

  edit(id: string) {
    this.radiologyService.get(id).subscribe((res) => {
      this.selectedItem = res;
      this.buildForm();
      this.isModalOpen = true;
    });
  }

  buildForm() {
    this.form = this.fb.group({
      reportBody: [this.selectedItem.reportBody || '', Validators.required],
      technicianNotes: [this.selectedItem.technicianNotes || ''],
      status: [this.selectedItem.status === undefined ? RadiologyRequestStatus.Requested : this.selectedItem.status, Validators.required],
    });
  }

  save() {
    if (this.form.invalid) return;

    const request = this.selectedItem.id
      ? this.radiologyService.update(this.selectedItem.id, this.form.value)
      : this.radiologyService.create(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.form.reset();
      this.list.get();
    });
  }

  printResult(id: string) {
    this.radiologyService.getRadiologyResultPdf(id).subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `Radiology_Report_${id.substring(0, 8)}.pdf`;
      link.click();
      window.URL.revokeObjectURL(url);
    });
  }

  getStatusText(status: number) {
    switch (status) {
      case RadiologyRequestStatus.Requested: return 'مطلوب';
      case RadiologyRequestStatus.UnderProcedure: return 'قيد التنفيذ';
      case RadiologyRequestStatus.Reported: return 'تم التقرير';
      case RadiologyRequestStatus.Cancelled: return 'ملغي';
      default: return 'غير معروف';
    }
  }
}
