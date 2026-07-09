import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CoreModule } from '@abp/ng.core';
import { ThemeSharedModule, ToasterService } from '@abp/ng.theme.shared';
import { PosService } from '../../proxy/pharmacy/pos.service';
import { PharmacySettingsService } from '../../proxy/settings/pharmacy-settings.service';
import {
  PosProductDto, PosSaleItemDto, PosInvoiceListDto, PosInvoiceItemDto,
  PosApproveDto, PosRejectDto, PosPartialRefundDto, PosRefundItemDto
} from '../../proxy/pharmacy/dtos/models';

declare var abp: any;

// Invoice Status values (mirror backend enum)
const STATUS = {
  Draft: 0,
  Paid: 3,
  Refunded: 6,
  PendingApproval: 7,
  Rejected: 8,
  Dispensed: 9
};

const STATUS_LABELS: Record<number, string> = {
  0: 'مسودة',
  1: 'صادرة',
  2: 'مدفوعة جزئياً',
  3: 'مدفوعة',
  4: 'ملغية',
  5: 'مؤجلة',
  6: 'مستردة',
  7: 'في انتظار الاعتماد',
  8: 'مرفوضة',
  9: 'تم الصرف'
};

const STATUS_CLASS: Record<number, string> = {
  0: 'bg-secondary',
  3: 'bg-success',
  6: 'bg-warning text-dark',
  7: 'bg-info text-dark',
  8: 'bg-danger',
  9: 'bg-primary'
};

interface CartItem extends PosProductDto {
  quantity: number;
}

interface RefundSelection {
  item: PosInvoiceItemDto;
  selected: boolean;
  returnQty: number;
}

type ActiveTab = 'new-sale' | 'pending-approval' | 'to-dispense' | 'return';

