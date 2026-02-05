import { ListService, PagedResultDto, CoreModule } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { ServiceItemService } from '../../proxy/services/service-item.service';
import { ServiceItemDto, ServiceCategory } from '../../proxy/services';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

@Component({
  selector: 'app-services',
  templateUrl: './services.html',
  styleUrls: ['./services.scss'],
  standalone: true,
  imports: [CommonModule, CoreModule, ThemeSharedModule, ReactiveFormsModule, FormsModule],
  providers: [ListService],
})
export class ServicesComponent implements OnInit {
  items: ServiceItemDto[] = [];
  totalCount = 0;

  isModalOpen = false;
  form: FormGroup;
  selectedItem: ServiceItemDto = {} as ServiceItemDto;

  categories = Object.keys(ServiceCategory)
    .filter(k => !isNaN(Number(k)))
    .map(k => ({ key: Number(k), value: ServiceCategory[k as any] }));

  constructor(
    public readonly list: ListService,
    private serviceItemService: ServiceItemService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    this.list.hookToQuery(query => this.serviceItemService.getList(query)).subscribe(res => {
      this.items = res.items;
      this.totalCount = res.totalCount;
    });
  }

  create() {
    this.selectedItem = {} as ServiceItemDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  edit(id: string) {
    const item = this.items.find(x => x.id === id);
    this.selectedItem = item || {} as ServiceItemDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  buildForm() {
    this.form = this.fb.group({
      code: [this.selectedItem.code || ''], // Not required, will be auto-generated
      name: [this.selectedItem.name || '', [Validators.required]],
      category: [this.selectedItem.category !== undefined ? this.selectedItem.category : null, [Validators.required]],
      price: [this.selectedItem.price || 0],
      unit: [this.selectedItem.unit || ''],
      referenceRange: [this.selectedItem.referenceRange || ''],
      instructions: [this.selectedItem.instructions || ''],
      isActive: [this.selectedItem.isActive !== false], // Default true
    });
  }

  save() {
    if (this.form.invalid) return;

    const data = {
      ...this.form.value,
      category: parseInt(this.form.value.category, 10),
      price: parseFloat(this.form.value.price || 0)
    };

    const request = this.selectedItem.id
      ? this.serviceItemService.update(this.selectedItem.id, data)
      : this.serviceItemService.create(data);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.list.get();
    });
  }

  delete(id: string) {
    this.confirmation.warn('Are you sure you want to delete this service?', 'Confirm Delete').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.serviceItemService.delete(id).subscribe(() => this.list.get());
      }
    });
  }

  getCategoryName(category: number): string {
    const names: { [key: number]: string } = {
      0: 'استشارة',
      1: 'إجراء',
      2: 'تحليل مخبري',
      3: 'أشعة',
      4: 'عملية',
      5: 'أخرى'
    };
    return names[category] || 'غير محدد';
  }
}
