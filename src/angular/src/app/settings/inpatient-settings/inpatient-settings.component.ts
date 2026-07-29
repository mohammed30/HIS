import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-inpatient-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="card shadow-sm">
      <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
        <h5 class="m-0"><i class="fas fa-bed me-2"></i>إعدادات التنويم</h5>
      </div>
      <div class="card-body">
        <form [formGroup]="form" (ngSubmit)="save()">
          <div class="row">
            <div class="col-md-6 mb-3">
              <label class="form-label fw-bold">مبلغ الدفعة المقدمة (الحد الائتماني)</label>
              <div class="input-group">
                <input type="number" class="form-control" formControlName="admissionDepositAmount">
                <span class="input-group-text">ر.س</span>
              </div>
              <small class="text-muted">هذا المبلغ سيكون الحد الائتماني المبدئي للمريض عند التنويم.</small>
            </div>
            <div class="col-md-6 mb-3">
              <label class="form-label fw-bold">إلزامية الدفعة المقدمة</label>
              <div class="form-check form-switch mt-2">
                <input class="form-check-input" type="checkbox" formControlName="requireAdvancePayment">
                <label class="form-check-label">إجبار موظف الاستقبال على تحصيل الدفعة المقدمة</label>
              </div>
            </div>
          </div>
          
          <div class="mt-4 border-top pt-3 text-end">
            <button type="button" class="btn btn-secondary me-2" (click)="loadSettings()" [disabled]="saving">
              <i class="fas fa-sync-alt me-1"></i> إعادة تحميل
            </button>
            <button type="submit" class="btn btn-success" [disabled]="form.invalid || saving">
              <i class="fas fa-save me-1"></i> حفظ الإعدادات
            </button>
          </div>
        </form>
      </div>
    </div>
  `
})
export class InpatientSettingsComponent implements OnInit {
  form: FormGroup;
  saving = false;
  apiUrl = environment.apis.default.url + '/api/app/inpatient-settings';

  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.form = this.fb.group({
      admissionDepositAmount: [1000, [Validators.required, Validators.min(0)]],
      requireAdvancePayment: [false]
    });
  }

  ngOnInit() {
    this.loadSettings();
  }

  loadSettings() {
    this.http.get<any>(this.apiUrl).subscribe({
      next: (res) => {
        this.form.patchValue(res);
      },
      error: (err) => {
        console.error('Error loading settings', err);
      }
    });
  }

  save() {
    if (this.form.invalid) return;
    this.saving = true;
    this.http.put(this.apiUrl, this.form.value).subscribe({
      next: () => {
        this.saving = false;
        alert('تم حفظ الإعدادات بنجاح');
      },
      error: (err) => {
        this.saving = false;
        console.error('Error saving settings', err);
        alert('حدث خطأ أثناء الحفظ');
      }
    });
  }
}
