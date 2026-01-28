import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { PriceListService, PriceListDto, ServicePriceDto } from '../../proxy/pricing/price-list.service';
import { ServiceItemService, ServiceItemDto } from '../../proxy/services/service-item.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

@Component({
  selector: 'app-price-lists',
  templateUrl: './price-lists.html',
  styleUrls: ['./price-lists.scss'],
  standalone: true,
  imports: [CommonModule, ThemeSharedModule, ReactiveFormsModule, FormsModule],
  providers: [ListService],
})
export class PriceListsComponent implements OnInit {
  items: PriceListDto[] = [];
  totalCount = 0;

  isModalOpen = false;
  form: FormGroup;
  selectedItem: PriceListDto = {} as PriceListDto;

  // Price Management
  isPricesModalOpen = false;
  currentPriceListId = '';
  servicePrices: ServicePriceDto[] = [];
  serviceItems: ServiceItemDto[] = [];
  priceForm: FormGroup;

  constructor(
    public readonly list: ListService,
    private priceListService: PriceListService,
    private serviceItemService: ServiceItemService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    this.list.hookToQuery(query => this.priceListService.getList(query)).subscribe(res => {
      this.items = res.items;
      this.totalCount = res.totalCount;
    });
  }

  // --- Master List CRUD ---

  create() {
    this.selectedItem = {} as PriceListDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  edit(id: string) {
    const item = this.items.find(x => x.id === id);
    this.selectedItem = item || {} as PriceListDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  buildForm() {
    this.form = this.fb.group({
      name: [this.selectedItem.name || '', [Validators.required]],
      isDefault: [this.selectedItem.isDefault || false],
      effectiveFrom: [this.selectedItem.effectiveFrom || null, [Validators.required]],
      effectiveTo: [this.selectedItem.effectiveTo || null],
    });
  }

  save() {
    if (this.form.invalid) return;

    const request = this.selectedItem.id
      ? this.priceListService.update(this.selectedItem.id, this.form.value)
      : this.priceListService.create(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.list.get();
    });
  }

  delete(id: string) {
    this.confirmation.warn('Are you sure?', 'Confirm Delete').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.priceListService.delete(id).subscribe(() => this.list.get());
      }
    });
  }

  // --- Manage Prices ---

  managePrices(id: string) {
    this.currentPriceListId = id;
    this.isPricesModalOpen = true;
    this.loadServiceItemsAndPrices();
    this.buildPriceForm();
  }

  loadServiceItemsAndPrices() {
    // 1. Get all services (simplified, normally paged)
    this.serviceItemService.getList({ maxResultCount: 1000 }).subscribe(res => {
      this.serviceItems = res.items;
    });
    // 2. Get existing prices
    this.priceListService.getPrices(this.currentPriceListId, { maxResultCount: 1000 }).subscribe(res => {
      this.servicePrices = res.items;
    });
  }

  buildPriceForm() {
    // Simple form for adding/updating a single price
    this.priceForm = this.fb.group({
      serviceItemId: [null, [Validators.required]],
      amount: [0, [Validators.required, Validators.min(0)]],
      coPayAmount: [0]
    });
  }

  getPrice(serviceId: string): ServicePriceDto | undefined {
    return this.servicePrices.find(p => p.serviceItemId === serviceId);
  }

  savePrice() {
    if (this.priceForm.invalid) return;

    const input = {
      priceListId: this.currentPriceListId,
      ...this.priceForm.value
    };

    this.priceListService.setPrice(input).subscribe(newPrice => {
      // Update local list
      const idx = this.servicePrices.findIndex(p => p.serviceItemId === newPrice.serviceItemId);
      if (idx > -1) this.servicePrices[idx] = newPrice;
      else this.servicePrices.push(newPrice);

      this.priceForm.reset({ amount: 0, coPayAmount: 0 }); // Reset but keep list open
    });
  }

  getServiceItem(id: string): ServiceItemDto | undefined {
    return this.serviceItems.find(x => x.id === id);
  }
}