@Component({
  selector: 'app-pharmacy-pos',
  standalone: true,
  imports: [CommonModule, ThemeSharedModule, CoreModule, FormsModule],
  styles: [`
    .step-badge {
      width: 32px; height: 32px; border-radius: 50%;
      display: inline-flex; align-items: center; justify-content: center;
      font-weight: 700; font-size: 0.85rem;
    }
    .tab-pill {
      border-radius: 20px; padding: 6px 18px;
      border: 2px solid transparent; cursor: pointer;
      font-weight: 600; transition: all 0.2s;
    }
    .tab-pill.active { border-color: #0d6efd; background: #0d6efd; color: #fff; }
    .tab-pill:not(.active) { border-color: #dee2e6; color: #6c757d; }
    .tab-pill:not(.active):hover { border-color: #adb5bd; color: #343a40; }
    .workflow-step {
      display: flex; align-items: center; gap: 10px;
      padding: 12px 16px; border-radius: 10px; margin-bottom: 8px;
      border: 1px solid #e9ecef;
    }
    .workflow-step.active { background: #e7f3ff; border-color: #0d6efd; }
    .workflow-step.done { background: #d4edda; border-color: #28a745; }
    .invoice-card {
      border-radius: 12px; border: 1px solid #dee2e6;
      transition: box-shadow 0.2s;
    }
    .invoice-card:hover { box-shadow: 0 4px 16px rgba(0,0,0,0.1); }
    .refund-item-row { padding: 10px; border-radius: 8px; border: 1px solid #dee2e6; margin-bottom: 6px; }
    .refund-item-row.selected { background: #fff3cd; border-color: #ffc107; }
    .return-badge { background: #dc3545; color: white; padding: 2px 8px; border-radius: 8px; font-size: 0.75rem; }
  `],
  template: `
  <div class="container-fluid py-3">

    <!-- ─── Page Header ─── -->
    <div class="d-flex align-items-center justify-content-between mb-4">
      <div>
        <h4 class="mb-1 fw-bold"><i class="fas fa-cash-register text-primary me-2"></i>نقطة البيع - الصيدلية</h4>
        <small class="text-muted">إدارة المبيعات والارتجاع وفق مسار العمل المعتمد</small>
      </div>
      <div class="d-flex gap-2 flex-wrap justify-content-end">
        <button class="tab-pill" [class.active]="activeTab==='new-sale'" (click)="setTab('new-sale')">
          <i class="fas fa-plus me-1"></i> فاتورة جديدة
        </button>
        <button class="tab-pill" [class.active]="activeTab==='pending-approval'" (click)="setTab('pending-approval')">
          <i class="fas fa-clock me-1"></i> بانتظار الاعتماد
          <span *ngIf="pendingApprovalCount > 0" class="badge bg-warning text-dark ms-1">{{ pendingApprovalCount }}</span>
        </button>
        <button class="tab-pill" [class.active]="activeTab==='to-dispense'" (click)="setTab('to-dispense')">
          <i class="fas fa-pills me-1"></i> للصرف
          <span *ngIf="toDispenseCount > 0" class="badge bg-success ms-1">{{ toDispenseCount }}</span>
        </button>
        <button class="tab-pill" [class.active]="activeTab==='return'" (click)="setTab('return')">
          <i class="fas fa-undo me-1"></i> الارتجاع
        </button>
      </div>
    </div>

    <!-- ══════════════════════════════════════════════════════════ -->
    <!-- TAB 1: New Sale                                           -->
    <!-- ══════════════════════════════════════════════════════════ -->
    <div *ngIf="activeTab==='new-sale'">

      <!-- Workflow Steps Indicator -->
      <div class="card mb-3 border-0 shadow-sm">
        <div class="card-body py-3">
          <div class="d-flex align-items-center gap-3 flex-wrap">
            <div class="workflow-step flex-fill" [class.active]="saleStep==='cart'" [class.done]="saleStep==='review'||saleStep==='sent'">
              <span class="step-badge" [class.bg-primary]="saleStep==='cart'" [class.text-white]="saleStep==='cart'" [class.bg-success]="saleStep!=='cart'" [class.text-white]="saleStep!=='cart'">1</span>
              <div><div class="fw-bold small">إنشاء الفاتورة</div><div class="text-muted" style="font-size:0.75rem">إدخال الأصناف والكميات</div></div>
            </div>
            <i class="fas fa-arrow-left text-muted"></i>
            <div class="workflow-step flex-fill" [class.active]="saleStep==='review'" [class.done]="saleStep==='sent'">
              <span class="step-badge" [class.bg-primary]="saleStep==='review'" [class.text-white]="saleStep==='review'" [class.bg-success]="saleStep==='sent'" [class.text-white]="saleStep==='sent'" [class.bg-light]="saleStep==='cart'">2</span>
              <div><div class="fw-bold small">مراجعة الفاتورة</div><div class="text-muted" style="font-size:0.75rem">مراجعة البيانات والإجمالي</div></div>
            </div>
            <i class="fas fa-arrow-left text-muted"></i>
            <div class="workflow-step flex-fill" [class.active]="saleStep==='sent'">
              <span class="step-badge" [class.bg-info]="saleStep==='sent'" [class.text-white]="saleStep==='sent'" [class.bg-light]="saleStep!=='sent'">3</span>
              <div><div class="fw-bold small">إرسال للمحاسب</div><div class="text-muted" style="font-size:0.75rem">في انتظار الاعتماد</div></div>
            </div>
          </div>
        </div>
      </div>

      <!-- STEP 1: Cart -->
      <div *ngIf="saleStep==='cart'">
        <div class="row g-3">
          <!-- Left: Search & Cart -->
          <div class="col-lg-8">
            <!-- Search / Barcode -->
            <div class="card mb-3 shadow-sm">
              <div class="card-body">
                <div class="input-group">
                  <span class="input-group-text"><i class="fas fa-barcode"></i></span>
                  <input type="text" class="form-control" [(ngModel)]="searchQuery"
                    (keyup.enter)="searchOrScan()"
                    placeholder="امسح الباركود أو ابحث عن الدواء..."
                    autofocus>
                  <button class="btn btn-outline-secondary" (click)="toggleSearch()">
                    <i class="fas" [class.fa-search]="!showSearchResults" [class.fa-times]="showSearchResults"></i>
                  </button>
                  <button class="btn btn-primary" (click)="searchOrScan()">
                    <i class="fas fa-plus me-1"></i> إضافة
                  </button>
                </div>
                <!-- Search Results Dropdown -->
                <div *ngIf="showSearchResults && searchResults.length > 0"
                  class="border rounded mt-1 shadow-sm" style="max-height:200px;overflow-y:auto;">
                  <div *ngFor="let p of searchResults"
                    class="d-flex align-items-center justify-content-between px-3 py-2 border-bottom"
                    style="cursor:pointer" (click)="addProductToCart(p)">
                    <div>
                      <strong>{{ p.name }}</strong>
                      <small class="text-muted ms-2">{{ p.barcode }}</small>
                    </div>
                    <div class="d-flex align-items-center gap-2">
                      <span class="badge" [class.bg-success]="(p.currentStock||0)>0" [class.bg-danger]="(p.currentStock||0)<=0">
                        {{ p.currentStock || 0 }}
                      </span>
                      <strong class="text-primary">{{ p.price | number:'1.2-2' }} ج.م</strong>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Cart Table -->
            <div class="card shadow-sm">
              <div class="card-header bg-white d-flex justify-content-between align-items-center">
                <h6 class="mb-0 fw-bold"><i class="fas fa-shopping-cart text-primary me-2"></i>قائمة الأصناف</h6>
                <button *ngIf="cart.length > 0" class="btn btn-sm btn-outline-danger" (click)="clearCart()">
                  <i class="fas fa-trash me-1"></i>مسح الكل
                </button>
              </div>
              <div class="table-responsive">
                <table class="table align-middle mb-0">
                  <thead class="table-light">
                    <tr>
                      <th>الصنف</th>
                      <th class="text-center" width="100">المخزون</th>
                      <th class="text-center" width="100">السعر</th>
                      <th class="text-center" width="110">الكمية</th>
                      <th class="text-center" width="110">الإجمالي</th>
                      <th width="50"></th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr *ngFor="let item of cart; let i = index">
                      <td>
                        <div class="fw-semibold">{{ item.name }}</div>
                        <small class="text-muted">{{ item.barcode }}</small>
                      </td>
                      <td class="text-center">
                        <span class="badge" [class.bg-success]="(item.currentStock||0)>0" [class.bg-danger]="(item.currentStock||0)<=0">
                          {{ item.currentStock || 0 }}
                        </span>
                      </td>
                      <td class="text-center">{{ item.price | number:'1.2-2' }}</td>
                      <td>
                        <input type="number" class="form-control form-control-sm text-center"
                          [(ngModel)]="item.quantity" (change)="recalcTotal()" min="1"
                          [max]="item.currentStock || 9999" style="width:80px;margin:auto">
                      </td>
                      <td class="text-center fw-bold">{{ (item.price || 0) * item.quantity | number:'1.2-2' }}</td>
                      <td>
                        <button class="btn btn-sm btn-outline-danger" (click)="removeFromCart(i)">
                          <i class="fas fa-trash"></i>
                        </button>
                      </td>
                    </tr>
                    <tr *ngIf="cart.length === 0">
                      <td colspan="6" class="text-center py-5 text-muted">
                        <i class="fas fa-shopping-cart fa-2x mb-2 d-block"></i>
                        السلة فارغة - ابحث عن دواء وأضفه
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <!-- Right: Summary -->
          <div class="col-lg-4">
            <div class="card shadow-sm mb-3">
              <div class="card-body text-center py-4" style="background:linear-gradient(135deg,#0d6efd,#0a58ca);border-radius:inherit">
                <div class="text-white mb-1" style="font-size:0.85rem">الإجمالي</div>
                <div class="text-white fw-bold" style="font-size:2rem">{{ cartTotal | number:'1.2-2' }}</div>
                <div class="text-white-50" style="font-size:0.8rem">جنيه مصري</div>
              </div>
              <div class="card-body">
                <div class="mb-3">
                  <label class="form-label fw-semibold">اسم المريض (اختياري)</label>
                  <input type="text" class="form-control" [(ngModel)]="patientSearchName" placeholder="بحث عن مريض...">
                </div>
                <div class="mb-3">
                  <label class="form-label fw-semibold">ملاحظات</label>
                  <textarea class="form-control" [(ngModel)]="saleNotes" rows="2" placeholder="ملاحظات إضافية..."></textarea>
                </div>
                <button class="btn btn-success w-100 py-2 fw-bold" (click)="goToReview()" [disabled]="cart.length === 0">
                  <i class="fas fa-eye me-2"></i>مراجعة الفاتورة
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- STEP 2: Review -->
      <div *ngIf="saleStep==='review'">
        <div class="row g-3">
          <div class="col-lg-8">
            <div class="card shadow-sm">
              <div class="card-header bg-white">
                <h6 class="mb-0 fw-bold text-primary"><i class="fas fa-file-invoice me-2"></i>مراجعة الفاتورة قبل الإرسال</h6>
              </div>
              <div class="table-responsive">
                <table class="table align-middle mb-0">
                  <thead class="table-light">
                    <tr>
                      <th>الصنف</th>
                      <th class="text-center">الكمية</th>
                      <th class="text-center">السعر</th>
                      <th class="text-center">الإجمالي</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr *ngFor="let item of cart">
                      <td><strong>{{ item.name }}</strong></td>
                      <td class="text-center">{{ item.quantity }}</td>
                      <td class="text-center">{{ item.price | number:'1.2-2' }}</td>
                      <td class="text-center fw-bold">{{ (item.price || 0) * item.quantity | number:'1.2-2' }}</td>
                    </tr>
                  </tbody>
                  <tfoot class="table-light">
                    <tr>
                      <td colspan="3" class="text-start fw-bold">الإجمالي الكلي:</td>
                      <td class="text-center fw-bold text-success fs-5">{{ cartTotal | number:'1.2-2' }} ج.م</td>
                    </tr>
                  </tfoot>
                </table>
              </div>
              <div class="card-footer bg-white d-flex gap-2 justify-content-end">
                <button class="btn btn-outline-secondary" (click)="saleStep='cart'">
                  <i class="fas fa-arrow-right me-1"></i>تعديل
                </button>
                <button class="btn btn-primary px-4" (click)="submitForApproval()" [disabled]="isBusy">
                  <span *ngIf="isBusy" class="spinner-border spinner-border-sm me-1"></span>
                  <i class="fas fa-paper-plane me-1" *ngIf="!isBusy"></i>
                  إرسال للمحاسب
                </button>
              </div>
            </div>
          </div>
          <div class="col-lg-4">
            <div class="card shadow-sm border-warning">
              <div class="card-body">
                <h6 class="fw-bold text-warning"><i class="fas fa-info-circle me-2"></i>ملاحظات الإرسال</h6>
                <p class="small text-muted mb-0">بعد الإرسال ستنتقل الفاتورة لقائمة <strong>انتظار الاعتماد</strong> عند المحاسب. ستتمكن من صرف الأصناف فقط بعد موافقة المحاسب.</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- STEP 3: Sent -->
      <div *ngIf="saleStep==='sent'" class="text-center py-5">
        <div class="mb-4" style="font-size:4rem">✅</div>
        <h4 class="fw-bold text-success">تم إرسال الفاتورة للمحاسب بنجاح!</h4>
        <p class="text-muted mb-4">الفاتورة في انتظار مراجعة واعتماد المحاسب.</p>
        <div class="d-flex gap-2 justify-content-center">
          <button class="btn btn-outline-primary" (click)="setTab('pending-approval')">
            <i class="fas fa-clock me-1"></i>عرض الفواتير المعلقة
          </button>
          <button class="btn btn-success px-4" (click)="startNewSale()">
            <i class="fas fa-plus me-1"></i>فاتورة جديدة
          </button>
        </div>
      </div>
    </div>

    <!-- ══════════════════════════════════════════════════════════ -->
    <!-- TAB 2: Pending Approval (Accountant View)                -->
    <!-- ══════════════════════════════════════════════════════════ -->
    <div *ngIf="activeTab==='pending-approval'">
      <div class="d-flex justify-content-between align-items-center mb-3">
        <h6 class="fw-bold mb-0"><i class="fas fa-clock text-warning me-2"></i>الفواتير في انتظار الاعتماد</h6>
        <button class="btn btn-sm btn-outline-secondary" (click)="loadPendingApproval()">
          <i class="fas fa-sync me-1"></i>تحديث
        </button>
      </div>

      <div *ngIf="isLoading" class="text-center py-5">
        <div class="spinner-border text-primary"></div>
        <div class="mt-2 text-muted">جاري التحميل...</div>
      </div>

      <div *ngIf="!isLoading && pendingInvoices.length === 0" class="text-center py-5 text-muted">
        <i class="fas fa-check-circle fa-3x text-success mb-3 d-block"></i>
        لا توجد فواتير في انتظار الاعتماد
      </div>

      <div class="row g-3">
        <div class="col-md-6 col-xl-4" *ngFor="let inv of pendingInvoices">
          <div class="card invoice-card h-100">
            <div class="card-header bg-warning bg-opacity-10 d-flex justify-content-between">
              <span class="fw-bold">{{ inv.invoiceNumber }}</span>
              <span class="badge" [class]="getStatusClass(inv.status)">{{ getStatusLabel(inv.status) }}</span>
            </div>
            <div class="card-body">
              <div class="mb-2"><i class="fas fa-user text-muted me-1"></i>{{ inv.patientName }}</div>
              <div class="mb-2"><i class="fas fa-calendar text-muted me-1"></i>{{ inv.invoiceDate | date:'yyyy/MM/dd HH:mm' }}</div>
              <div class="mb-3">
                <small class="text-muted">الأصناف:</small>
                <ul class="mb-0 ps-3">
                  <li *ngFor="let item of inv.items" class="small">
                    {{ item.description }} × {{ item.quantity }} = {{ item.totalPrice | number:'1.2-2' }} ج.م
                  </li>
                </ul>
              </div>
              <div class="d-flex justify-content-between align-items-center">
                <span class="text-muted small">الإجمالي:</span>
                <strong class="text-primary fs-5">{{ inv.totalAmount | number:'1.2-2' }} ج.م</strong>
              </div>
            </div>
            <div class="card-footer bg-white">
              <!-- Approve Form -->
              <div *ngIf="approvingInvoiceId === inv.id" class="mb-2">
                <div class="mb-2">
                  <label class="form-label small fw-semibold">المبلغ المدفوع</label>
                  <input type="number" class="form-control form-control-sm" [(ngModel)]="approveDto.paidAmount" [min]="inv.totalAmount">
                </div>
                <div class="mb-2">
                  <label class="form-label small fw-semibold">طريقة الدفع</label>
                  <select class="form-select form-select-sm" [(ngModel)]="approveDto.paymentMethod">
                    <option [ngValue]="0">نقدي</option>
                    <option [ngValue]="1">بطاقة ائتمان</option>
                    <option [ngValue]="2">بطاقة مدى</option>
                    <option [ngValue]="3">تحويل بنكي</option>
                  </select>
                </div>
                <div class="d-flex gap-2">
                  <button class="btn btn-success btn-sm flex-fill" (click)="confirmApprove(inv)" [disabled]="isBusy">
                    <span *ngIf="isBusy" class="spinner-border spinner-border-sm me-1"></span>
                    تأكيد الاعتماد
                  </button>
                  <button class="btn btn-outline-secondary btn-sm" (click)="approvingInvoiceId=null">إلغاء</button>
                </div>
              </div>
              <!-- Reject Form -->
              <div *ngIf="rejectingInvoiceId === inv.id" class="mb-2">
                <div class="mb-2">
                  <label class="form-label small fw-semibold">سبب الرفض</label>
                  <textarea class="form-control form-control-sm" [(ngModel)]="rejectReason" rows="2" placeholder="اكتب سبب الرفض..."></textarea>
                </div>
                <div class="d-flex gap-2">
                  <button class="btn btn-danger btn-sm flex-fill" (click)="confirmReject(inv)" [disabled]="isBusy || !rejectReason">
                    <span *ngIf="isBusy" class="spinner-border spinner-border-sm me-1"></span>
                    تأكيد الرفض
                  </button>
                  <button class="btn btn-outline-secondary btn-sm" (click)="rejectingInvoiceId=null">إلغاء</button>
                </div>
              </div>
              <!-- Action Buttons -->
              <div *ngIf="approvingInvoiceId !== inv.id && rejectingInvoiceId !== inv.id" class="d-flex gap-2">
                <button class="btn btn-success btn-sm flex-fill" (click)="startApprove(inv)">
                  <i class="fas fa-check me-1"></i>موافقة
                </button>
                <button class="btn btn-danger btn-sm flex-fill" (click)="startReject(inv)">
                  <i class="fas fa-times me-1"></i>رفض
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Rejected Invoices (for pharmacist to re-edit) -->
      <div *ngIf="rejectedInvoices.length > 0" class="mt-4">
        <h6 class="fw-bold mb-3 text-danger"><i class="fas fa-times-circle me-2"></i>الفواتير المرفوضة - تحتاج تعديل</h6>
        <div class="row g-3">
          <div class="col-md-6 col-xl-4" *ngFor="let inv of rejectedInvoices">
            <div class="card invoice-card border-danger h-100">
              <div class="card-header bg-danger bg-opacity-10 d-flex justify-content-between">
                <span class="fw-bold">{{ inv.invoiceNumber }}</span>
                <span class="badge bg-danger">مرفوضة</span>
              </div>
              <div class="card-body">
                <div class="alert alert-danger small p-2 mb-2">
                  <i class="fas fa-exclamation-triangle me-1"></i>
                  <strong>سبب الرفض:</strong> {{ inv.rejectionReason }}
                </div>
                <div><i class="fas fa-user text-muted me-1"></i>{{ inv.patientName }}</div>
                <div class="mt-1"><strong class="text-danger">{{ inv.totalAmount | number:'1.2-2' }} ج.م</strong></div>
              </div>
              <div class="card-footer bg-white">
                <button class="btn btn-warning w-100 btn-sm" (click)="resubmitRejected(inv)" [disabled]="isBusy">
                  <i class="fas fa-paper-plane me-1"></i>إعادة الإرسال
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ══════════════════════════════════════════════════════════ -->
    <!-- TAB 3: To Dispense (Pharmacist)                          -->
    <!-- ══════════════════════════════════════════════════════════ -->
    <div *ngIf="activeTab==='to-dispense'">
      <div class="d-flex justify-content-between align-items-center mb-3">
        <h6 class="fw-bold mb-0"><i class="fas fa-pills text-success me-2"></i>الفواتير المعتمدة - جاهزة للصرف</h6>
        <button class="btn btn-sm btn-outline-secondary" (click)="loadToDispense()">
          <i class="fas fa-sync me-1"></i>تحديث
        </button>
      </div>

      <div *ngIf="isLoading" class="text-center py-5">
        <div class="spinner-border text-success"></div>
      </div>

      <div *ngIf="!isLoading && approvedInvoices.length === 0" class="text-center py-5 text-muted">
        <i class="fas fa-box-open fa-3x text-muted mb-3 d-block"></i>
        لا توجد فواتير جاهزة للصرف حالياً
      </div>

      <div class="row g-3">
        <div class="col-md-6 col-xl-4" *ngFor="let inv of approvedInvoices">
          <div class="card invoice-card border-success h-100">
            <div class="card-header bg-success bg-opacity-10 d-flex justify-content-between">
              <span class="fw-bold">{{ inv.invoiceNumber }}</span>
              <span class="badge bg-success">مدفوعة - جاهزة</span>
            </div>
            <div class="card-body">
              <div class="mb-2"><i class="fas fa-user text-muted me-1"></i>{{ inv.patientName }}</div>
              <div class="mb-2"><i class="fas fa-calendar text-muted me-1"></i>{{ inv.invoiceDate | date:'yyyy/MM/dd HH:mm' }}</div>
              <div class="mb-3">
                <small class="text-muted fw-semibold">الأصناف للصرف:</small>
                <ul class="mb-0 ps-3 mt-1">
                  <li *ngFor="let item of inv.items" class="small fw-semibold">
                    {{ item.description }} × <span class="text-primary">{{ item.quantity }}</span>
                  </li>
                </ul>
              </div>
              <div class="text-center">
                <strong class="text-primary fs-5">{{ inv.totalAmount | number:'1.2-2' }} ج.م</strong>
              </div>
            </div>
            <div class="card-footer bg-white d-flex gap-2">
              <button class="btn btn-success btn-sm flex-fill" (click)="confirmDispense(inv)" [disabled]="isBusy">
                <span *ngIf="isBusy" class="spinner-border spinner-border-sm me-1"></span>
                <i class="fas fa-check me-1" *ngIf="!isBusy"></i>
                تأكيد الصرف
              </button>
              <button class="btn btn-outline-secondary btn-sm" (click)="printInvoice(inv.id)">
                <i class="fas fa-print"></i>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ══════════════════════════════════════════════════════════ -->
    <!-- TAB 4: Return / Refund                                   -->
    <!-- ══════════════════════════════════════════════════════════ -->
    <div *ngIf="activeTab==='return'">
      <div class="row g-3">
        <div class="col-lg-5">
          <div class="card shadow-sm">
            <div class="card-header bg-white">
              <h6 class="mb-0 fw-bold text-danger"><i class="fas fa-undo me-2"></i>ارتجاع فاتورة</h6>
            </div>
            <div class="card-body">
              <div class="mb-3">
                <label class="form-label fw-semibold">رقم الفاتورة الأصلية</label>
                <div class="input-group">
                  <input type="text" class="form-control" [(ngModel)]="returnInvoiceNumber"
                    placeholder="مثال: POS-123456789" (keyup.enter)="loadInvoiceForReturn()">
                  <button class="btn btn-outline-primary" (click)="loadInvoiceForReturn()" [disabled]="isBusy">
                    <i class="fas fa-search"></i>
                  </button>
                </div>
              </div>

              <!-- Invoice for Return -->
              <div *ngIf="invoiceForReturn">
                <div class="alert alert-info small">
                  <div><strong>{{ invoiceForReturn.invoiceNumber }}</strong></div>
                  <div>{{ invoiceForReturn.patientName }}</div>
                  <div>{{ invoiceForReturn.invoiceDate | date:'yyyy/MM/dd' }}</div>
                  <div class="mt-1 fw-bold">الإجمالي: {{ invoiceForReturn.totalAmount | number:'1.2-2' }} ج.م</div>
                </div>

                <p class="fw-semibold mb-2">اختر الأصناف المراد إرجاعها:</p>

                <div *ngFor="let sel of returnSelections; let i = index"
                  class="refund-item-row" [class.selected]="sel.selected">
                  <div class="d-flex align-items-center gap-2">
                    <input type="checkbox" class="form-check-input mt-0" [(ngModel)]="sel.selected" style="width:18px;height:18px">
                    <div class="flex-fill">
                      <div class="fw-semibold small">{{ sel.item.description }}</div>
                      <div class="text-muted" style="font-size:0.75rem">الكمية الأصلية: {{ sel.item.quantity }}</div>
                    </div>
                    <div *ngIf="sel.selected" style="width:80px">
                      <input type="number" class="form-control form-control-sm text-center"
                        [(ngModel)]="sel.returnQty"
                        [min]="1" [max]="sel.item.quantity"
                        (change)="recalcRefundTotal()">
                    </div>
                    <div class="text-end" style="min-width:80px">
                      <span *ngIf="sel.selected" class="fw-bold text-danger small">
                        -{{ (sel.item.unitPrice || 0) * sel.returnQty | number:'1.2-2' }}
                      </span>
                    </div>
                  </div>
                </div>

                <div class="mt-3 d-flex justify-content-between align-items-center p-3 bg-warning bg-opacity-10 rounded">
                  <span class="fw-bold">إجمالي الارتجاع:</span>
                  <strong class="text-danger fs-5">{{ refundTotal | number:'1.2-2' }} ج.م</strong>
                </div>

                <div class="d-flex gap-2 mt-3">
                  <button class="btn btn-warning flex-fill" (click)="selectAllReturn()">الكل</button>
                  <button class="btn btn-outline-secondary flex-fill" (click)="clearReturnSelections()">إلغاء</button>
                </div>

                <button class="btn btn-danger w-100 mt-2 py-2 fw-bold"
                  (click)="processReturn()"
                  [disabled]="isBusy || getSelectedReturnCount() === 0">
                  <span *ngIf="isBusy" class="spinner-border spinner-border-sm me-1"></span>
                  <i class="fas fa-undo me-1" *ngIf="!isBusy"></i>
                  تأكيد الارتجاع وطباعة نسختين
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Return Result -->
        <div class="col-lg-7" *ngIf="refundResult">
          <div class="card border-warning shadow-sm">
            <div class="card-body text-center py-5">
              <div style="font-size:3.5rem">🧾</div>
              <h5 class="fw-bold text-success mt-3">تم إجراء الارتجاع بنجاح!</h5>
              <div class="mt-3 mb-4">
                <div class="text-muted small mb-1">رقم فاتورة الارتجاع</div>
                <div class="fw-bold fs-4 text-primary">{{ refundResult.refundInvoiceNumber }}</div>
                <div class="mt-2 text-muted small">المبلغ المرتجع</div>
                <div class="fw-bold fs-3 text-danger">{{ refundResult.refundAmount | number:'1.2-2' }} ج.م</div>
              </div>
              <div class="d-flex justify-content-center gap-3">
                <button class="btn btn-primary px-4" (click)="printReturnInvoice(refundResult.refundInvoiceId)" [disabled]="isBusy">
                  <i class="fas fa-print me-1"></i>طباعة نسختين (أصل وصورة)
                </button>
                <button class="btn btn-outline-secondary" (click)="resetReturn()">
                  <i class="fas fa-undo me-1"></i>ارتجاع جديد
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

  </div>
  `
})
export class PharmacyPosComponent implements OnInit {

