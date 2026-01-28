import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { PageModule } from '@abp/ng.components/page';
import { RestService } from '@abp/ng.core';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-patient-medical-record',
  standalone: true,
  imports: [CommonModule, FormsModule, PageModule],
  template: `
    <abp-page [title]="'السجل الطبي للمريض'">
      <abp-page-toolbar-container>
        <div class="d-flex gap-2">
          <button class="btn btn-sm btn-outline-secondary" (click)="refreshData()">
            <i class="fas fa-sync"></i> تحديث
          </button>
        </div>
      </abp-page-toolbar-container>

      <!-- Patient Summary Header -->
      <div class="card mb-3" *ngIf="summary">
        <div class="card-body">
          <div class="row align-items-center">
            <div class="col-md-4">
              <h5 class="mb-1">{{ summary.patientName }}</h5>
              <small class="text-muted">العمر: {{ summary.age }} سنة | فصيلة الدم: {{ summary.bloodType || 'غير محدد' }}</small>
            </div>
            <div class="col-md-8">
              <div class="row text-center">
                <div class="col">
                  <span class="badge bg-danger fs-6 p-2">
                    <i class="fas fa-allergies me-1"></i>
                    {{ summary.activeAllergiesCount }} حساسية
                  </span>
                </div>
                <div class="col">
                  <span class="badge bg-warning text-dark fs-6 p-2">
                    <i class="fas fa-heartbeat me-1"></i>
                    {{ summary.chronicConditionsCount }} أمراض مزمنة
                  </span>
                </div>
                <div class="col">
                  <span class="badge bg-info fs-6 p-2">
                    <i class="fas fa-diagnoses me-1"></i>
                    {{ summary.activeDiagnosesCount }} تشخيص نشط
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Latest Vitals Card -->
      <div class="card mb-3" *ngIf="summary?.latestVitals">
        <div class="card-header bg-light d-flex justify-content-between align-items-center">
          <span><i class="fas fa-heartbeat text-danger me-2"></i>آخر العلامات الحيوية</span>
          <small class="text-muted">{{ summary.latestVitals.recordedAt | date:'yyyy-MM-dd HH:mm' }}</small>
        </div>
        <div class="card-body">
          <div class="row text-center">
            <div class="col" *ngIf="summary.latestVitals.bloodPressureSystolic">
              <div class="fw-bold">{{ summary.latestVitals.bloodPressureSystolic }}/{{ summary.latestVitals.bloodPressureDiastolic }}</div>
              <small class="text-muted">ضغط الدم</small>
            </div>
            <div class="col" *ngIf="summary.latestVitals.heartRate">
              <div class="fw-bold">{{ summary.latestVitals.heartRate }}</div>
              <small class="text-muted">النبض</small>
            </div>
            <div class="col" *ngIf="summary.latestVitals.temperature">
              <div class="fw-bold">{{ summary.latestVitals.temperature }}°</div>
              <small class="text-muted">الحرارة</small>
            </div>
            <div class="col" *ngIf="summary.latestVitals.oxygenSaturation">
              <div class="fw-bold">{{ summary.latestVitals.oxygenSaturation }}%</div>
              <small class="text-muted">الأكسجين</small>
            </div>
            <div class="col" *ngIf="summary.latestVitals.weight">
              <div class="fw-bold">{{ summary.latestVitals.weight }} كجم</div>
              <small class="text-muted">الوزن</small>
            </div>
          </div>
        </div>
      </div>

      <!-- Tabs Navigation -->
      <ul class="nav nav-tabs" role="tablist">
        <li class="nav-item">
          <button class="nav-link" [class.active]="activeTab === 'vitals'" (click)="activeTab = 'vitals'">
            <i class="fas fa-heartbeat me-1"></i> العلامات الحيوية
          </button>
        </li>
        <li class="nav-item">
          <button class="nav-link" [class.active]="activeTab === 'diagnoses'" (click)="activeTab = 'diagnoses'">
            <i class="fas fa-diagnoses me-1"></i> التشخيصات
          </button>
        </li>
        <li class="nav-item">
          <button class="nav-link" [class.active]="activeTab === 'allergies'" (click)="activeTab = 'allergies'">
            <i class="fas fa-allergies me-1"></i> الحساسية
          </button>
        </li>
        <li class="nav-item">
          <button class="nav-link" [class.active]="activeTab === 'history'" (click)="activeTab = 'history'">
            <i class="fas fa-history me-1"></i> التاريخ المرضي
          </button>
        </li>
        <li class="nav-item">
          <button class="nav-link" [class.active]="activeTab === 'notes'" (click)="activeTab = 'notes'">
            <i class="fas fa-sticky-note me-1"></i> الملاحظات
          </button>
        </li>
      </ul>

      <!-- Tab Content -->
      <div class="tab-content border border-top-0 p-3">
        <!-- Vital Signs Tab -->
        <div *ngIf="activeTab === 'vitals'">
          <div class="d-flex justify-content-between mb-3">
            <h5>العلامات الحيوية</h5>
            <button class="btn btn-sm btn-primary" (click)="openVitalSignModal()">
              <i class="fas fa-plus me-1"></i> إضافة قياس
            </button>
          </div>
          <table class="table table-striped table-hover">
            <thead>
              <tr>
                <th>التاريخ</th>
                <th>ضغط الدم</th>
                <th>النبض</th>
                <th>الحرارة</th>
                <th>الأكسجين</th>
                <th>الوزن</th>
                <th>الطول</th>
                <th>BMI</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let v of vitalSigns">
                <td>{{ v.recordedAt | date:'yyyy-MM-dd HH:mm' }}</td>
                <td>{{ v.bloodPressureSystolic }}/{{ v.bloodPressureDiastolic }}</td>
                <td>{{ v.heartRate }}</td>
                <td>{{ v.temperature }}</td>
                <td>{{ v.oxygenSaturation }}%</td>
                <td>{{ v.weight }}</td>
                <td>{{ v.height }}</td>
                <td>{{ v.bmi | number:'1.1-1' }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Diagnoses Tab -->
        <div *ngIf="activeTab === 'diagnoses'">
          <div class="d-flex justify-content-between mb-3">
            <h5>التشخيصات</h5>
            <button class="btn btn-sm btn-primary" (click)="openDiagnosisModal()">
              <i class="fas fa-plus me-1"></i> إضافة تشخيص
            </button>
          </div>
          <table class="table table-striped table-hover">
            <thead>
              <tr>
                <th>التاريخ</th>
                <th>ICD-10</th>
                <th>التشخيص</th>
                <th>النوع</th>
                <th>الحالة</th>
                <th>الطبيب</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let d of diagnoses">
                <td>{{ d.diagnosisDate | date:'yyyy-MM-dd' }}</td>
                <td><code>{{ d.icd10Code }}</code></td>
                <td>{{ d.diagnosisNameAr }}</td>
                <td>
                  <span class="badge" [ngClass]="d.type === 0 ? 'bg-primary' : 'bg-secondary'">
                    {{ d.type === 0 ? 'رئيسي' : 'ثانوي' }}
                  </span>
                </td>
                <td>
                  <span class="badge" [ngClass]="{'bg-success': d.status === 1, 'bg-warning text-dark': d.status === 2, 'bg-info': d.status === 0}">
                    {{ d.status === 0 ? 'نشط' : d.status === 1 ? 'تم الشفاء' : 'مزمن' }}
                  </span>
                </td>
                <td>{{ d.diagnosedByName }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Allergies Tab -->
        <div *ngIf="activeTab === 'allergies'">
          <div class="d-flex justify-content-between mb-3">
            <h5>الحساسية</h5>
            <button class="btn btn-sm btn-primary" (click)="openAllergyModal()">
              <i class="fas fa-plus me-1"></i> إضافة حساسية
            </button>
          </div>
          <div class="row">
            <div class="col-md-4" *ngFor="let a of allergies">
              <div class="card mb-3" [ngClass]="{'border-danger': a.severity >= 2}">
                <div class="card-header d-flex justify-content-between">
                  <span>
                    <i class="fas fa-exclamation-triangle text-warning me-1" *ngIf="a.severity >= 2"></i>
                    {{ a.allergenNameAr }}
                  </span>
                  <span class="badge" [ngClass]="{'bg-success': a.severity === 0, 'bg-warning text-dark': a.severity === 1, 'bg-danger': a.severity >= 2}">
                    {{ getSeverityLabel(a.severity) }}
                  </span>
                </div>
                <div class="card-body">
                  <p class="mb-1"><strong>النوع:</strong> {{ getAllergenTypeLabel(a.allergenType) }}</p>
                  <p class="mb-1" *ngIf="a.reaction"><strong>رد الفعل:</strong> {{ a.reaction }}</p>
                  <span class="badge" [ngClass]="a.status === 0 ? 'bg-danger' : 'bg-secondary'">
                    {{ a.status === 0 ? 'نشطة' : 'تم الشفاء' }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Medical History Tab -->
        <div *ngIf="activeTab === 'history'">
          <div class="d-flex justify-content-between mb-3">
            <h5>التاريخ المرضي</h5>
            <button class="btn btn-sm btn-primary" (click)="openHistoryModal()">
              <i class="fas fa-plus me-1"></i> إضافة حالة
            </button>
          </div>
          <table class="table table-striped table-hover">
            <thead>
              <tr>
                <th>الحالة</th>
                <th>ICD-10</th>
                <th>تاريخ التشخيص</th>
                <th>مزمن</th>
                <th>ملاحظات</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let h of medicalHistories">
                <td>{{ h.conditionAr }}</td>
                <td><code>{{ h.icd10Code }}</code></td>
                <td>{{ h.diagnosedDate | date:'yyyy-MM-dd' }}</td>
                <td>
                  <span class="badge" [ngClass]="h.isChronic ? 'bg-danger' : 'bg-secondary'">
                    {{ h.isChronic ? 'مزمن' : 'غير مزمن' }}
                  </span>
                </td>
                <td>{{ h.notes }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Notes Tab -->
        <div *ngIf="activeTab === 'notes'">
          <div class="d-flex justify-content-between mb-3">
            <h5>الملاحظات الطبية</h5>
            <button class="btn btn-sm btn-primary" (click)="openNoteModal()">
              <i class="fas fa-plus me-1"></i> إضافة ملاحظة
            </button>
          </div>
          <div class="row">
            <div class="col-12 mb-3" *ngFor="let n of patientNotes">
              <div class="card">
                <div class="card-header d-flex justify-content-between">
                  <span>
                    <i class="fas fa-sticky-note me-1"></i>
                    {{ n.title }}
                  </span>
                  <small class="text-muted">{{ n.creationTime | date:'yyyy-MM-dd HH:mm' }} - {{ n.createdByName }}</small>
                </div>
                <div class="card-body">
                  <p class="mb-0" style="white-space: pre-wrap;">{{ n.content }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Add Vital Sign Modal -->
      <div class="modal fade show" id="vitalSignModal" tabindex="-1" *ngIf="showVitalSignModal" style="display: block;">
        <div class="modal-dialog modal-lg">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">إضافة علامات حيوية</h5>
              <button type="button" class="btn-close" (click)="showVitalSignModal = false"></button>
            </div>
            <div class="modal-body">
              <div class="row g-3">
                <div class="col-md-4">
                  <label class="form-label">ضغط الدم الانقباضي</label>
                  <input type="number" class="form-control" [(ngModel)]="newVitalSign.bloodPressureSystolic">
                </div>
                <div class="col-md-4">
                  <label class="form-label">ضغط الدم الانبساطي</label>
                  <input type="number" class="form-control" [(ngModel)]="newVitalSign.bloodPressureDiastolic">
                </div>
                <div class="col-md-4">
                  <label class="form-label">معدل النبض</label>
                  <input type="number" class="form-control" [(ngModel)]="newVitalSign.heartRate">
                </div>
                <div class="col-md-4">
                  <label class="form-label">درجة الحرارة</label>
                  <input type="number" step="0.1" class="form-control" [(ngModel)]="newVitalSign.temperature">
                </div>
                <div class="col-md-4">
                  <label class="form-label">نسبة الأكسجين</label>
                  <input type="number" class="form-control" [(ngModel)]="newVitalSign.oxygenSaturation">
                </div>
                <div class="col-md-4">
                  <label class="form-label">معدل التنفس</label>
                  <input type="number" class="form-control" [(ngModel)]="newVitalSign.respiratoryRate">
                </div>
                <div class="col-md-6">
                  <label class="form-label">الوزن (كجم)</label>
                  <input type="number" step="0.1" class="form-control" [(ngModel)]="newVitalSign.weight">
                </div>
                <div class="col-md-6">
                  <label class="form-label">الطول (سم)</label>
                  <input type="number" class="form-control" [(ngModel)]="newVitalSign.height">
                </div>
                <div class="col-12">
                  <label class="form-label">ملاحظات</label>
                  <textarea class="form-control" rows="2" [(ngModel)]="newVitalSign.notes"></textarea>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" (click)="showVitalSignModal = false">إلغاء</button>
              <button type="button" class="btn btn-primary" (click)="saveVitalSign()">حفظ</button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade show" *ngIf="showVitalSignModal" (click)="showVitalSignModal = false"></div>

      <!-- Add Diagnosis Modal -->
      <div class="modal fade show" id="diagnosisModal" tabindex="-1" *ngIf="showDiagnosisModal" style="display: block;">
        <div class="modal-dialog modal-lg">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">إضافة تشخيص</h5>
              <button type="button" class="btn-close" (click)="showDiagnosisModal = false"></button>
            </div>
            <div class="modal-body">
              <div class="row g-3">
                <div class="col-md-4">
                  <label class="form-label">كود ICD-10</label>
                  <input type="text" class="form-control" [(ngModel)]="newDiagnosis.icd10Code">
                </div>
                <div class="col-md-8">
                  <label class="form-label">التشخيص بالعربي *</label>
                  <input type="text" class="form-control" [(ngModel)]="newDiagnosis.diagnosisNameAr" required>
                </div>
                <div class="col-md-6">
                  <label class="form-label">التشخيص بالإنجليزي</label>
                  <input type="text" class="form-control" [(ngModel)]="newDiagnosis.diagnosisNameEn">
                </div>
                <div class="col-md-6">
                  <label class="form-label">تاريخ التشخيص</label>
                  <input type="date" class="form-control" [(ngModel)]="newDiagnosis.diagnosisDate">
                </div>
                <div class="col-md-6">
                  <label class="form-label">النوع</label>
                  <select class="form-select" [(ngModel)]="newDiagnosis.type">
                    <option [value]="0">رئيسي</option>
                    <option [value]="1">ثانوي</option>
                    <option [value]="2">تفريقي</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">الحالة</label>
                  <select class="form-select" [(ngModel)]="newDiagnosis.status">
                    <option [value]="0">نشط</option>
                    <option [value]="1">تم الشفاء</option>
                    <option [value]="2">مزمن</option>
                  </select>
                </div>
                <div class="col-12">
                  <label class="form-label">ملاحظات</label>
                  <textarea class="form-control" rows="2" [(ngModel)]="newDiagnosis.notes"></textarea>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" (click)="showDiagnosisModal = false">إلغاء</button>
              <button type="button" class="btn btn-primary" (click)="saveDiagnosis()">حفظ</button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade show" *ngIf="showDiagnosisModal" (click)="showDiagnosisModal = false"></div>

      <!-- Add Allergy Modal -->
      <div class="modal fade show" id="allergyModal" tabindex="-1" *ngIf="showAllergyModal" style="display: block;">
        <div class="modal-dialog modal-lg">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">إضافة حساسية</h5>
              <button type="button" class="btn-close" (click)="showAllergyModal = false"></button>
            </div>
            <div class="modal-body">
              <div class="row g-3">
                <div class="col-md-6">
                  <label class="form-label">نوع المادة</label>
                  <select class="form-select" [(ngModel)]="newAllergy.allergenType">
                    <option [value]="0">دواء</option>
                    <option [value]="1">طعام</option>
                    <option [value]="2">بيئي</option>
                    <option [value]="3">أخرى</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">الشدة</label>
                  <select class="form-select" [(ngModel)]="newAllergy.severity">
                    <option [value]="0">خفيفة</option>
                    <option [value]="1">متوسطة</option>
                    <option [value]="2">شديدة</option>
                    <option [value]="3">مهددة للحياة</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">اسم المادة بالعربي *</label>
                  <input type="text" class="form-control" [(ngModel)]="newAllergy.allergenNameAr" required>
                </div>
                <div class="col-md-6">
                  <label class="form-label">اسم المادة بالإنجليزي</label>
                  <input type="text" class="form-control" [(ngModel)]="newAllergy.allergenNameEn">
                </div>
                <div class="col-md-12">
                  <label class="form-label">رد الفعل</label>
                  <input type="text" class="form-control" [(ngModel)]="newAllergy.reaction">
                </div>
                <div class="col-12">
                  <label class="form-label">ملاحظات</label>
                  <textarea class="form-control" rows="2" [(ngModel)]="newAllergy.notes"></textarea>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" (click)="showAllergyModal = false">إلغاء</button>
              <button type="button" class="btn btn-primary" (click)="saveAllergy()">حفظ</button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade show" *ngIf="showAllergyModal" (click)="showAllergyModal = false"></div>

      <!-- Add Medical History Modal -->
      <div class="modal fade show" id="historyModal" tabindex="-1" *ngIf="showHistoryModal" style="display: block;">
        <div class="modal-dialog modal-lg">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">إضافة تاريخ مرضي</h5>
              <button type="button" class="btn-close" (click)="showHistoryModal = false"></button>
            </div>
            <div class="modal-body">
              <div class="row g-3">
                <div class="col-md-6">
                  <label class="form-label">الحالة بالعربي *</label>
                  <input type="text" class="form-control" [(ngModel)]="newHistory.conditionAr" required>
                </div>
                <div class="col-md-6">
                  <label class="form-label">الحالة بالإنجليزي</label>
                  <input type="text" class="form-control" [(ngModel)]="newHistory.conditionEn">
                </div>
                <div class="col-md-4">
                  <label class="form-label">كود ICD-10</label>
                  <input type="text" class="form-control" [(ngModel)]="newHistory.icd10Code">
                </div>
                <div class="col-md-4">
                  <label class="form-label">تاريخ التشخيص</label>
                  <input type="date" class="form-control" [(ngModel)]="newHistory.diagnosedDate">
                </div>
                <div class="col-md-4">
                  <label class="form-label">حالة مزمنة؟</label>
                  <div class="form-check mt-2">
                    <input type="checkbox" class="form-check-input" [(ngModel)]="newHistory.isChronic" id="chronicCheck">
                    <label class="form-check-label" for="chronicCheck">نعم</label>
                  </div>
                </div>
                <div class="col-12">
                  <label class="form-label">ملاحظات</label>
                  <textarea class="form-control" rows="2" [(ngModel)]="newHistory.notes"></textarea>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" (click)="showHistoryModal = false">إلغاء</button>
              <button type="button" class="btn btn-primary" (click)="saveHistory()">حفظ</button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade show" *ngIf="showHistoryModal" (click)="showHistoryModal = false"></div>

      <!-- Add Patient Note Modal -->
      <div class="modal fade show" id="noteModal" tabindex="-1" *ngIf="showNoteModal" style="display: block;">
        <div class="modal-dialog modal-lg">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">إضافة ملاحظة</h5>
              <button type="button" class="btn-close" (click)="showNoteModal = false"></button>
            </div>
            <div class="modal-body">
              <div class="row g-3">
                <div class="col-md-6">
                  <label class="form-label">نوع الملاحظة</label>
                  <select class="form-select" [(ngModel)]="newNote.noteType">
                    <option [value]="0">متابعة</option>
                    <option [value]="1">استشارة</option>
                    <option [value]="2">تعليمات</option>
                    <option [value]="3">أخرى</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">العنوان *</label>
                  <input type="text" class="form-control" [(ngModel)]="newNote.title" required>
                </div>
                <div class="col-12">
                  <label class="form-label">المحتوى *</label>
                  <textarea class="form-control" rows="4" [(ngModel)]="newNote.content" required></textarea>
                </div>
                <div class="col-12">
                  <div class="form-check">
                    <input type="checkbox" class="form-check-input" [(ngModel)]="newNote.isPrivate" id="privateCheck">
                    <label class="form-check-label" for="privateCheck">ملاحظة خاصة</label>
                  </div>
                </div>
              </div>
            </div>
            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" (click)="showNoteModal = false">إلغاء</button>
              <button type="button" class="btn btn-primary" (click)="saveNote()">حفظ</button>
            </div>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade show" *ngIf="showNoteModal" (click)="showNoteModal = false"></div>

    </abp-page>
  `,
  styles: [`
    .modal.fade { display: block; opacity: 1; }
    .modal.show { display: block !important; }
    .modal-backdrop.show { opacity: 0.5; }
    .nav-link { cursor: pointer; }
    .badge { font-size: 0.85rem; }
  `]
})
export class PatientMedicalRecordComponent implements OnInit {
  private rest = inject(RestService);
  private route = inject(ActivatedRoute);

