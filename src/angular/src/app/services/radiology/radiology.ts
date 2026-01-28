import { ListService, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { ServiceItemService, RadiologyItemDto } from '../../proxy/services/service-item.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-radiology',
  templateUrl: './radiology.html',
  styleUrls: ['./radiology.scss'],
  providers: [ListService],
})
export class RadiologyComponent implements OnInit {
  items: RadiologyItemDto[] = [];
  totalCount = 0;

  isModalOpen = false;
  form: FormGroup;
  selectedItem: RadiologyItemDto = {} as RadiologyItemDto;

  modalities = ['X-Ray', 'CT Scan', 'MRI', 'Ultrasound', 'Fluoroscopy', 'Mammography', 'Nuclear Medicine'];

  constructor(
    public readonly list: ListService,
    private serviceItemService: ServiceItemService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    this.list.hookToQuery(query => this.serviceItemService.getRadiologyList(query)).subscribe(res => {
      this.items = res.items;
      this.totalCount = res.totalCount;
    });
  }

  create() {
    this.selectedItem = {} as RadiologyItemDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  edit(id: string) {
    const item = this.items.find(x => x.id === id);
    this.selectedItem = item || {} as RadiologyItemDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  buildForm() {
    this.form = this.fb.group({
      code: [this.selectedItem.code || '', [Validators.required]],
      name: [this.selectedItem.name || '', [Validators.required]],
      modality: [this.selectedItem.modality || null, [Validators.required]],
      bodyPart: [this.selectedItem.bodyPart || '', [Validators.required]],
      instructions: [this.selectedItem.instructions || ''],
      isActive: [this.selectedItem.isActive !== false],
      // Fixed values
      category: [3], // Radiology enum value
    });
  }

  save() {
    if (this.form.invalid) return;

    const request = this.selectedItem.id
      ? this.serviceItemService.updateRadiology(this.selectedItem.id, this.form.value)
      : this.serviceItemService.createRadiology(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.list.get();
    });
  }

  delete(id: string) {
    this.confirmation.warn('Delete this radiology exam?', 'Confirm Delete').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.serviceItemService.delete(id).subscribe(() => this.list.get());
      }
    });
  }
}