  // ── State ────────────────────────────────────────────────────
  activeTab: ActiveTab = 'new-sale';
  saleStep: 'cart' | 'review' | 'sent' = 'cart';
  isLoading = false;
  isBusy = false;

  // ── Cart ─────────────────────────────────────────────────────
  cart: CartItem[] = [];
  cartTotal = 0;
  searchQuery = '';
  searchResults: PosProductDto[] = [];
  showSearchResults = false;
  patientSearchName = '';
  saleNotes = '';
  allowNegativeStock = false;
  currentDraftId: string | null = null;

  // ── Pending Approval ─────────────────────────────────────────
  pendingInvoices: PosInvoiceListDto[] = [];
  rejectedInvoices: PosInvoiceListDto[] = [];
  approvingInvoiceId: string | null = null;
  rejectingInvoiceId: string | null = null;
  approveDto: PosApproveDto = { paidAmount: 0, paymentMethod: 0 };
  rejectReason = '';

  // ── To Dispense ──────────────────────────────────────────────
  approvedInvoices: PosInvoiceListDto[] = [];

  // ── Return ───────────────────────────────────────────────────
  returnInvoiceNumber = '';
  invoiceForReturn: PosInvoiceListDto | null = null;
  returnSelections: RefundSelection[] = [];
  refundTotal = 0;
  refundResult: { refundInvoiceId: string; refundInvoiceNumber: string; refundAmount: number } | null = null;

