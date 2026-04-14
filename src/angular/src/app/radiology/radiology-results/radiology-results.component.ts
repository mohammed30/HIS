import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { RadiologyRequestDto, RadiologyRequestStatus } from '../../proxy/radiology/models';
import { RadiologyService } from '../../proxy/radiology/radiology.service';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { FormsModule } from '@angular/forms';
import { PageModule } from '@abp/ng.components/page';

@Component({
  selector: 'app-radiology-results',
  standalone: true,
  imports: [
    CommonModule,
    CoreModule,
    ThemeSharedModule,
    NgxDatatableModule,
    FormsModule,
    PageModule
  ],
  providers: [ListService],
  template: `
    <abp-page [title]="'::RadiologyArchives' | abpLocalization">
      <abp-page-toolbar-container class="col">
        <!-- Optional: Add Print or Export buttons here -->
      </abp-page-toolbar-container>

      <div class="card">
        <div class="card-header">
          <div class="row">
            <div class="col col-md-4">
              <div class="input-group">
                <span class="input-group-text"><i class="fas fa-search"></i></span>
                <input
                  type="text"
                  class="form-control"
                  [placeholder]="'::SearchByPatientName' | abpLocalization"
                  [(ngModel)]="list.filter"
                  (ngModelChange)="list.get()"
                />
              </div>
            </div>
          </div>
        </div>
        <div class="card-body">
          <ngx-datatable [rows]="items.items" [count]="items.totalCount" [list]="list" default>
            <ngx-datatable-column
              [name]="'::Actions' | abpLocalization"
              [maxWidth]="150"
              [sortable]="false"
            >
              <ng-template let-row="row" ngx-datatable-cell-template>
                <button
                  class="btn btn-primary btn-sm rounded-pill"
                  (click)="viewReport(row)"
                  *ngIf="row.status === statusEnum.Reported"
                >
                  <i class="fas fa-eye me-1"></i> {{ '::ViewReport' | abpLocalization }}
                </button>
              </ng-template>
            </ngx-datatable-column>
            
            <ngx-datatable-column [name]="'::PatientName' | abpLocalization" prop="patientName"></ngx-datatable-column>
            <ngx-datatable-column [name]="'::ExamName' | abpLocalization" prop="radiologyItemName"></ngx-datatable-column>
            <ngx-datatable-column [name]="'::Date' | abpLocalization" prop="requestDate">
              <ng-template let-row="row" ngx-datatable-cell-template>
                {{ row.requestDate | date }}
              </ng-template>
            </ngx-datatable-column>
            <ngx-datatable-column [name]="'::Status' | abpLocalization" prop="status">
              <ng-template let-row="row" ngx-datatable-cell-template>
                <span class="badge" [ngClass]="{
                  'bg-warning text-dark': row.status === 0,
                  'bg-info': row.status === 1,
                  'bg-success': row.status === 2,
                  'bg-danger': row.status === 3
                }">
                  {{ getStatusText(row.status) }}
                </span>
              </ng-template>
            </ngx-datatable-column>
          </ngx-datatable>
        </div>
      </div>

      <!-- Result Modal -->
      <abp-modal [(visible)]="isModalOpen" [busy]="isModalBusy">
        <ng-template #abpHeader>
          <h3>{{ '::RadiologyReport' | abpLocalization }}</h3>
        </ng-template>

        <ng-template #abpBody>
            <div class="p-3 border rounded bg-light mb-3">
                <div class="row">
                    <div class="col-md-6"><strong>{{ '::Patient' | abpLocalization }}:</strong> {{ selectedItem.patientName }}</div>
                    <div class="col-md-6 text-end"><strong>{{ '::Date' | abpLocalization }}:</strong> {{ selectedItem.reportDate | date }}</div>
                </div>
            </div>
            
            <div class="medical-report-content" style="white-space: pre-wrap; font-family: 'Courier New', Courier, monospace; min-height: 200px; padding: 15px; border: 1px solid #ddd; background: #fff;">
                {{ selectedItem.reportBody }}
            </div>
            
            <div class="mt-3 text-muted" *ngIf="selectedItem.technicianNotes">
                <small><strong>{{ '::TechnicianNotes' | abpLocalization }}:</strong> {{ selectedItem.technicianNotes }}</small>
            </div>
        </ng-template>

        <ng-template #abpFooter>
          <button type="button" class="btn btn-secondary" abpClose>
            {{ '::Close' | abpLocalization }}
          </button>
          <button type="button" class="btn btn-primary" (click)="printReport()">
            <i class="fas fa-print me-1"></i> {{ '::Print' | abpLocalization }}
          </button>
        </ng-template>
      </abp-modal>
    </abp-page>
  `,
})
export class RadiologyResultsComponent implements OnInit {
  items: PagedResultDto<RadiologyRequestDto> = { items: [], totalCount: 0 };
  isModalOpen = false;
  isModalBusy = false;
  selectedItem = {} as RadiologyRequestDto;
  statusEnum = RadiologyRequestStatus;

  constructor(
    public readonly list: ListService,
    private radiologyService: RadiologyService
  ) {}

  ngOnInit() {
    const streamCreator = (query) => this.radiologyService.getList(query);

    this.list.hookToQuery(streamCreator).subscribe((response) => {
      this.items = response;
    });
  }

  viewReport(row: RadiologyRequestDto) {
    this.selectedItem = row;
    this.isModalOpen = true;
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

  printReport() {
    window.print();
  }
}
