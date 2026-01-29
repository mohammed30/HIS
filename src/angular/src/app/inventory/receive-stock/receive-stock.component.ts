import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RestService } from '@abp/ng.core';
import { LocalizationModule } from '@abp/ng.core';
import { Router } from '@angular/router';
import { WarehouseDto } from '../../proxy/inventory/models';
import { ToasterService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-receive-stock',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LocalizationModule],
  templateUrl: './receive-stock.component.html',
  styleUrls: ['./receive-stock.scss']
})
export class ReceiveStockComponent implements OnInit {
  form: FormGroup;
  warehouses: WarehouseDto[] = [];
  itemTypes = [
    { value: 0, label: '::Enum:Medication' },
    { value: 1, label: '::Enum:Consumable' },
    { value: 2, label: '::Enum:Asset' },
    { value: 3, label: '::Enum:Reagent' },
    { value: 4, label: '::Enum:Other' }
  ];

  constructor(
    private fb: FormBuilder,
    private restService: RestService,
    private router: Router,
    private toaster: ToasterService
  ) { }

  ngOnInit() {
    this.buildForm();
    this.loadWarehouses();
  }

  buildForm() {
    this.form = this.fb.group({
      warehouseId: [null, Validators.required],
      productName: ['', Validators.required],
      productId: [this.generateGuid()], // Auto-generate ID if not selecting from master
      type: [0, Validators.required],
      quantity: [null, [Validators.required, Validators.min(0.01)]],
      unitCost: [null, [Validators.required, Validators.min(0)]],
      referenceNumber: ['', Validators.required]
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

  save() {
    if (this.form.invalid) return;

    this.restService.request({
      method: 'POST',
      url: '/api/app/inventory/receive-stock',
      body: this.form.value
    }).subscribe(() => {
      this.toaster.success('::StockReceivedSuccessfully');
      this.router.navigate(['/inventory/dashboard']);
    });
  }

  // Temporary helper until we have a proper Product Selection
  generateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}