  // ── Computed Counts ──────────────────────────────────────────
  get pendingApprovalCount() { return this.pendingInvoices.length; }
  get toDispenseCount() { return this.approvedInvoices.length; }

  constructor(
    private posService: PosService,
    private settingsService: PharmacySettingsService,
    private toaster: ToasterService
  ) {}

  ngOnInit() {
    this.settingsService.get().subscribe(s => { this.allowNegativeStock = s.allowNegativeStock; });
    this.loadAllQueues();
  }

  // ── Tab & Navigation ─────────────────────────────────────────

  setTab(tab: ActiveTab) {
    this.activeTab = tab;
    if (tab === 'pending-approval') { this.loadPendingApproval(); }
    if (tab === 'to-dispense') { this.loadToDispense(); }
  }

  loadAllQueues() {
    this.loadPendingApproval();
    this.loadToDispense();
  }

  // ── Cart Operations ──────────────────────────────────────────

  searchOrScan() {
    if (!this.searchQuery.trim()) return;

    // Try barcode first
    this.posService.getProductByBarcode(this.searchQuery).subscribe({
      next: (p) => { this.addProductToCart(p); this.searchQuery = ''; this.showSearchResults = false; },
      error: () => {
        // Fall back to search
        this.posService.searchProducts(this.searchQuery).subscribe({
          next: (results) => {
            this.searchResults = results;
            this.showSearchResults = true;
            if (results.length === 1) { this.addProductToCart(results[0]); this.searchQuery = ''; this.showSearchResults = false; }
          },
          error: () => this.toaster.error('لم يتم العثور على الدواء', 'خطأ')
        });
      }
    });
  }

