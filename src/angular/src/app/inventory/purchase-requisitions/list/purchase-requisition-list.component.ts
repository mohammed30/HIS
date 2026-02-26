import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { PurchaseRequisitionService, Dtos } from '../../../proxy/inventory';
type PurchaseRequisitionDto = Dtos.PurchaseRequisitionDto;

@Component({
  selector: 'app-purchase-requisition-list',
  standalone: true,
  imports: [CommonModule, RouterModule, ThemeSharedModule],
  template: `
    <div class="card shadow-sm">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h5 class="m-0"><i class="fas fa-file-invoice me-2"></i>Purchase Requisitions</h5>
        <button class="btn btn-primary" routerLink="create">
            <i class="fas fa-plus me-1"></i> New Request
        </button>
      </div>
      <div class="card-body">
        <div class="table-responsive">
          <table class="table table-hover table-premium">
            <thead>
              <tr>
                <th>Number</th>
                <th>Dept</th>
                <th>Requestor</th>
                <th>Required Date</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let req of requisitions">
                <td>{{ req.requisitionNumber }}</td>
                <td>{{ req.departmentName }}</td>
                <td>{{ req.requestorName }}</td>
                <td>{{ req.requiredDate | date }}</td>
                <td>
                  <span class="badge" [ngClass]="{
                    'bg-secondary': req.status === 0,
                    'bg-warning text-dark': req.status === 1,
                    'bg-success': req.status === 2,
                    'bg-danger': req.status === 3,
                    'bg-info': req.status === 4
                  }">
                    {{ req.status === 0 ? 'Draft' : req.status === 1 ? 'Pending' : req.status === 2 ? 'Approved' : req.status === 3 ? 'Rejected' : 'Converted to PO' }}
                  </span>
                </td>
                <td>
                  <button class="btn btn-sm btn-outline-primary" [routerLink]="['edit', req.id]">
                    <i class="fas fa-edit"></i>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class PurchaseRequisitionListComponent implements OnInit {
  private service = inject(PurchaseRequisitionService);
  requisitions: PurchaseRequisitionDto[] = [];

  ngOnInit() {
    this.loadList();
  }

  loadList() {
    this.service.getList({ maxResultCount: 1000 }).subscribe(res => {
      this.requisitions = res.items;
    });
  }
}
