import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { ServiceItemService, ServiceItemDto, ServiceCategory } from '../../proxy/services/service-item.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common'; // Important for structural directives like *ngIf
import { ReactiveFormsModule, FormsModule } from '@angular/forms';

@Component({
  selector: 'app-services',
  templateUrl: './services.html',
  styleUrls: ['./services.scss'],
  standalone: true,
  imports: [CommonModule, ThemeSharedModule, ReactiveFormsModule, FormsModule],
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
    .map(k => ({ key: k, value: ServiceCategory[k] }));

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
      code: [this.selectedItem.code || '', [Validators.required]],
      name: [this.selectedItem.name || '', [Validators.required]],
      category: [this.selectedItem.category || null, [Validators.required]],
      isActive: [this.selectedItem.isActive !== false], // Default true
    });
  }

  save() {
    if (this.form.invalid) return;

    const request = this.selectedItem.id
      ? this.serviceItemService.update(this.selectedItem.id, this.form.value)
      : this.serviceItemService.create(this.form.value);

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
}
