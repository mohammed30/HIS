import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { PageModule } from '@abp/ng.components/page';
import { RestService, CoreModule } from '@abp/ng.core';
import { ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { MedicalOrderService } from '../../proxy/clinical/medical-order.service';
import { ServiceItemService } from '../../proxy/services/service-item.service';
import { OrderType } from '../../proxy/clinical/order-type.enum';
import { OrderStatus } from '../../proxy/clinical/order-status.enum';

@Component({
  selector: 'app-patient-medical-record',
  standalone: true,
  imports: [CommonModule, FormsModule, PageModule, CoreModule],
  template: `
    <abp-page [title]="'السجل الطبي للمريض' | abpLocalization">
      
      <div class="container-fluid px-0">
        
        <!-- Modern Patient Header -->
        <div class="patient-header-card p-4 mx-0" *ngIf="summary">
          <div class="row align-items-center">
            <div class="col-auto">
              <div class="patient-avatar">
                <i class="fas fa-user"></i>
              </div>
            </div>
            <div class="col">
              <h3 class="fw-bold text-primary mb-2">{{ summary.patientName }}</h3>
              <div class="d-flex gap-4 align-items-center">
                <span class="text-secondary fw-medium">
                  <ng-container *ngIf="summary.gender === 0; else femaleIcon">
                    <i class="fas fa-mars me-2 text-primary fs-5"></i> ذكر
                  </ng-container>
                  <ng-template #femaleIcon>
                    <i class="fas fa-venus me-2 text-danger fs-5"></i> أنثى
                  </ng-template>
                </span>
                <span class="text-secondary fw-medium"><i class="fas fa-birthday-cake me-2 text-warning fs-5"></i> {{ summary.age }} سنة</span>
                <span class="badge bg-danger bg-opacity-10 text-danger rounded-pill px-3 py-2 border border-danger border-opacity-10">
                    <i class="fas fa-tint me-2"></i> {{ summary.bloodType || 'غير محدد' }}
                </span>
              </div>
            </div>
            <div class="col-auto">
              <div class="d-flex gap-2">
                <div class="stat-badge bg-danger bg-opacity-10 text-danger border border-danger border-opacity-25" *ngIf="summary.activeAllergiesCount > 0">
                  <div class="stat-info text-center px-2">
                    <span class="stat-value">{{ summary.activeAllergiesCount }}</span>
                    <span class="stat-label">حساسية</span>
                  </div>
                  <i class="fas fa-allergies"></i>
                </div>
                
                <div class="stat-badge bg-warning bg-opacity-10 text-warning border border-warning border-opacity-25" *ngIf="summary.chronicConditionsCount > 0">
                  <div class="stat-info text-center px-2">
                    <span class="stat-value">{{ summary.chronicConditionsCount }}</span>
                    <span class="stat-label">أمراض مزمنة</span>
                  </div>
                  <i class="fas fa-heartbeat"></i>
                </div>

                <div class="stat-badge bg-info bg-opacity-10 text-info border border-info border-opacity-25">
                  <div class="stat-info text-center px-2">
                    <span class="stat-value">{{ summary.activeDiagnosesCount }}</span>
                    <span class="stat-label">تشخيص نشط</span>
                  </div>
                  <i class="fas fa-clipboard-check"></i>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Latest Vitals Grid -->
        <div class="d-flex justify-content-between align-items-center mb-3">
          <h5 class="fw-bold text-dark mb-0"><i class="fas fa-heartbeat text-danger me-2"></i>آخر العلامات الحيوية</h5>
          <small class="text-muted" *ngIf="summary?.latestVitals">
             <i class="far fa-clock me-1"></i> {{ summary.latestVitals.recordedAt | date:'yyyy-MM-dd HH:mm' }}
          </small>
        </div>

        <div class="row g-3 mb-4" *ngIf="summary?.latestVitals; else noVitals">
          <div class="col-md-2 col-sm-4">
            <div class="vital-card" style="border-right-color: #dc3545;">
              <i class="fas fa-tachometer-alt vital-icon text-danger"></i>
              <div class="vital-value">{{ summary.latestVitals.bloodPressureSystolic }}/{{ summary.latestVitals.bloodPressureDiastolic }}</div>
              <div class="vital-label">ضغط الدم</div>
            </div>
          </div>
          <div class="col-md-2 col-sm-4">
            <div class="vital-card" style="border-right-color: #fd7e14;">
              <i class="fas fa-heartbeat vital-icon text-warning"></i>
              <div class="vital-value">{{ summary.latestVitals.heartRate }}</div>
              <div class="vital-label">النبض (bpm)</div>
            </div>
          </div>
          <div class="col-md-2 col-sm-4">
            <div class="vital-card" style="border-right-color: #ffc107;">
              <i class="fas fa-thermometer-half vital-icon text-warning"></i>
              <div class="vital-value">{{ summary.latestVitals.temperature }}°</div>
              <div class="vital-label">الحرارة (C)</div>
            </div>
          </div>
          <div class="col-md-2 col-sm-4">
            <div class="vital-card" style="border-right-color: #0dcaf0;">
              <i class="fas fa-lungs vital-icon text-info"></i>
              <div class="vital-value">{{ summary.latestVitals.oxygenSaturation }}%</div>
              <div class="vital-label">الأكسجين (SpO2)</div>
            </div>
          </div>
          <div class="col-md-2 col-sm-4">
            <div class="vital-card" style="border-right-color: #198754;">
              <i class="fas fa-weight vital-icon text-success"></i>
              <div class="vital-value">{{ summary.latestVitals.weight }} <small class="fs-6 fw-normal text-muted">كجم</small></div>
              <div class="vital-label">الوزن</div>
            </div>
          </div>
           <!-- BMI Placeholder if calculated -->
           <div class="col-md-2 col-sm-4">
            <div class="vital-card" style="border-right-color: #6c757d;">
              <i class="fas fa-calculator vital-icon text-secondary"></i>
              <!-- Simple calculation if not provided -->
              <div class="vital-value">-</div> 
              <div class="vital-label">مؤشر الكتلة (BMI)</div>
            </div>
          </div>
        </div>
        <ng-template #noVitals>
          <div class="alert alert-light text-center border-0 shadow-sm mb-4">
             <i class="fas fa-info-circle me-2"></i> لا توجد علامات حيوية مسجلة بعد.
          </div>
        </ng-template>

        <!-- Tabs & Content -->
        <div class="card shadow-sm border-0 rounded-3">
          <div class="card-header bg-white border-bottom-0 pb-0">
            <ul class="nav nav-tabs card-header-tabs" role="tablist">
              <li class="nav-item">
                <button class="nav-link" [class.active]="activeTab === 'vitals'" (click)="activeTab = 'vitals'">
                  <i class="fas fa-chart-line me-2"></i> العلامات الحيوية
                </button>
              </li>
              <li class="nav-item">
                <button class="nav-link" [class.active]="activeTab === 'diagnoses'" (click)="activeTab = 'diagnoses'">
                  <i class="fas fa-stethoscope me-2"></i> التشخيصات
                </button>
              </li>
              <li class="nav-item">
                <button class="nav-link" [class.active]="activeTab === 'allergies'" (click)="activeTab = 'allergies'">
                  <i class="fas fa-allergies me-2"></i> الحساسية
                </button>
              </li>
              <li class="nav-item">
                <button class="nav-link" [class.active]="activeTab === 'history'" (click)="activeTab = 'history'">
                  <i class="fas fa-notes-medical me-2"></i> التاريخ المرضي
                </button>
              </li>
              <li class="nav-item">
                <button class="nav-link" [class.active]="activeTab === 'orders'" (click)="activeTab = 'orders'">
                  <i class="fas fa-clipboard-list me-2"></i> الطلبات
                </button>
              </li>
              <li class="nav-item">
                <button class="nav-link" [class.active]="activeTab === 'notes'" (click)="activeTab = 'notes'">
                  <i class="fas fa-file-medical-alt me-2"></i> الملاحظات
                </button>
              </li>
            </ul>
          </div>
          
          <div class="card-body p-4 bg-white rounded-bottom-3">
             
             <!-- Vital Signs Table -->
             <div *ngIf="activeTab === 'vitals'">
               <div class="text-end mb-3">
                  <button class="btn btn-primary btn-sm rounded-pill" (click)="openVitalSignModal()">
                    <i class="fas fa-plus me-1"></i> تسجيل قياس جديد
                  </button>
               </div>
               <div class="table-responsive">
                 <table class="table table-hover align-middle">
                    <thead class="table-light">
                      <tr>
                        <th>التاريخ</th>
                        <th><i class="fas fa-tachometer-alt text-muted me-1"></i> ضغط الدم</th>
                        <th><i class="fas fa-heartbeat text-muted me-1"></i> النبض</th>
                        <th><i class="fas fa-thermometer-half text-muted me-1"></i> الحرارة</th>
                        <th><i class="fas fa-lungs text-muted me-1"></i> الأكسجين</th>
                        <th>الوزن</th>
                        <th>الطول</th>
                        <th>BMI</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr *ngFor="let v of vitalSigns">
                        <td>{{ v.recordedAt | date:'yyyy-MM-dd HH:mm' }}</td>
                        <td><span class="badge bg-light text-dark border">{{ v.bloodPressureSystolic }}/{{ v.bloodPressureDiastolic }}</span></td>
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
             </div>

             <!-- Diagnoses Table -->
             <div *ngIf="activeTab === 'diagnoses'">
                <div class="text-end mb-3">
                  <button class="btn btn-primary btn-sm rounded-pill" (click)="openDiagnosisModal()">
                    <i class="fas fa-plus me-1"></i> إضافة تشخيص
                  </button>
               </div>
               <table class="table table-hover align-middle">
                 <thead class="table-light">
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
                     <td><span class="badge bg-secondary rounded-pill font-monospace">{{ d.icd10Code }}</span></td>
                     <td class="fw-bold">{{ d.diagnosisNameAr }}</td>
                     <td>
                       <span class="badge" [ngClass]="d.type === 0 ? 'bg-primary' : 'bg-secondary'">
                         {{ d.type === 0 ? 'رئيسي' : 'ثانوي' }}
                       </span>
                     </td>
                     <td>
                       <span class="badge rounded-pill" [ngClass]="{'bg-success': d.status === 1, 'bg-warning text-dark': d.status === 2, 'bg-info': d.status === 0}">
                         {{ d.status === 0 ? 'نشط' : d.status === 1 ? 'تم الشفاء' : 'مزمن' }}
                       </span>
                     </td>
                     <td><small>{{ d.diagnosedByName }}</small></td>
                   </tr>
                 </tbody>
               </table>
             </div>

             <!-- Allergies Cards -->
             <div *ngIf="activeTab === 'allergies'">
                <div class="text-end mb-3">
                  <button class="btn btn-primary btn-sm rounded-pill" (click)="openAllergyModal()">
                    <i class="fas fa-plus me-1"></i> إضافة حساسية
                  </button>
               </div>
               <div class="row g-3">
                  <div class="col-md-6 col-lg-4" *ngFor="let a of allergies">
                    <div class="card h-100 border-0 shadow-sm" [ngClass]="{'border-start border-4 border-danger': a.severity >= 2}">
                      <div class="card-body">
                         <div class="d-flex justify-content-between align-items-start mb-2">
                           <h6 class="fw-bold mb-0">
                             <i class="fas fa-exclamation-circle text-danger me-2" *ngIf="a.severity >= 2"></i>
                             {{ a.allergenNameAr }}
                           </h6>
                           <span class="badge rounded-pill" [ngClass]="{'bg-success': a.severity === 0, 'bg-warning text-dark': a.severity === 1, 'bg-danger': a.severity >= 2}">
                              {{ getSeverityLabel(a.severity) }}
                           </span>
                         </div>
                         <div class="small text-muted mb-2">
                           <i class="fas fa-tag me-1"></i> {{ getAllergenTypeLabel(a.allergenType) }}
                         </div>
                         <p class="small mb-2 bg-light p-2 rounded" *ngIf="a.reaction">
                           <strong>رد الفعل:</strong> {{ a.reaction }}
                         </p>
                         <div class="text-end">
                            <span class="badge rounded-pill" [ngClass]="a.status === 0 ? 'bg-danger' : 'bg-secondary'">
                              {{ a.status === 0 ? 'نشطة' : 'تم الشفاء' }}
                            </span>
                         </div>
                      </div>
                    </div>
                  </div>
               </div>
             </div>
             
             <!-- History -->
             <div *ngIf="activeTab === 'history'">
                <div class="text-end mb-3">
                  <button class="btn btn-primary btn-sm rounded-pill" (click)="openHistoryModal()">
                    <i class="fas fa-plus me-1"></i> إضافة حالة
                  </button>
                </div>
                <table class="table table-hover">
                   <thead class="table-light">
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
                         <td class="fw-bold">{{ h.conditionAr }}</td>
                         <td><code>{{ h.icd10Code }}</code></td>
                         <td>{{ h.diagnosedDate | date:'yyyy-MM-dd' }}</td>
                         <td>
                           <span class="badge" [ngClass]="h.isChronic ? 'bg-danger' : 'bg-secondary'">
                             {{ h.isChronic ? 'مزمن' : 'غير مزمن' }}
                           </span>
                         </td>
                         <td class="text-truncate" style="max-width: 200px;">{{ h.notes }}</td>
                      </tr>
                   </tbody>
                </table>
             </div>

             <!-- Notes -->
             <div *ngIf="activeTab === 'notes'">
                <div class="text-end mb-3">
                  <button class="btn btn-primary btn-sm rounded-pill" (click)="openNoteModal()">
                    <i class="fas fa-plus me-1"></i> إضافة ملاحظة
                  </button>
                </div>
                <div class="row">
                   <div class="col-12 mb-3" *ngFor="let n of patientNotes">
                      <div class="card border-0 shadow-sm bg-light">
                         <div class="card-body">
                            <div class="d-flex justify-content-between align-items-center mb-2">
                               <h6 class="fw-bold text-primary mb-0">
                                  <i class="fas fa-comment-medical me-2"></i> {{ n.title }}
                               </h6>
                               <small class="text-muted">{{ n.creationTime | date:'yyyy-MM-dd HH:mm' }} <span class="mx-1">•</span> {{ n.createdByName }}</small>
                            </div>
                            <hr class="my-2 opacity-10">
                            <p class="mb-0 text-dark" style="white-space: pre-wrap;">{{ n.content }}</p>
                         </div>
                      </div>
                   </div>
                </div>
             </div>

             <!-- Orders -->
             <div *ngIf="activeTab === 'orders'">
                <div class="text-end mb-3">
                  <button class="btn btn-primary btn-sm rounded-pill" (click)="openOrderModal()">
                    <i class="fas fa-plus me-1"></i> طلب جديد (Referral)
                  </button>
                </div>
                <div class="table-responsive">
                    <table class="table table-hover align-middle">
                        <thead class="table-light">
                            <tr>
                                <th>التاريخ</th>
                                <th>النوع</th>
                                <th>الخدمة</th>
                                <th>السعر</th>
                                <th>الحالة</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr *ngFor="let o of orders">
                                <td>{{ o.creationTime | date:'yyyy-MM-dd HH:mm' }}</td>
                                <td>
                                    <span class="badge" [ngClass]="o.type === 1 ? 'bg-info' : 'bg-primary'">
                                      {{ o.type === 1 ? 'Radiology' : 'Other' }}
                                    </span>
                                </td>
                                <td class="fw-bold">{{ o.serviceName }}</td>
                                <td>{{ o.price | currency }}</td>
                                <td>
                                    <span class="badge" [ngClass]="{
                                        'bg-warning text-dark': o.status === 0,
                                        'bg-success': o.status === 2,
                                        'bg-danger': o.status === 3
                                    }">
                                      {{ o.status === 0 ? 'Pending' : (o.status === 2 ? 'Completed' : 'Status ' + o.status) }}
                                    </span>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <div *ngIf="orders.length === 0" class="text-center py-4 text-muted">
                        <i class="fas fa-inbox fa-2x mb-2"></i>
                        <p>لا توجد طلبات</p>
                    </div>
                </div>
             </div>

          </div>
        </div>

      </div>

      <!-- Add Order Modal -->
      <div class="modal fade show" id="orderModal" tabindex="-1" *ngIf="showOrderModal" style="display: block;">
        <div class="modal-dialog modal-lg">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">طلب جديد (Referral)</h5>
              <button type="button" class="btn-close" (click)="showOrderModal = false"></button>
            </div>
            <div class="modal-body">
               <div class="row g-3">
                  <div class="col-12">
                      <label class="form-label">النوع</label>
                      <select class="form-select" [(ngModel)]="newOrder.type">
                          <option [ngValue]="orderTypes.Radiology">Radiology (أشعة)</option>
                          <option [ngValue]="orderTypes.Lab">Lab (معمل)</option>
                      </select>
                  </div>
                  
                  <div class="col-12" *ngIf="newOrder.type === orderTypes.Radiology">
                      <label class="form-label">الفحص المطلوب</label>
                      <select class="form-select" [(ngModel)]="newOrder.serviceItemId">
                          <option [ngValue]="null">اختر الفحص...</option>
                          <option *ngFor="let item of radiologyItems" [ngValue]="item.id">
                              {{ item.name }} - {{ item.price }} S.R
                          </option>
                      </select>
                  </div>

                  <div class="col-12" *ngIf="newOrder.type === orderTypes.Lab">
                      <label class="form-label">الفحص المطلوب</label>
                      <select class="form-select" [(ngModel)]="newOrder.serviceItemId">
                          <option [ngValue]="null">اختر الفحص...</option>
                          <option *ngFor="let item of labItems" [ngValue]="item.id">
                              {{ item.name }} - {{ item.price }} S.R
                          </option>
                      </select>
                  </div>
                  
                  <div class="col-12">
                      <label class="form-label">ملاحظات سريرية</label>
                      <textarea class="form-control" rows="3" [(ngModel)]="newOrder.clinicalNotes"></textarea>
                  </div>
               </div>
            </div>
             <div class="modal-footer">
              <button type="button" class="btn btn-secondary" (click)="showOrderModal = false">إلغاء</button>
              <button type="button" class="btn btn-primary" (click)="saveOrder()" [disabled]="!newOrder.serviceItemId">حفظ الطلب</button>
            </div>
           </div>
        </div>
      </div>
      <div class="modal-backdrop fade show" *ngIf="showOrderModal" (click)="showOrderModal = false"></div>

      <!-- Keep existing Modals as they are functional -->
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
    /* General Card & Layout */
    .patient-header-card {
      border: none;
      border-radius: 12px;
      box-shadow: 0 4px 6px rgba(0,0,0,0.05);
      background: linear-gradient(135deg, #ffffff 0%, #f8f9fa 100%);
      overflow: hidden;
      margin-bottom: 1.5rem;
    }

    .patient-avatar {
      width: 80px;
      height: 80px;
      background: #e9ecef;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 2.5rem;
      color: #6c757d;
      border: 3px solid #fff;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    /* Stat Badge */
    .stat-badge {
      display: flex;
      align-items: center;
      padding: 0.75rem 1rem;
      border-radius: 8px;
      margin-bottom: 0.5rem;
      transition: transform 0.2s;
    }
    .stat-badge:hover { transform: translateY(-2px); }
    .stat-badge i { font-size: 1.5rem; margin-left: 0.75rem; opacity: 0.8; }
    .stat-badge .stat-info { display: flex; flex-direction: column; }
    .stat-badge .stat-value { font-weight: bold; font-size: 1.1rem; }
    .stat-badge .stat-label { font-size: 0.8rem; opacity: 0.9; }

    /* Vitals Grid - Default (Light Mode) */
    .vital-card {
      background: #ffffff;
      background: linear-gradient(to bottom right, #ffffff, #f8f9fa);
      border-radius: 12px;
      padding: 1.25rem;
      box-shadow: 0 4px 12px rgba(0,0,0,0.08);
      border-right: 5px solid transparent; 
      border-top: 1px solid rgba(0,0,0,0.05);
      color: #212529;
      height: 100%;
      position: relative;
      overflow: hidden;
      transition: all 0.2s ease;
    }
    .vital-card:hover { 
      transform: translateY(-5px); 
      box-shadow: 0 8px 15px rgba(0,0,0,0.1); 
    }
    
    .vital-icon {
      font-size: 1.8rem;
      margin-bottom: 0.5rem;
      opacity: 0.9;
    }
    .vital-value { font-size: 1.6rem; font-weight: 800; color: #343a40; }
    .vital-label { color: #6c757d; font-size: 0.9rem; font-weight: 500; }

    /* Custom Tabs */
    .nav-tabs { border-bottom: 2px solid #dee2e6; gap: 0.5rem; }
    .nav-link {
      border: none;
      border-bottom: 3px solid transparent;
      color: #6c757d;
      font-weight: 600;
      padding: 1rem 1.5rem;
      border-radius: 8px 8px 0 0;
      transition: all 0.2s;
    }
    .nav-link:hover { color: var(--lpx-theme-primary); background: #f8f9fa; }
    .nav-link.active {
      color: var(--lpx-theme-primary) !important;
      border-bottom-color: var(--lpx-theme-primary);
      background: rgba(var(--lpx-theme-primary-rgb), 0.05);
    }

    /* Modal Fixes */
    .modal.fade { display: block; opacity: 1; background: rgba(0,0,0,0.5); }
    .modal.show { display: block !important; }

    /* 
       DARK MODE OVERRIDES 
       Using :host-context to detect [data-theme="dark"] on body/html 
    */
    :host-context([data-theme="dark"]) .patient-header-card { 
      background: #212529; 
      color: #e9ecef;
      box-shadow: 0 4px 6px rgba(0,0,0,0.3);
    }
    
    :host-context([data-theme="dark"]) .vital-card { 
      background: #2c3035 !important; 
      border: 1px solid #373b3e; 
      box-shadow: 0 4px 6px rgba(0,0,0,0.3);
      color: #fff !important;
    }
    
    :host-context([data-theme="dark"]) .vital-value { color: #fff !important; }
    :host-context([data-theme="dark"]) .vital-label { color: #adb5bd !important; }
    :host-context([data-theme="dark"]) .vital-icon { opacity: 0.9; }

    :host-context([data-theme="dark"]) .card.shadow-sm { 
      background-color: #212529 !important;
      border: 1px solid #373b3e;
    }
    
    :host-context([data-theme="dark"]) .card-header.bg-white {
      background-color: #2c3035 !important;
      border-bottom: 1px solid #373b3e !important;
    }
    :host-context([data-theme="dark"]) .card-body.bg-white {
      background-color: #212529 !important;
      color: #e9ecef;
    }

    :host-context([data-theme="dark"]) .nav-link { color: #adb5bd; }
    :host-context([data-theme="dark"]) .nav-link:hover { background-color: #343a40; color: #fff; }
    :host-context([data-theme="dark"]) .nav-link.active { 
       background-color: #2c3035; 
       color: #fff !important; 
    }
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

  // Orders
  private medicalOrderService = inject(MedicalOrderService);
  private serviceItemService = inject(ServiceItemService);

  orders: any[] = [];
  radiologyItems: any[] = [];
  labItems: any[] = [];
  showOrderModal = false;
  newOrder: any = { type: OrderType.Radiology };

  orderTypes = OrderType;
  orderStatuses = OrderStatus;

  ngOnInit() {
    this.patientId = this.route.snapshot.paramMap.get('id') || '';
    if (this.patientId) {
      this.loadSummary();
      this.loadVitalSigns();
      this.loadDiagnoses();
      this.loadAllergies();
      this.loadMedicalHistories();
      this.loadMedicalHistories();
      this.loadPatientNotes();
      this.loadOrders();
      this.loadRadiologyItems();
      this.loadLabItems();
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

  loadOrders() {
    this.medicalOrderService.getList({} as any).subscribe(res => {
      // Filter by patient locally or via API if supported
      this.orders = res.items.filter((o: any) => o.patientId === this.patientId);
    });
  }

  loadRadiologyItems() {
    // 3 is Radiology
    this.serviceItemService.getList({} as any).subscribe(res => {
      this.radiologyItems = res.items.filter(x => x.category === 3);
    });
  }

  loadLabItems() {
    // 2 is Lab
    this.serviceItemService.getList({} as any).subscribe(res => {
      this.labItems = res.items.filter(x => x.category === 2);
    });
  }

  openOrderModal() {
    this.newOrder = {
      patientId: this.patientId,
      type: OrderType.Radiology, // Default to Radiology
      serviceItemId: null
    };
    this.showOrderModal = true;
  }

  saveOrder() {
    if (!this.newOrder.serviceItemId) return;

    this.medicalOrderService.create(this.newOrder).subscribe(() => {
      this.showOrderModal = false;
      this.loadOrders();
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