  toggleSearch() {
    this.showSearchResults = !this.showSearchResults;
    if (!this.showSearchResults) { this.searchResults = []; }
  }

  addProductToCart(product: PosProductDto) {
    if ((product.currentStock || 0) <= 0 && !this.allowNegativeStock) {
      this.toaster.error('المخزون غير كافٍ', 'تنبيه');
      return;
    }
    const existing = this.cart.find(x => x.id === product.id);
    if (existing) {
      existing.quantity++;
    } else {
      this.cart.push({ ...product, quantity: 1 });
    }
    this.recalcTotal();
    this.showSearchResults = false;
  }

  removeFromCart(i: number) {
    this.cart.splice(i, 1);
    this.recalcTotal();
  }

  clearCart() {
    this.cart = [];
    this.cartTotal = 0;
  }

  recalcTotal() {
    this.cartTotal = this.cart.reduce((s, i) => s + (i.price || 0) * i.quantity, 0);
  }

  // ── Sale Workflow ─────────────────────────────────────────────

  goToReview() {
    if (this.cart.length === 0) return;
    this.saleStep = 'review';
  }

  submitForApproval() {
    this.isBusy = true;
    const payload: any = {
      items: this.cart.map(i => ({
        drugId: i.id,
        quantity: i.quantity,
        unitPrice: i.price,
        discount: 0
      })),
      totalAmount: this.cartTotal,
      paidAmount: 0,
      paymentMethod: 0,
      notes: this.saleNotes || null
    };

    this.posService.createDraft(payload).subscribe({
      next: (invoiceId) => {
        this.currentDraftId = invoiceId.replace(/"/g, '');
        this.posService.submitForApproval(this.currentDraftId).subscribe({
          next: () => {
            this.saleStep = 'sent';
            this.isBusy = false;
            this.loadAllQueues();
            abp.notify.success('تم إرسال الفاتورة للمحاسب', 'نجاح');
          },
          error: () => { this.isBusy = false; this.toaster.error('فشل الإرسال', 'خطأ'); }
        });
      },
      error: () => { this.isBusy = false; this.toaster.error('فشل إنشاء الفاتورة', 'خطأ'); }
    });
  }

  startNewSale() {
    this.cart = [];
    this.cartTotal = 0;
    this.saleNotes = '';
    this.patientSearchName = '';
    this.saleStep = 'cart';
    this.currentDraftId = null;
  }

  // ── Approval (Accountant) ────────────────────────────────────

  loadPendingApproval() {
    this.isLoading = true;
    this.posService.getPosInvoices(7 /* PendingApproval */).subscribe({
      next: (inv) => { this.pendingInvoices = inv; this.isLoading = false; },
      error: () => this.isLoading = false
    });
    this.posService.getPosInvoices(8 /* Rejected */).subscribe({
      next: (inv) => { this.rejectedInvoices = inv; },
      error: () => {}
    });
  }

  startApprove(inv: PosInvoiceListDto) {
    this.approvingInvoiceId = inv.id;
    this.rejectingInvoiceId = null;
    this.approveDto = { paidAmount: inv.totalAmount, paymentMethod: 0 };
  }

  confirmApprove(inv: PosInvoiceListDto) {
    this.isBusy = true;
    this.posService.approveAndPay(inv.id, this.approveDto).subscribe({
      next: () => {
        this.isBusy = false;
        this.approvingInvoiceId = null;
        abp.notify.success('تم اعتماد الفاتورة بنجاح', 'موافقة');
        this.loadPendingApproval();
        this.loadToDispense();
      },
      error: () => { this.isBusy = false; this.toaster.error('فشل الاعتماد', 'خطأ'); }
    });
  }

  startReject(inv: PosInvoiceListDto) {
    this.rejectingInvoiceId = inv.id;
    this.approvingInvoiceId = null;
    this.rejectReason = '';
  }

  confirmReject(inv: PosInvoiceListDto) {
    if (!this.rejectReason.trim()) return;
    this.isBusy = true;
    this.posService.reject(inv.id, { rejectionReason: this.rejectReason }).subscribe({
      next: () => {
        this.isBusy = false;
        this.rejectingInvoiceId = null;
        this.rejectReason = '';
        abp.notify.warn('تم رفض الفاتورة وإعادتها للصيدلي', 'رفض');
        this.loadPendingApproval();
      },
      error: () => { this.isBusy = false; this.toaster.error('فشل الرفض', 'خطأ'); }
    });
  }

  resubmitRejected(inv: PosInvoiceListDto) {
    this.isBusy = true;
    this.posService.submitForApproval(inv.id).subscribe({
      next: () => {
        this.isBusy = false;
        abp.notify.success('تمت إعادة إرسال الفاتورة للمحاسب', 'نجاح');
        this.loadPendingApproval();
      },
      error: () => { this.isBusy = false; }
    });
  }

  // ── Dispense (Pharmacist) ─────────────────────────────────────

  loadToDispense() {
    this.posService.getPosInvoices(3 /* Paid */).subscribe({
      next: (inv) => { this.approvedInvoices = inv; },
      error: () => {}
    });
  }

  confirmDispense(inv: PosInvoiceListDto) {
    this.isBusy = true;
    this.posService.dispense(inv.id).subscribe({
      next: () => {
        this.isBusy = false;
        abp.notify.success('تم صرف الأصناف بنجاح', 'صرف');
        this.loadToDispense();
        this.printInvoice(inv.id);
      },
      error: () => { this.isBusy = false; this.toaster.error('فشل الصرف', 'خطأ'); }
    });
  }

  printInvoice(id: string) {
    this.posService.getInvoicePdf(id).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      window.open(url, '_blank');
      setTimeout(() => URL.revokeObjectURL(url), 5000);
    });
  }

  // ── Return / Refund ──────────────────────────────────────────

  loadInvoiceForReturn() {
    if (!this.returnInvoiceNumber.trim()) return;
    this.isBusy = true;
    this.posService.getPosInvoices().subscribe({
      next: (invoices) => {
        const found = invoices.find(i =>
          i.invoiceNumber?.toLowerCase() === this.returnInvoiceNumber.trim().toLowerCase());
        this.isBusy = false;
        if (!found) { this.toaster.error('لم يتم العثور على الفاتورة', 'خطأ'); return; }
        if (found.invoiceType === 1) { this.toaster.error('لا يمكن ارتجاع فاتورة مرتجع', 'خطأ'); return; }
        if (found.status !== 3 && found.status !== 9) {
          this.toaster.error('لا يمكن ارتجاع فاتورة غير مكتملة الدفع', 'تنبيه'); return;
        }
        this.invoiceForReturn = found;
        this.returnSelections = (found.items || []).map(item => ({
          item, selected: false, returnQty: item.quantity || 1
        }));
        this.refundTotal = 0;
        this.refundResult = null;
      },
      error: () => { this.isBusy = false; this.toaster.error('خطأ في البحث', 'خطأ'); }
    });
  }

  selectAllReturn() {
    this.returnSelections.forEach(s => { s.selected = true; s.returnQty = s.item.quantity || 1; });
    this.recalcRefundTotal();
  }

  clearReturnSelections() {
    this.returnSelections.forEach(s => { s.selected = false; });
    this.refundTotal = 0;
  }

  recalcRefundTotal() {
    this.refundTotal = this.returnSelections
      .filter(s => s.selected)
      .reduce((sum, s) => sum + (s.item.unitPrice || 0) * s.returnQty, 0);
  }

  getSelectedReturnCount(): number {
    return this.returnSelections.filter(s => s.selected).length;
  }

  processReturn() {
    const selected = this.returnSelections.filter(s => s.selected);
    if (selected.length === 0) return;

    const payload: PosPartialRefundDto = {
      items: selected.map(s => ({
        invoiceItemId: s.item.id,
        returnQuantity: s.returnQty
      } as PosRefundItemDto))
    };

    this.isBusy = true;
    this.posService.partialRefund(this.invoiceForReturn!.id!, payload).subscribe({
      next: (result) => {
        this.isBusy = false;
        this.refundResult = result as any;
        abp.notify.success('تم إجراء الارتجاع بنجاح', 'ارتجاع');
        // Auto-print
        this.printReturnInvoice(result.refundInvoiceId!);
      },
      error: () => { this.isBusy = false; this.toaster.error('فشل إجراء الارتجاع', 'خطأ'); }
    });
  }

  printReturnInvoice(refundId: string) {
    this.posService.getReturnInvoicePdf(refundId).subscribe(blob => {
      const url = URL.createObjectURL(blob);
      window.open(url, '_blank');
      setTimeout(() => URL.revokeObjectURL(url), 5000);
    });
  }

  resetReturn() {
    this.returnInvoiceNumber = '';
    this.invoiceForReturn = null;
    this.returnSelections = [];
    this.refundTotal = 0;
    this.refundResult = null;
  }

  // ── Utilities ─────────────────────────────────────────────────

  getStatusLabel(status: number): string {
    return STATUS_LABELS[status] || 'غير معروف';
  }

  getStatusClass(status: number): string {
    return STATUS_CLASS[status] || 'bg-secondary';
  }
}

