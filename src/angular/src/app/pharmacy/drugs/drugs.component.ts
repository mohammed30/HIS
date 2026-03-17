import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { PharmacyService } from '../pharmacy.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DrugDialogComponent } from './drug-dialog/drug-dialog.component';
import { CommonModule } from '@angular/common';
import { ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-drugs',
  standalone: true,
  imports: [CommonModule, ThemeSharedModule, NgxDatatableModule, CoreModule],
  providers: [ListService],
  template: `
    <div class="card">
      <div class="card-header">
        <div class="row align-items-center">
          <div class="col col-md-6">
            <h5 class="card-title mb-0">
              <i class="fas fa-pills me-2"></i> {{ '::DrugMasterData' | abpLocalization }}
            </h5>
          </div>
          <div class="text-end col col-md-6">
            <button class="btn btn-success me-1" (click)="downloadTemplate()">
              <i class="fa fa-download me-1"></i> {{ '::DownloadTemplate' | abpLocalization }}
            </button>
            <button class="btn btn-warning me-1" (click)="fileInput.click()">
              <i class="fa fa-file-excel me-1"></i> {{ '::ImportFromExcel' | abpLocalization }}
            </button>
            <button class="btn btn-primary" (click)="createDrug()">
              <i class="fa fa-plus me-1"></i> {{ '::NewDrug' | abpLocalization }}
            </button>
            <input type="file" #fileInput accept=".xlsx,.xls" (change)="onFileSelected($event)" style="display:none" />
          </div>
        </div>
      </div>
      <div class="card-body">
        <!-- Search Input -->
        <div class="row mb-3">
          <div class="col-md-6">
            <div class="input-group">
              <span class="input-group-text"><i class="fas fa-search"></i></span>
              <input type="text" class="form-control" [placeholder]="'::Search' | abpLocalization"
                     [(ngModel)]="searchText" (keyup.enter)="list.get()" (ngModelChange)="list.get()">
            </div>
          </div>
        </div>

        <ngx-datatable [rows]="book.items" [count]="book.totalCount" [list]="list" default class="material" [footerHeight]="50">
          
          <ngx-datatable-column [name]="'::Barcode' | abpLocalization" prop="barcode">
             <ng-template let-row="row" ngx-datatable-cell-template>
                <code class="text-primary">{{ row.barcode }}</code>
             </ng-template>
          </ngx-datatable-column>
          
          <ngx-datatable-column [name]="'::BrandName' | abpLocalization" prop="brandName"></ngx-datatable-column>
          <ngx-datatable-column [name]="'::ScientificName' | abpLocalization" prop="scientificName"></ngx-datatable-column>
          
          <ngx-datatable-column [name]="'::Strength' | abpLocalization" prop="strength">
             <ng-template let-row="row" ngx-datatable-cell-template>
                <span class="badge bg-light text-dark border">{{ row.strength }}</span>
             </ng-template>
          </ngx-datatable-column>
 
          <ngx-datatable-column [name]="'::Form' | abpLocalization" prop="form">
            <ng-template let-row="row" ngx-datatable-cell-template>
               {{ '::' + row.form | abpLocalization }}
            </ng-template>
          </ngx-datatable-column>
          
          <ngx-datatable-column [name]="'::Bin' | abpLocalization" prop="binLocation" [width]="80"></ngx-datatable-column>
          
          <ngx-datatable-column [name]="'::StockParams' | abpLocalization" [width]="150">
             <ng-template let-row="row" ngx-datatable-cell-template>
                <small>Min: {{ row.minimumStockLevel }} | Re: {{ row.reorderLevel }}</small>
             </ng-template>
          </ngx-datatable-column>
          
          <ngx-datatable-column [name]="'::Manufacturer' | abpLocalization" prop="manufacturer"></ngx-datatable-column>
 
          <ngx-datatable-column [name]="'::Actions' | abpLocalization" sortable="false" [width]="120">
            <ng-template let-row="row" ngx-datatable-cell-template>
              <div class="btn-group">
                <button class="btn btn-sm btn-outline-primary" (click)="editDrug(row.id)" [title]="'::Edit' | abpLocalization">
                  <i class="fas fa-pencil-alt"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger ms-1" (click)="deleteDrug(row.id)" [title]="'::Delete' | abpLocalization">
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </ng-template>
          </ngx-datatable-column>
        </ngx-datatable>
        
        <!-- Total Count Badge -->
        <div class="mt-2">
            <span class="badge bg-secondary p-2">
                {{ '::TotalRecords' | abpLocalization }}: {{ book.totalCount }}
            </span>
        </div>
      </div>
    </div>
  `
})
export class DrugsComponent implements OnInit {
  book = { items: [], totalCount: 0 } as PagedResultDto<any>;
  searchText = '';
  importing = false;

  constructor(
    public readonly list: ListService,
    private pharmacyService: PharmacyService,
    private modalService: NgbModal,
    private confirmation: ConfirmationService,
    private toaster: ToasterService
  ) { }

  ngOnInit() {
    this.list.hookToQuery(query => this.pharmacyService.getDrugs({ ...query, searchText: this.searchText })).subscribe(res => {
      this.book = res;
    });
  }

  createDrug() {
    const modal = this.modalService.open(DrugDialogComponent, { size: 'lg' });
    modal.result.then((res) => {
      if (res) this.list.get();
    }, () => { });
  }

  editDrug(id: string) {
    const modal = this.modalService.open(DrugDialogComponent, { size: 'lg' });
    modal.componentInstance.id = id; // Pass ID to dialog
    modal.result.then((res) => {
      if (res) this.list.get();
    }, () => { });
  }

  deleteDrug(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.pharmacyService.deleteDrug(id).subscribe(() => {
          this.list.get();
        });
      }
    });
  }

  downloadTemplate() {
    this.pharmacyService.downloadDrugTemplate().subscribe((blob) => {
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = 'DrugImportTemplate.xlsx';
      link.click();
      window.URL.revokeObjectURL(url);
    });
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.importing = true;
      this.pharmacyService.importDrugsFromExcel(file).subscribe({
        next: () => {
          this.importing = false;
          this.toaster.success('::ImportSuccess');
          this.list.get();
        },
        error: (err) => {
          this.importing = false;
          this.toaster.error('::ImportError');
          console.error(err);
        }
      });
    }
  }
}
