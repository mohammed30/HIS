import { ListService, CoreModule, PagedResultDto } from '@abp/ng.core';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CompensationItemDto } from '../../proxy/hr/models';
import { HRService } from '../../proxy/hr/hr.service';
import { ConfirmationService, Confirmation, ThemeSharedModule } from '@abp/ng.theme.shared';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { compensationNatureOptions } from '../../proxy/hr/enums/compensation-nature.enum';
import { compensationValueTypeOptions } from '../../proxy/hr/enums/compensation-value-type.enum';
import { compensationMethodOptions } from '../../proxy/hr/enums/compensation-method.enum';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-compensation-items',
  standalone: true,
  imports: [CoreModule, ThemeSharedModule, ReactiveFormsModule, CommonModule, NgbDropdownModule],
  templateUrl: './compensation-items.html',
  styleUrls: ['./compensation-items.scss'],
  providers: [ListService],
})
export class CompensationItems implements OnInit {
  items: PagedResultDto<CompensationItemDto> = { items: [], totalCount: 0 };
  selectedItem = {} as CompensationItemDto;
  isModalOpen = false;
  form: FormGroup;

  natureOptions = compensationNatureOptions;
  valueTypeOptions = compensationValueTypeOptions;
  methodOptions = compensationMethodOptions;

  constructor(
    public readonly list: ListService,
    private hrService: HRService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService
  ) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.hrService.getCompensationItems().subscribe((items) => {
      this.items = {
        items: items,
        totalCount: items.length
      };
    });
  }

  createItem() {
    this.selectedItem = {} as CompensationItemDto;
    this.buildForm();
    this.isModalOpen = true;
  }

  editItem(id: string) {
    const item = this.items.items.find((x) => x.id === id);
    if (item) {
      this.selectedItem = { ...item };
      this.buildForm();
      this.isModalOpen = true;
    }
  }

  buildForm() {
    this.form = this.fb.group({
      nameAr: [this.selectedItem.nameAr || '', Validators.required],
      displayName: [this.selectedItem.displayName || '', Validators.required],
      nature: [this.selectedItem.nature || null, Validators.required],
      valueType: [this.selectedItem.valueType || null, Validators.required],
      method: [this.selectedItem.method || null, Validators.required],
      formulaExpression: [this.selectedItem.formulaExpression || ''],
      accountId: [this.selectedItem.accountId || ''],
      isActive: [this.selectedItem.isActive ?? true],
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }

    const request = this.selectedItem.id
      ? this.hrService.updateCompensationItem(this.selectedItem.id, this.form.value)
      : this.hrService.createCompensationItem(this.form.value);

    request.subscribe(() => {
      this.isModalOpen = false;
      this.form.reset();
      this.loadData();
    });
  }

  delete(id: string) {
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.hrService.deleteCompensationItem(id).subscribe(() => this.loadData());
      }
    });
  }
}