  patientId!: string;
  activeTab = 'vitals';

  summary: any = null;
  vitalSigns: any[] = [];
  diagnoses: any[] = [];
  allergies: any[] = [];
  medicalHistories: any[] = [];
  patientNotes: any[] = [];

  showVitalSignModal = false;
  newVitalSign: any = {};

  showDiagnosisModal = false;
  newDiagnosis: any = {};

  showAllergyModal = false;
  newAllergy: any = {};

  showHistoryModal = false;
  newHistory: any = {};

  showNoteModal = false;
  newNote: any = {};

  ngOnInit() {
    this.patientId = this.route.snapshot.paramMap.get('id') || '';
    if (this.patientId) {
      this.loadSummary();
      this.loadVitalSigns();
      this.loadDiagnoses();
      this.loadAllergies();
      this.loadMedicalHistories();
      this.loadPatientNotes();
    }
  }

  refreshData() {
    this.loadSummary();
    this.loadVitalSigns();
    this.loadDiagnoses();
    this.loadAllergies();
    this.loadMedicalHistories();
    this.loadPatientNotes();
  }

  loadSummary() {
    this.rest.request({ method: 'GET', url: `/api/app/medical-records/summary/${this.patientId}` })
      .subscribe({
        next: (res: any) => this.summary = res,
        error: (err) => console.error('Summary error:', err)
      });
  }

