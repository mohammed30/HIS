import { Component, Input, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { SupplierService } from '../../../proxy/inventory/supplier.service';
import { SupplierDto } from '../../../proxy/inventory/dtos/supplier-dto';

@Component({
  selector: 'app-supplier-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './supplier-detail.html',
  styleUrls: ['./supplier-detail.scss']
})
export class SupplierDetailComponent implements OnInit {
  @Input() id: string;

  activeModal = inject(NgbActiveModal);
  private fb = inject(FormBuilder);
  private supplierService = inject(SupplierService);

  form: FormGroup;
  isSaving = false;

  ngOnInit() {
    this.buildForm();
    if (this.id) {
      this.loadData();
    }
  }

  buildForm() {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      contactPerson: ['', Validators.maxLength(100)],
      phone: ['', Validators.maxLength(20)],
      email: ['', [Validators.email, Validators.maxLength(100)]],
      address: ['', Validators.maxLength(200)],
      taxId: ['', Validators.maxLength(50)],
    });
  }

  loadData() {
    this.supplierService.get(this.id).subscribe((res) => {
      this.form.patchValue(res);
    });
  }

  save() {
    if (this.form.invalid) {
      return;
    }

    this.isSaving = true;
    const input = this.form.value;

    const request = this.id
      ? this.supplierService.update(this.id, input)
      : this.supplierService.create(input);

    request.subscribe({
      next: () => {
        this.activeModal.close(true);
      },
      error: (err) => {
        this.isSaving = false;
        console.error(err);
      }
    });
  }

  dismiss() {
    this.activeModal.dismiss();
  }
}
