import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RestService } from '@abp/ng.core';
import { LocalizationModule } from '@abp/ng.core';
import { Router } from '@angular/router';
import { WarehouseDto, InventoryItemDto } from '../../proxy/inventory/models';
import { DepartmentDto } from '../../proxy/settings/models';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-issue-stock',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LocalizationModule],
  templateUrl: './issue-stock.component.html',
  styleUrls: ['./issue-stock.scss']
})
export class IssueStockComponent implements OnInit {
  form: FormGroup;
  warehouses: WarehouseDto[] = [];
  departments: DepartmentDto[] = [];
  stockItems: InventoryItemDto[] = [];

  constructor(
    private fb: FormBuilder,
    private restService: RestService,
    private router: Router,
    private toaster: ToasterService
  ) { }

  ngOnInit() {
    this.buildForm();
    this.loadWarehouses();
    this.loadDepartments();
  }

  buildForm() {
    this.form = this.fb.group({
      warehouseId: [null, Validators.required],
      productId: [null, Validators.required],
      departmentId: [null, Validators.required], // Required for "Departmental Consumption"
      quantity: [null, [Validators.required, Validators.min(0.01)]],
      referenceNumber: ['', Validators.required]
    });

    this.form.get('warehouseId').valueChanges.subscribe(val => {
      if (val) {
        this.loadStockItems(val);
      } else {
        this.stockItems = [];
      }
    });

    // Optional: Max quantity validation based on selected product
    this.form.get('productId').valueChanges.subscribe(val => {
      const item = this.stockItems.find(x => x.productId === val);
      if (item) {
        this.form.get('quantity').setValidators([Validators.required, Validators.min(0.01), Validators.max(item.quantity)]);
        this.form.get('quantity').updateValueAndValidity();
      }
    });
  }

  loadWarehouses() {
    this.restService.request<void, { items: WarehouseDto[] }>({
      method: 'GET',
      url: '/api/app/inventory/warehouse'
    }).subscribe(res => {
      this.warehouses = res.items;
    });
  }

  loadDepartments() {
    this.restService.request<void, { items: DepartmentDto[] }>({
      method: 'GET',
      url: '/api/app/department'
    }).subscribe(res => {
      this.departments = res.items;
    });
  }

  loadStockItems(warehouseId: string) {
    this.restService.request<void, { items: InventoryItemDto[] }>({
      method: 'GET',
      url: `/api/app/inventory/stock-levels?warehouseId=${warehouseId}` // Ensure this endpoint exists and works
    }).subscribe(res => {
      this.stockItems = res.items;
    });
  }

  save() {
    if (this.form.invalid) return;

    this.restService.request({
      method: 'POST',
      url: '/api/app/inventory/issue-stock',
      body: this.form.value
    }).subscribe(() => {
      this.toaster.success('::StockIssuedSuccessfully');
      this.router.navigate(['/inventory/dashboard']);
    });
  }
}