  loadVitalSigns() {
    this.rest.request({ method: 'GET', url: `/api/app/medical-records/vital-signs/${this.patientId}` })
      .subscribe({
        next: (res: any) => this.vitalSigns = res.items || [],
        error: (err) => console.error('Vitals error:', err)
      });
  }

  loadDiagnoses() {
    this.rest.request({ method: 'GET', url: `/api/app/medical-records/diagnoses/${this.patientId}` })
      .subscribe({
        next: (res: any) => this.diagnoses = res.items || [],
        error: (err) => console.error('Diagnoses error:', err)
      });
  }

  loadAllergies() {
    this.rest.request({ method: 'GET', url: `/api/app/medical-records/allergies/${this.patientId}` })
      .subscribe({
        next: (res: any) => this.allergies = res.items || [],
        error: (err) => console.error('Allergies error:', err)
      });
  }

  loadMedicalHistories() {
    this.rest.request({ method: 'GET', url: `/api/app/medical-records/history/${this.patientId}` })
      .subscribe({
        next: (res: any) => this.medicalHistories = res.items || [],
        error: (err) => console.error('History error:', err)
      });
  }

  loadPatientNotes() {
    this.rest.request({ method: 'GET', url: `/api/app/medical-records/notes/${this.patientId}` })
      .subscribe({
        next: (res: any) => this.patientNotes = res.items || [],
        error: (err) => console.error('Notes error:', err)
      });
  }

  getSeverityLabel(severity: number): string {
    const labels = ['خفيفة', 'متوسطة', 'شديدة', 'مهددة للحياة'];
    return labels[severity] || '';
  }

  getAllergenTypeLabel(type: number): string {
    const labels = ['دواء', 'طعام', 'بيئي'];
    return labels[type] || 'أخرى';
  }

  openVitalSignModal() {
    this.newVitalSign = { patientId: this.patientId, recordedAt: new Date().toISOString() };
    this.showVitalSignModal = true;
  }

  saveVitalSign() {
    this.rest.request({ method: 'POST', url: '/api/app/medical-records/vital-signs', body: this.newVitalSign })
      .subscribe({
        next: () => {
          this.showVitalSignModal = false;
          this.loadVitalSigns();
          this.loadSummary();
        },
        error: (err) => {
          console.error('Error saving vital sign:', err);
          alert('حدث خطأ أثناء الحفظ');
        }
      });
  }

  openDiagnosisModal() {
    this.newDiagnosis = { patientId: this.patientId, diagnosisDate: new Date().toISOString().split('T')[0], type: 0, status: 0 };
    this.showDiagnosisModal = true;
  }

  saveDiagnosis() {
    this.rest.request({ method: 'POST', url: '/api/app/medical-records/diagnoses', body: this.newDiagnosis })
      .subscribe({
        next: () => {
          this.showDiagnosisModal = false;
          this.loadDiagnoses();
          this.loadSummary();
        },
        error: (err) => {
          console.error('Error saving diagnosis:', err);
          alert('حدث خطأ أثناء الحفظ');
        }
      });
  }

  openAllergyModal() {
    this.newAllergy = { patientId: this.patientId, allergenType: 0, severity: 1, status: 0 };
    this.showAllergyModal = true;
  }

  saveAllergy() {
    this.rest.request({ method: 'POST', url: '/api/app/medical-records/allergies', body: this.newAllergy })
      .subscribe({
        next: () => {
          this.showAllergyModal = false;
          this.loadAllergies();
          this.loadSummary();
        },
        error: (err) => {
          console.error('Error saving allergy:', err);
          alert('حدث خطأ أثناء الحفظ');
        }
      });
  }

  openHistoryModal() {
    this.newHistory = { patientId: this.patientId, isChronic: false };
    this.showHistoryModal = true;
  }

  saveHistory() {
    this.rest.request({ method: 'POST', url: '/api/app/medical-records/history', body: this.newHistory })
      .subscribe({
        next: () => {
          this.showHistoryModal = false;
          this.loadMedicalHistories();
          this.loadSummary();
        },
        error: (err) => {
          console.error('Error saving history:', err);
          alert('حدث خطأ أثناء الحفظ');
        }
      });
  }

  openNoteModal() {
    this.newNote = { patientId: this.patientId, noteType: 0, isPrivate: false };
    this.showNoteModal = true;
  }

  saveNote() {
    this.rest.request({ method: 'POST', url: '/api/app/medical-records/notes', body: this.newNote })
      .subscribe({
        next: () => {
          this.showNoteModal = false;
          this.loadPatientNotes();
        },
        error: (err) => {
          console.error('Error saving note:', err);
          alert('حدث خطأ أثناء الحفظ');
        }
      });
  }
}
