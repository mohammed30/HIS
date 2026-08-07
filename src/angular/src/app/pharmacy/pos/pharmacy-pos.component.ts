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
import { InternalRequestService } from '../../proxy/inventory/internal-request.service';
import { InternalRequestDto } from '../../proxy/inventory/dtos/models';

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

type ActiveTab = 'new-sale' | 'pending-approval' | 'to-dispense' | 'return' | 'refunded-list' | 'dispensed-list' | 'pending-returns';

@Component({
  selector: 'app-pharmacy-pos',
  standalone: true,
  imports: [CommonModule, ThemeSharedModule, CoreModule, FormsModule],
  styles: [`
    @import url('https://fonts.googleapis.com/css2?family=Cairo:wght@400;600;700;800&family=Outfit:wght@400;500;700;800&display=swap');

    :host {
      /* Light Mode Colors (Default) */
      --pos-bg: #f8fafc;
      --pos-card: #ffffff;
      --pos-card2: #f1f5f9;
      --pos-border: #e2e8f0;
      --pos-accent: #0284c7;  /* Deeper blue for readability */
      --pos-accent2: #4f46e5; /* Indigo */
      --pos-success: #10b981;
      --pos-danger: #ef4444;
      --pos-warning: #f59e0b;
      --pos-text: #0f172a;
      --pos-muted: #64748b;
      font-family: 'Cairo', sans-serif;
    }

    :host-context([data-theme="dark"]) {
      /* Dark Mode Colors */
      --pos-bg: #0f1623;
      --pos-card: #1a2235;
      --pos-card2: #1f2a3d;
      --pos-border: #2a3a54;
      --pos-accent: #38bdf8;
      --pos-accent2: #818cf8;
      --pos-success: #34d399;
      --pos-danger: #f87171;
      --pos-warning: #fbbf24;
      --pos-text: #e2e8f0;
      --pos-muted: #94a3b8;
    }

    .pos-shell {
      min-height: 100vh;
      background: var(--pos-bg);
      padding: 1.5rem;
      color: var(--pos-text);
      font-family: 'Cairo', sans-serif;
    }

    /* ─── Header ─── */
    .pos-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 1.5rem;
      flex-wrap: wrap;
      gap: 1rem;
    }
    .pos-title {
      display: flex;
      align-items: center;
      gap: 1rem;
    }
    .pos-icon-wrap {
      width: 52px; height: 52px;
      background: linear-gradient(135deg, var(--pos-accent), var(--pos-accent2));
      border-radius: 14px;
      display: flex; align-items: center; justify-content: center;
      font-size: 1.4rem; color: #fff;
      box-shadow: 0 8px 20px rgba(56,189,248,0.3);
    }
    .pos-title h1 {
      font-size: 1.35rem; font-weight: 800; margin: 0;
      background: linear-gradient(135deg, var(--pos-accent), var(--pos-accent2));
      -webkit-background-clip: text; -webkit-text-fill-color: transparent;
    }
    .pos-title p { margin: 0; font-size: 0.82rem; color: var(--pos-muted); }

    /* ─── Tabs ─── */
    .pos-tabs {
      display: flex; gap: 0.5rem; flex-wrap: wrap;
    }
    .pos-tab {
      display: flex; align-items: center; gap: 0.4rem;
      padding: 0.55rem 1.2rem;
      border-radius: 12px;
      border: 1px solid var(--pos-border);
      background: var(--pos-card);
      color: var(--pos-muted);
      font-family: 'Cairo', sans-serif;
      font-weight: 600; font-size: 0.88rem;
      cursor: pointer;
      transition: all 0.25s ease;
    }
    .pos-tab:hover { border-color: var(--pos-accent); color: var(--pos-text); }
    .pos-tab.active {
      background: linear-gradient(135deg, rgba(56,189,248,0.15), rgba(129,140,248,0.15));
      border-color: var(--pos-accent);
      color: var(--pos-accent);
      box-shadow: 0 0 0 1px rgba(56,189,248,0.2);
    }
    .tab-badge {
      background: var(--pos-warning);
      color: #111; border-radius: 8px;
      padding: 1px 7px; font-size: 0.72rem; font-weight: 700;
    }
    .tab-badge.green { background: var(--pos-success); }

    /* ─── Workflow Steps ─── */
    .pos-steps {
      display: flex; align-items: center; gap: 0.5rem;
      background: var(--pos-card);
      border: 1px solid var(--pos-border);
      border-radius: 16px;
      padding: 1rem 1.5rem;
      margin-bottom: 1.5rem;
      flex-wrap: wrap;
    }
    .step-item {
      display: flex; align-items: center; gap: 0.75rem;
      flex: 1; min-width: 150px;
    }
    .step-num {
      width: 34px; height: 34px; border-radius: 50%;
      display: flex; align-items: center; justify-content: center;
      font-weight: 800; font-size: 0.9rem;
      background: var(--pos-card2); color: var(--pos-muted);
      border: 2px solid var(--pos-border);
      transition: all 0.3s;
      flex-shrink: 0;
    }
    .step-item.active .step-num {
      background: linear-gradient(135deg, var(--pos-accent), var(--pos-accent2));
      color: #fff; border-color: transparent;
      box-shadow: 0 0 16px rgba(56,189,248,0.4);
    }
    .step-item.done .step-num {
      background: var(--pos-success); color: #fff; border-color: transparent;
    }
    .step-label { font-size: 0.85rem; font-weight: 700; color: var(--pos-text); }
    .step-sub { font-size: 0.72rem; color: var(--pos-muted); }
    .step-arrow { color: var(--pos-border); font-size: 0.8rem; }

    /* ─── Cards ─── */
    .pos-card {
      background: var(--pos-card);
      border: 1px solid var(--pos-border);
      border-radius: 16px;
      overflow: hidden;
    }
    .pos-card-header {
      display: flex; align-items: center; justify-content: space-between;
      padding: 1rem 1.25rem;
      border-bottom: 1px solid var(--pos-border);
      background: var(--pos-card2);
    }
    .pos-card-title {
      font-weight: 700; font-size: 0.95rem;
      display: flex; align-items: center; gap: 0.5rem;
    }

    /* ─── Search Input ─── */
    .pos-search-group {
      display: flex; gap: 0;
      background: var(--pos-card2);
      border: 1px solid var(--pos-border);
      border-radius: 12px; overflow: hidden;
      transition: border-color 0.2s;
    }
    .pos-search-group:focus-within { border-color: var(--pos-accent); }
    .pos-search-icon {
      padding: 0 1rem; color: var(--pos-muted);
      display: flex; align-items: center;
    }
    .pos-search-input {
      flex: 1; background: transparent; border: none; outline: none;
      color: var(--pos-text); font-family: 'Cairo', sans-serif;
      font-size: 0.95rem; padding: 0.75rem 0;
    }
    .pos-search-input::placeholder { color: var(--pos-muted); }
    .pos-btn-add {
      background: linear-gradient(135deg, var(--pos-accent), var(--pos-accent2));
      border: none; color: #fff; padding: 0.75rem 1.5rem;
      font-family: 'Cairo', sans-serif; font-weight: 700; font-size: 0.88rem;
      cursor: pointer; transition: opacity 0.2s;
    }
    .pos-btn-add:hover { opacity: 0.9; }

    /* ─── Search Dropdown ─── */
    .search-dropdown {
      position: absolute; top: 100%; left: 0; right: 0; z-index: 999;
      background: var(--pos-card2);
      border: 1px solid var(--pos-border);
      border-top: none; border-radius: 0 0 12px 12px;
      max-height: 220px; overflow-y: auto;
      box-shadow: 0 12px 30px rgba(0,0,0,0.4);
      scrollbar-width: thin; scrollbar-color: var(--pos-border) transparent;
    }
    .search-item {
      display: flex; align-items: center; justify-content: space-between;
      padding: 0.65rem 1rem; cursor: pointer;
      border-bottom: 1px solid rgba(255,255,255,0.05);
      transition: background 0.15s;
    }
    .search-item:hover { background: rgba(56,189,248,0.08); }
    .search-item-name { font-weight: 700; font-size: 0.88rem; }
    .search-item-code { font-size: 0.75rem; color: var(--pos-muted); }
    .stock-badge {
      padding: 2px 10px; border-radius: 8px;
      font-size: 0.75rem; font-weight: 700;
    }
    .stock-ok { background: rgba(52,211,153,0.15); color: var(--pos-success); }
    .stock-low { background: rgba(248,113,113,0.15); color: var(--pos-danger); }

    /* ─── Cart Table ─── */
    .cart-table { width: 100%; border-collapse: collapse; }
    .cart-table th {
      padding: 0.65rem 1rem; font-size: 0.8rem; font-weight: 700;
      color: var(--pos-muted); text-transform: uppercase; letter-spacing: 0.04em;
      background: var(--pos-card2); border-bottom: 1px solid var(--pos-border);
    }
    .cart-table td {
      padding: 0.7rem 1rem; font-size: 0.9rem;
      border-bottom: 1px solid rgba(255,255,255,0.04);
      color: var(--pos-text);
    }
    .cart-table tr:hover td { background: rgba(56,189,248,0.04); }
    .cart-table tr:last-child td { border-bottom: none; }
    .item-name { font-weight: 700; font-size: 0.9rem; }
    .item-code { font-size: 0.73rem; color: var(--pos-muted); }
    .qty-input {
      width: 72px; background: var(--pos-card2);
      border: 1px solid var(--pos-border); border-radius: 8px;
      color: var(--pos-text); text-align: center; padding: 4px 8px;
      font-family: 'Outfit', sans-serif; font-weight: 700;
    }
    .qty-input:focus { outline: none; border-color: var(--pos-accent); }
    .cart-empty {
      text-align: center; padding: 3rem 1rem;
      color: var(--pos-muted); font-size: 0.9rem;
    }
    .cart-empty i { font-size: 3rem; margin-bottom: 1rem; opacity: 0.3; display: block; }

    /* ─── Summary Panel ─── */
    .summary-panel {
      display: flex; flex-direction: column; gap: 1rem;
    }
    .total-card {
      background: linear-gradient(135deg, #0ea5e9, #6366f1);
      border-radius: 16px; padding: 1.5rem; text-align: center;
      box-shadow: 0 8px 30px rgba(14,165,233,0.3);
    }
    .total-label { font-size: 0.82rem; color: rgba(255,255,255,0.7); margin-bottom: 0.25rem; }
    .total-value { font-family: 'Outfit', sans-serif; font-size: 2.4rem; font-weight: 800; color: #fff; }
    .total-currency { font-size: 0.82rem; color: rgba(255,255,255,0.6); margin-top: 0.2rem; }

    .pos-form-group { display: flex; flex-direction: column; gap: 0.4rem; }
    .pos-label { font-size: 0.82rem; font-weight: 700; color: var(--pos-muted); }
    .pos-input, .pos-textarea {
      background: var(--pos-card2); border: 1px solid var(--pos-border);
      border-radius: 10px; color: var(--pos-text); padding: 0.65rem 1rem;
      font-family: 'Cairo', sans-serif; font-size: 0.9rem;
      transition: border-color 0.2s; width: 100%; box-sizing: border-box;
    }
    .pos-input:focus, .pos-textarea:focus { outline: none; border-color: var(--pos-accent); }
    .pos-input::placeholder, .pos-textarea::placeholder { color: var(--pos-muted); }

    /* ─── Buttons ─── */
    .btn-pos-primary {
      background: linear-gradient(135deg, var(--pos-accent), var(--pos-accent2));
      border: none; border-radius: 12px; color: #fff;
      padding: 0.75rem 1.5rem; font-family: 'Cairo', sans-serif;
      font-weight: 700; font-size: 0.92rem; cursor: pointer;
      transition: all 0.2s; width: 100%;
    }
    .btn-pos-primary:hover { opacity: 0.9; transform: translateY(-1px); }
    .btn-pos-primary:disabled { opacity: 0.5; transform: none; cursor: not-allowed; }

    .btn-pos-success {
      background: linear-gradient(135deg, #059669, #34d399);
      border: none; border-radius: 10px; color: #fff;
      padding: 0.6rem 1.2rem; font-family: 'Cairo', sans-serif;
      font-weight: 700; font-size: 0.88rem; cursor: pointer;
      transition: all 0.2s;
    }
    .btn-pos-success:hover { opacity: 0.9; }
    .btn-pos-success:disabled { opacity: 0.5; cursor: not-allowed; }

    .btn-pos-danger {
      background: linear-gradient(135deg, #dc2626, #f87171);
      border: none; border-radius: 10px; color: #fff;
      padding: 0.6rem 1.2rem; font-family: 'Cairo', sans-serif;
      font-weight: 700; font-size: 0.88rem; cursor: pointer;
      transition: all 0.2s;
    }
    .btn-pos-danger:hover { opacity: 0.9; }

    .btn-pos-ghost {
      background: transparent; border: 1px solid var(--pos-border);
      border-radius: 10px; color: var(--pos-muted);
      padding: 0.6rem 1.2rem; font-family: 'Cairo', sans-serif;
      font-weight: 600; font-size: 0.88rem; cursor: pointer;
      transition: all 0.2s;
    }
    .btn-pos-ghost:hover { border-color: var(--pos-accent); color: var(--pos-accent); }

    .btn-icon {
      background: transparent; border: 1px solid var(--pos-border);
      border-radius: 8px; color: var(--pos-muted);
      padding: 0.4rem 0.65rem; cursor: pointer; transition: all 0.2s;
    }
    .btn-icon:hover { border-color: var(--pos-danger); color: var(--pos-danger); }

    /* ─── Invoice Cards (Queues) ─── */
    .inv-card {
      background: var(--pos-card);
      border: 1px solid var(--pos-border);
      border-radius: 14px; overflow: hidden;
      transition: box-shadow 0.25s, transform 0.25s;
    }
    .inv-card:hover { box-shadow: 0 8px 24px rgba(0,0,0,0.3); transform: translateY(-2px); }
    .inv-card-head {
      padding: 0.85rem 1rem;
      border-bottom: 1px solid var(--pos-border);
      display: flex; align-items: center; justify-content: space-between;
    }
    .inv-number { font-family: 'Outfit', sans-serif; font-weight: 700; font-size: 0.9rem; }
    .inv-status {
      padding: 3px 10px; border-radius: 8px;
      font-size: 0.72rem; font-weight: 700;
    }
    .status-pending { background: rgba(251,191,36,0.15); color: var(--pos-warning); }
    .status-paid { background: rgba(52,211,153,0.15); color: var(--pos-success); }
    .status-rejected { background: rgba(248,113,113,0.15); color: var(--pos-danger); }

    .inv-card-body { padding: 1rem; }
    .inv-meta { display: flex; align-items: center; gap: 0.4rem; font-size: 0.83rem; color: var(--pos-muted); margin-bottom: 0.4rem; }
    .inv-items { margin-top: 0.5rem; }
    .inv-item-line { font-size: 0.82rem; padding: 0.3rem 0; border-bottom: 1px solid rgba(255,255,255,0.04); }
    .inv-total {
      margin-top: 0.75rem; padding-top: 0.75rem;
      border-top: 1px solid var(--pos-border);
      display: flex; justify-content: space-between; align-items: center;
    }
    .inv-total-val { font-family: 'Outfit', sans-serif; font-size: 1.2rem; font-weight: 800; color: var(--pos-accent); }
    .inv-card-foot {
      padding: 0.75rem 1rem;
      border-top: 1px solid var(--pos-border);
      background: var(--pos-card2);
    }

    /* ─── Section headings ─── */
    .section-head {
      display: flex; align-items: center; justify-content: space-between;
      margin-bottom: 1.25rem;
    }
    .section-title {
      font-size: 1rem; font-weight: 800;
      display: flex; align-items: center; gap: 0.5rem;
    }

    /* ─── Approve/Reject inline form ─── */
    .inline-form { background: var(--pos-card2); border-radius: 10px; padding: 0.75rem; margin-bottom: 0.75rem; }
    .inline-label { font-size: 0.78rem; font-weight: 700; color: var(--pos-muted); margin-bottom: 0.3rem; }
    .inline-input, .inline-select {
      width: 100%; background: var(--pos-bg);
      border: 1px solid var(--pos-border); border-radius: 8px;
      color: var(--pos-text); padding: 0.5rem 0.75rem;
      font-family: 'Cairo', sans-serif; font-size: 0.88rem;
    }
    .inline-input:focus, .inline-select:focus { outline: none; border-color: var(--pos-accent); }
    .inline-select option { background: var(--pos-card); }

    /* ─── Return Module ─── */
    .return-item {
      background: var(--pos-card2); border: 1px solid var(--pos-border);
      border-radius: 10px; padding: 0.75rem; margin-bottom: 0.5rem;
      transition: border-color 0.2s;
    }
    .return-item.selected { border-color: var(--pos-warning); background: rgba(251,191,36,0.06); }
    .return-total-box {
      background: rgba(248,113,113,0.08); border: 1px solid rgba(248,113,113,0.2);
      border-radius: 12px; padding: 1rem;
      display: flex; justify-content: space-between; align-items: center;
      margin-top: 0.75rem;
    }
    .return-total-label { font-weight: 700; font-size: 0.9rem; }
    .return-total-val { font-family: 'Outfit', sans-serif; font-size: 1.4rem; font-weight: 800; color: var(--pos-danger); }

    /* ─── Success state ─── */
    .success-state {
      text-align: center; padding: 3rem 2rem;
    }
    .success-emoji { font-size: 4rem; margin-bottom: 1rem; }
    .success-title { font-size: 1.3rem; font-weight: 800; color: var(--pos-success); }

    /* ─── Utility ─── */
    .row-pos { display: flex; gap: 1.25rem; }
    .col-pos-8 { flex: 0 0 calc(65% - 0.625rem); }
    .col-pos-4 { flex: 0 0 calc(35% - 0.625rem); }
    @media(max-width: 900px) { .row-pos { flex-direction: column; } .col-pos-8, .col-pos-4 { flex: 1; } }
    .grid-3 { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 1rem; }
    .gap-2 { gap: 0.5rem; }
    .d-flex { display: flex; }
    .align-center { align-items: center; }
    .justify-between { justify-content: space-between; }
    .w-100 { width: 100%; }
    .mt-1 { margin-top: 0.25rem; }
    .mt-2 { margin-top: 0.5rem; }
    .mt-3 { margin-top: 0.75rem; }
    .mb-1 { margin-bottom: 0.25rem; }
    .mb-2 { margin-bottom: 0.5rem; }
    .mb-3 { margin-bottom: 0.75rem; }
    .mb-4 { margin-bottom: 1rem; }
    .text-sm { font-size: 0.82rem; }
    .text-muted-c { color: var(--pos-muted); }
    .fw-700 { font-weight: 700; }
    .fw-800 { font-weight: 800; }
    .text-center { text-align: center; }
    .text-danger { color: var(--pos-danger); }
    .text-success { color: var(--pos-success); }
    .text-accent { color: var(--pos-accent); }
    .pos-spinner { width: 20px; height: 20px; border: 2px solid rgba(255,255,255,0.3); border-top-color: #fff; border-radius: 50%; animation: spin 0.6s linear infinite; display: inline-block; vertical-align: middle; }
    @keyframes spin { to { transform: rotate(360deg); } }
    .rejection-alert { background: rgba(248,113,113,0.1); border: 1px solid rgba(248,113,113,0.3); border-radius: 10px; padding: 0.75rem; margin-bottom: 0.75rem; font-size: 0.83rem; }
    .p-1rem { padding: 1rem; }
  `],
  template: `
  <div class="pos-shell">

    <!-- ─── Header ─── -->
    <div class="pos-header">
      <div class="pos-title">
        <div class="pos-icon-wrap"><i class="fas fa-cash-register"></i></div>
        <div>
          <h1>نقطة البيع – الصيدلية</h1>
          <p>إدارة المبيعات والارتجاع وفق مسار العمل المعتمد</p>
        </div>
      </div>
      <div class="pos-tabs">
        <button class="pos-tab" [class.active]="activeTab==='new-sale'" (click)="setTab('new-sale')">
          <i class="fas fa-plus"></i> فاتورة جديدة
        </button>
        <button class="pos-tab" [class.active]="activeTab==='pending-approval'" (click)="setTab('pending-approval')">
          <i class="fas fa-clock"></i> الاعتماد
          <span *ngIf="pendingApprovalCount > 0" class="tab-badge">{{ pendingApprovalCount }}</span>
        </button>
        <button class="pos-tab" [class.active]="activeTab==='to-dispense'" (click)="setTab('to-dispense')">
          <i class="fas fa-pills"></i> للصرف
          <span *ngIf="toDispenseCount > 0" class="tab-badge green">{{ toDispenseCount }}</span>
        </button>
        <button class="pos-tab" [class.active]="activeTab==='return'" (click)="setTab('return')">
          <i class="fas fa-undo"></i> الارتجاع
        </button>
        <button class="pos-tab" [class.active]="activeTab==='refunded-list'" (click)="setTab('refunded-list')">
          <i class="fas fa-history"></i> الفواتير المرتجعة
        </button>
        <button class="pos-tab" [class.active]="activeTab==='dispensed-list'" (click)="setTab('dispensed-list')">
          <i class="fas fa-check-double"></i> تم الصرف
        </button>
        <button class="pos-tab" [class.active]="activeTab==='pending-returns'" (click)="setTab('pending-returns')">
          <i class="fas fa-undo"></i> طلبات مرتجعة
          <span *ngIf="pendingReturnsCount > 0" class="tab-badge green">{{ pendingReturnsCount }}</span>
        </button>
      </div>
    </div>

    <!-- Global Filter Bar for list views -->
    <div class="pos-card mb-3 p-1rem" style="display: flex; gap: 1rem; align-items: flex-end;" *ngIf="activeTab === 'pending-approval' || activeTab === 'to-dispense' || activeTab === 'refunded-list' || activeTab === 'dispensed-list' || activeTab === 'pending-returns'">
      <div style="flex:1">
        <label class="inline-label">بحث برقم الفاتورة</label>
        <input class="inline-input" type="text" [(ngModel)]="filterText" placeholder="POS-xxxx..." (keyup.enter)="applyFilters()">
      </div>
      <div style="flex:1">
        <label class="inline-label">من تاريخ</label>
        <input class="inline-input" type="date" [(ngModel)]="filterFromDate" (change)="applyFilters()">
      </div>
      <div style="flex:1">
        <label class="inline-label">إلى تاريخ</label>
        <input class="inline-input" type="date" [(ngModel)]="filterToDate" (change)="applyFilters()">
      </div>
      <div>
        <button class="btn-pos-ghost" style="border-color: var(--pos-accent); color: var(--pos-accent)" (click)="applyFilters()">
          <i class="fas fa-search"></i> بحث
        </button>
      </div>
    </div>

    <!-- ══════════════════════════════════════════════════════════ -->
    <!-- TAB 1: New Sale                                           -->
    <!-- ══════════════════════════════════════════════════════════ -->
    <div *ngIf="activeTab==='new-sale'">

      <!-- Workflow Steps -->
      <div class="pos-steps">
        <div class="step-item" [class.active]="saleStep==='cart'" [class.done]="saleStep==='review'||saleStep==='sent'">
          <div class="step-num">
            <i *ngIf="saleStep==='review'||saleStep==='sent'" class="fas fa-check" style="font-size:0.8rem"></i>
            <span *ngIf="saleStep==='cart'">1</span>
          </div>
          <div><div class="step-label">إنشاء الفاتورة</div><div class="step-sub text-muted-c">إدخال الأصناف والكميات</div></div>
        </div>
        <i class="fas fa-arrow-left step-arrow"></i>
        <div class="step-item" [class.active]="saleStep==='review'" [class.done]="saleStep==='sent'">
          <div class="step-num">
            <i *ngIf="saleStep==='sent'" class="fas fa-check" style="font-size:0.8rem"></i>
            <span *ngIf="saleStep!=='sent'">2</span>
          </div>
          <div><div class="step-label">مراجعة الفاتورة</div><div class="step-sub text-muted-c">مراجعة البيانات والإجمالي</div></div>
        </div>
        <i class="fas fa-arrow-left step-arrow"></i>
        <div class="step-item" [class.active]="saleStep==='sent'">
          <div class="step-num">3</div>
          <div><div class="step-label">إرسال للمحاسب</div><div class="step-sub text-muted-c">في انتظار الاعتماد</div></div>
        </div>
      </div>

      <!-- STEP 1: Cart -->
      <div *ngIf="saleStep==='cart'">
        <div class="row-pos">
          <!-- Search + Cart -->
          <div class="col-pos-8">
            <!-- Search -->
            <div class="pos-card mb-3">
              <div class="p-1rem" style="position:relative">
                <div class="pos-search-group">
                  <span class="pos-search-icon"><i class="fas fa-barcode"></i></span>
                  <input class="pos-search-input" [(ngModel)]="searchQuery"
                    (keyup.enter)="searchOrScan()"
                    placeholder="امسح الباركود أو ابحث عن الدواء..." autofocus>
                  <button class="pos-btn-add" (click)="searchOrScan()">
                    <i class="fas fa-plus"></i> إضافة
                  </button>
                </div>
                <!-- Dropdown -->
                <div *ngIf="showSearchResults && searchResults.length > 0" class="search-dropdown">
                  <div *ngFor="let p of searchResults" class="search-item" (click)="addProductToCart(p)">
                    <div>
                      <div class="search-item-name">{{ p.name }}</div>
                      <div class="search-item-code">{{ p.barcode }}</div>
                    </div>
                    <div class="d-flex align-center gap-2">
                      <span class="stock-badge" [class.stock-ok]="(p.currentStock||0)>0" [class.stock-low]="(p.currentStock||0)<=0">
                        {{ p.currentStock || 0 }}
                      </span>
                      <span class="fw-700 text-accent" style="font-family:'Outfit',sans-serif">{{ p.price | number:'1.2-2' }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Cart Table -->
            <div class="pos-card">
              <div class="pos-card-header">
                <span class="pos-card-title"><i class="fas fa-shopping-cart text-accent"></i> قائمة الأصناف</span>
                <button *ngIf="cart.length > 0" class="btn-pos-danger" style="padding:0.35rem 0.9rem;font-size:0.8rem" (click)="clearCart()">
                  <i class="fas fa-trash"></i> مسح
                </button>
              </div>
              <div style="overflow-x:auto">
                <table class="cart-table">
                  <thead>
                    <tr>
                      <th>الصنف</th>
                      <th class="text-center" style="width:80px">المخزون</th>
                      <th class="text-center" style="width:90px">السعر</th>
                      <th class="text-center" style="width:100px">الكمية</th>
                      <th class="text-center" style="width:100px">الإجمالي</th>
                      <th style="width:44px"></th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr *ngFor="let item of cart; let i = index">
                      <td>
                        <div class="item-name">{{ item.name }}</div>
                        <div class="item-code">{{ item.barcode }}</div>
                      </td>
                      <td class="text-center">
                        <span class="stock-badge" [class.stock-ok]="(item.currentStock||0)>0" [class.stock-low]="(item.currentStock||0)<=0">
                          {{ item.currentStock || 0 }}
                        </span>
                      </td>
                      <td class="text-center fw-700" style="font-family:'Outfit',sans-serif">{{ item.price | number:'1.2-2' }}</td>
                      <td class="text-center">
                        <input type="number" class="qty-input" [(ngModel)]="item.quantity"
                          (change)="recalcTotal()" min="1" [max]="item.currentStock || 9999">
                      </td>
                      <td class="text-center fw-800 text-accent" style="font-family:'Outfit',sans-serif">
                        {{ (item.price || 0) * item.quantity | number:'1.2-2' }}
                      </td>
                      <td>
                        <button class="btn-icon" (click)="removeFromCart(i)" title="حذف"><i class="fas fa-trash"></i></button>
                      </td>
                    </tr>
                    <tr *ngIf="cart.length === 0">
                      <td colspan="6" class="cart-empty">
                        <i class="fas fa-shopping-cart"></i>
                        السلة فارغة — ابحث عن دواء وأضفه
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <!-- Summary Panel -->
          <div class="col-pos-4">
            <div class="summary-panel">
              <div class="total-card">
                <div class="total-label">الإجمالي</div>
                <div class="total-value">{{ cartTotal | number:'1.2-2' }}</div>
                <div class="total-currency">جنيه مصري</div>
              </div>
              <div class="pos-card p-1rem">
                <div class="pos-form-group mb-3">
                  <label class="pos-label">اسم المريض (اختياري)</label>
                  <input type="text" class="pos-input" [(ngModel)]="patientSearchName" placeholder="بحث عن مريض...">
                </div>
                <div class="pos-form-group mb-3">
                  <label class="pos-label">ملاحظات</label>
                  <textarea class="pos-textarea" [(ngModel)]="saleNotes" rows="2" placeholder="ملاحظات إضافية..."></textarea>
                </div>
                <button class="btn-pos-primary" (click)="goToReview()" [disabled]="cart.length === 0">
                  <i class="fas fa-eye"></i> مراجعة الفاتورة
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- STEP 2: Review -->
      <div *ngIf="saleStep==='review'">
        <div class="row-pos">
          <div class="col-pos-8">
            <div class="pos-card">
              <div class="pos-card-header">
                <span class="pos-card-title"><i class="fas fa-file-invoice text-accent"></i> مراجعة الفاتورة</span>
              </div>
              <div style="overflow-x:auto">
                <table class="cart-table">
                  <thead>
                    <tr>
                      <th>الصنف</th>
                      <th class="text-center">الكمية</th>
                      <th class="text-center">السعر</th>
                      <th class="text-center">الإجمالي</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr *ngFor="let item of cart">
                      <td class="fw-700">{{ item.name }}</td>
                      <td class="text-center fw-700" style="font-family:'Outfit',sans-serif">{{ item.quantity }}</td>
                      <td class="text-center" style="font-family:'Outfit',sans-serif">{{ item.price | number:'1.2-2' }}</td>
                      <td class="text-center fw-800 text-accent" style="font-family:'Outfit',sans-serif">{{ (item.price || 0) * item.quantity | number:'1.2-2' }}</td>
                    </tr>
                  </tbody>
                  <tfoot>
                    <tr>
                      <td colspan="3" class="fw-700" style="padding:0.75rem 1rem;border-top:1px solid var(--pos-border)">الإجمالي الكلي:</td>
                      <td class="text-center fw-800 text-success" style="font-family:'Outfit',sans-serif;font-size:1.2rem;padding:0.75rem 1rem;border-top:1px solid var(--pos-border)">{{ cartTotal | number:'1.2-2' }} ج.م</td>
                    </tr>
                  </tfoot>
                </table>
              </div>
              <div style="padding:1rem;display:flex;gap:0.75rem;justify-content:flex-end;border-top:1px solid var(--pos-border)">
                <button class="btn-pos-ghost" (click)="saleStep='cart'"><i class="fas fa-arrow-right"></i> تعديل</button>
                <button class="btn-pos-primary" style="width:auto;padding:0.65rem 2rem" (click)="submitForApproval()" [disabled]="isBusy">
                  <span *ngIf="isBusy" class="pos-spinner"></span>
                  <i *ngIf="!isBusy" class="fas fa-paper-plane"></i> إرسال للمحاسب
                </button>
              </div>
            </div>
          </div>
          <div class="col-pos-4">
            <div class="pos-card p-1rem" style="border-color:rgba(251,191,36,0.3)">
              <div class="d-flex align-center gap-2 mb-2">
                <i class="fas fa-info-circle" style="color:var(--pos-warning)"></i>
                <span class="fw-700" style="color:var(--pos-warning)">ملاحظات الإرسال</span>
              </div>
              <p class="text-sm text-muted-c mb-1">بعد الإرسال ستنتقل الفاتورة لقائمة <strong style="color:var(--pos-text)">انتظار الاعتماد</strong> عند المحاسب.</p>
              <p class="text-sm text-muted-c mb-1">ستتمكن من صرف الأصناف فقط بعد موافقة المحاسب.</p>
            </div>
          </div>
        </div>
      </div>

      <!-- STEP 3: Sent -->
      <div *ngIf="saleStep==='sent'" class="pos-card">
        <div class="success-state">
          <div class="success-emoji">✅</div>
          <div class="success-title mb-2">تم إرسال الفاتورة للمحاسب بنجاح!</div>
          <p class="text-muted-c text-sm mb-3">الفاتورة في انتظار مراجعة واعتماد المحاسب.</p>
          <div class="d-flex align-center gap-2" style="justify-content:center">
            <button class="btn-pos-ghost" (click)="setTab('pending-approval')"><i class="fas fa-clock"></i> عرض المعلقة</button>
            <button class="btn-pos-success" (click)="startNewSale()"><i class="fas fa-plus"></i> فاتورة جديدة</button>
          </div>
        </div>
      </div>
    </div>

    <!-- ══════════════════════════════════════════════════════════ -->
    <!-- TAB 2: Pending Approval                                   -->
    <!-- ══════════════════════════════════════════════════════════ -->
    <div *ngIf="activeTab==='pending-approval'">
      <div class="section-head">
        <div class="section-title"><i class="fas fa-clock" style="color:var(--pos-warning)"></i> الفواتير في انتظار الاعتماد</div>
        <button class="btn-pos-ghost" (click)="loadPendingApproval()" style="padding:0.45rem 1rem"><i class="fas fa-sync"></i> تحديث</button>
      </div>

      <div *ngIf="isLoading" class="text-center" style="padding:3rem">
        <div class="pos-spinner" style="width:36px;height:36px;border-width:3px;margin:0 auto"></div>
        <div class="text-muted-c mt-2">جاري التحميل...</div>
      </div>

      <div *ngIf="!isLoading && pendingInvoices.length === 0" class="pos-card" style="text-align:center;padding:3rem">
        <i class="fas fa-check-circle" style="font-size:2.5rem;color:var(--pos-success);margin-bottom:0.75rem;display:block"></i>
        <div class="text-muted-c">لا توجد فواتير في انتظار الاعتماد</div>
      </div>

      <div class="grid-3">
        <div *ngFor="let inv of pendingInvoices" class="inv-card">
          <div class="inv-card-head">
            <span class="inv-number">{{ inv.invoiceNumber }}</span>
            <span class="inv-status status-pending">{{ getStatusLabel(inv.status) }}</span>
          </div>
          <div class="inv-card-body">
            <div class="inv-meta"><i class="fas fa-user"></i> {{ inv.patientName || 'بدون اسم' }}</div>
            <div class="inv-meta"><i class="fas fa-calendar-alt"></i> {{ inv.invoiceDate | date:'yyyy/MM/dd – HH:mm' }}</div>
            <div class="inv-items">
              <div class="text-sm text-muted-c mb-1">الأصناف:</div>
              <div *ngFor="let item of inv.items" class="inv-item-line">
                {{ item.description }} × {{ item.quantity }} =
                <span style="font-family:'Outfit',sans-serif" class="text-accent"> {{ item.totalPrice | number:'1.2-2' }} ج.م</span>
              </div>
            </div>
            <div class="inv-total">
              <span class="text-sm text-muted-c">الإجمالي</span>
              <span class="inv-total-val">{{ inv.totalAmount | number:'1.2-2' }} ج.م</span>
            </div>
          </div>
          <div class="inv-card-foot">
            <!-- Approve Form -->
            <div *ngIf="approvingInvoiceId === inv.id" class="inline-form">
              <div class="inline-label mb-1">المبلغ المدفوع</div>
              <input type="number" class="inline-input mb-2" [(ngModel)]="approveDto.paidAmount" [min]="inv.totalAmount">
              <div class="inline-label mb-1">طريقة الدفع</div>
              <select class="inline-select mb-2" [(ngModel)]="approveDto.paymentMethod">
                <option [ngValue]="0">نقدي</option>
                <option [ngValue]="1">بطاقة ائتمان</option>
                <option [ngValue]="2">بطاقة مدى</option>
                <option [ngValue]="3">تحويل بنكي</option>
              </select>
              <div class="d-flex gap-2">
                <button class="btn-pos-success w-100" (click)="confirmApprove(inv)" [disabled]="isBusy">
                  <span *ngIf="isBusy" class="pos-spinner"></span> تأكيد الاعتماد
                </button>
                <button class="btn-pos-ghost" (click)="approvingInvoiceId=null">إلغاء</button>
              </div>
            </div>
            <!-- Reject Form -->
            <div *ngIf="rejectingInvoiceId === inv.id" class="inline-form">
              <div class="inline-label mb-1">سبب الرفض</div>
              <textarea class="inline-input mb-2" [(ngModel)]="rejectReason" rows="2" placeholder="اكتب سبب الرفض..."></textarea>
              <div class="d-flex gap-2">
                <button class="btn-pos-danger w-100" (click)="confirmReject(inv)" [disabled]="isBusy || !rejectReason">
                  <span *ngIf="isBusy" class="pos-spinner"></span> تأكيد الرفض
                </button>
                <button class="btn-pos-ghost" (click)="rejectingInvoiceId=null">إلغاء</button>
              </div>
            </div>
            <!-- Action Buttons -->
            <div *ngIf="approvingInvoiceId !== inv.id && rejectingInvoiceId !== inv.id" class="d-flex gap-2">
              <button class="btn-pos-success w-100" (click)="startApprove(inv)"><i class="fas fa-check"></i> موافقة</button>
              <button class="btn-pos-danger w-100" (click)="startReject(inv)"><i class="fas fa-times"></i> رفض</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Rejected Invoices -->
      <div *ngIf="rejectedInvoices.length > 0" class="mt-3">
        <div class="section-title mb-2" style="font-size:0.9rem"><i class="fas fa-times-circle text-danger"></i> الفواتير المرفوضة</div>
        <div class="grid-3">
          <div *ngFor="let inv of rejectedInvoices" class="inv-card" style="border-color:rgba(248,113,113,0.3)">
            <div class="inv-card-head">
              <span class="inv-number">{{ inv.invoiceNumber }}</span>
              <span class="inv-status status-rejected">مرفوضة</span>
            </div>
            <div class="inv-card-body">
              <div class="rejection-alert"><i class="fas fa-exclamation-triangle text-danger"></i> <strong>سبب الرفض:</strong> {{ inv.rejectionReason }}</div>
              <div class="inv-meta"><i class="fas fa-user"></i> {{ inv.patientName }}</div>
              <div class="inv-total">
                <span class="text-sm text-muted-c">الإجمالي</span>
                <span class="text-danger fw-800" style="font-family:'Outfit',sans-serif;font-size:1.1rem">{{ inv.totalAmount | number:'1.2-2' }} ج.م</span>
              </div>
            </div>
            <div class="inv-card-foot">
              <button class="btn-pos-primary" (click)="resubmitRejected(inv)" [disabled]="isBusy">
                <span *ngIf="isBusy" class="pos-spinner"></span>
                <i *ngIf="!isBusy" class="fas fa-paper-plane"></i> إعادة الإرسال
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ══════════════════════════════════════════════════════════ -->
    <!-- TAB: Refunded Invoices List                               -->
    <!-- ══════════════════════════════════════════════════════════ -->
    <div *ngIf="activeTab==='refunded-list'">
      <div class="section-head">
        <div class="section-title"><i class="fas fa-history" style="color:var(--pos-danger)"></i> الفواتير المرتجعة</div>
        <button class="btn-pos-ghost" (click)="loadRefundedInvoices()" style="padding:0.45rem 1rem"><i class="fas fa-sync"></i> تحديث</button>
      </div>

      <div *ngIf="isLoading" class="text-center" style="padding:3rem">
        <div class="pos-spinner" style="width:36px;height:36px;border-width:3px;margin:0 auto;border-top-color:var(--pos-danger)"></div>
      </div>
      <div *ngIf="!isLoading && refundedInvoices.length === 0" class="pos-card" style="text-align:center;padding:3rem">
        <i class="fas fa-folder-open" style="font-size:2.5rem;color:var(--pos-muted);margin-bottom:0.75rem;display:block;opacity:0.4"></i>
        <div class="text-muted-c">لا توجد فواتير مرتجعة حالياً</div>
      </div>

      <div class="grid-4" *ngIf="!isLoading && refundedInvoices.length > 0">
        <div *ngFor="let inv of refundedInvoices" class="inv-card">
          <div class="inv-card-head">
            <span class="inv-number">{{ inv.invoiceNumber }}</span>
            <span class="inv-status status-rejected">مرتجع</span>
          </div>
          <div class="inv-card-body">
            <div class="inv-meta"><i class="fas fa-user"></i> {{ inv.patientName || 'بدون اسم' }}</div>
            <div class="inv-meta"><i class="fas fa-calendar-alt"></i> {{ inv.invoiceDate | date:'yyyy/MM/dd – HH:mm' }}</div>
            <div class="inv-meta"><i class="fas fa-file-invoice"></i> الأصل: {{ inv.originalInvoiceNumber }}</div>
            <div class="inv-items">
              <div class="text-sm text-muted-c mb-1">الأصناف المرتجعة:</div>
              <div *ngFor="let item of inv.items" class="inv-item-line">
                {{ item.description }} × {{ item.quantity }} =
                <span style="font-family:'Outfit',sans-serif" class="text-danger"> {{ item.totalPrice | number:'1.2-2' }} ج.م</span>
              </div>
            </div>
            <div class="inv-total">
              <span class="text-sm text-muted-c">إجمالي المرتجع</span>
              <span class="inv-total-val text-danger">{{ inv.totalAmount | number:'1.2-2' }} ج.م</span>
            </div>
          </div>
          <div class="inv-card-foot d-flex gap-2">
            <button class="btn-pos-ghost w-100" (click)="printReturnInvoice(inv.id)"><i class="fas fa-print"></i> طباعة الإيصال</button>
          </div>
        </div>
      </div>
    </div>

    <!-- ══════════════════════════════════════════════════════════ -->
    <!-- TAB: Dispensed Invoices List                               -->
    <!-- ══════════════════════════════════════════════════════════ -->
    <div *ngIf="activeTab==='dispensed-list'">
      <div class="section-head">
        <div class="section-title"><i class="fas fa-check-double" style="color:var(--pos-accent)"></i> الفواتير المصروفة</div>
        <button class="btn-pos-ghost" (click)="loadDispensedInvoices()" style="padding:0.45rem 1rem"><i class="fas fa-sync"></i> تحديث</button>
      </div>

      <div *ngIf="isLoading" class="text-center" style="padding:3rem">
        <div class="pos-spinner" style="width:36px;height:36px;border-width:3px;margin:0 auto;border-top-color:var(--pos-accent)"></div>
      </div>
      <div *ngIf="!isLoading && dispensedInvoices.length === 0" class="pos-card" style="text-align:center;padding:3rem">
        <i class="fas fa-folder-open" style="font-size:2.5rem;color:var(--pos-muted);margin-bottom:0.75rem;display:block;opacity:0.4"></i>
        <div class="text-muted-c">لا توجد فواتير مصروفة حالياً</div>
      </div>

      <div class="grid-4" *ngIf="!isLoading && dispensedInvoices.length > 0">
        <div *ngFor="let inv of dispensedInvoices" class="inv-card">
          <div class="inv-card-head">
            <span class="inv-number">{{ inv.invoiceNumber }}</span>
            <span class="inv-status status-paid">تم الصرف</span>
          </div>
          <div class="inv-card-body">
            <div class="inv-meta"><i class="fas fa-user"></i> {{ inv.patientName || 'عميل نقدي' }}</div>
            <div class="inv-meta"><i class="fas fa-calendar-alt"></i> {{ inv.invoiceDate | date:'yyyy/MM/dd – HH:mm' }}</div>
            <div class="inv-items">
              <div class="text-sm text-muted-c mb-1">الأصناف:</div>
              <div *ngFor="let item of inv.items" class="inv-item-line">
                {{ item.productName || item.description }} × {{ item.quantity }} =
                <span style="font-family:'Outfit',sans-serif" class="text-accent"> {{ item.totalPrice | number:'1.2-2' }} ج.م</span>
              </div>
            </div>
            <div class="inv-total">
              <span class="text-sm text-muted-c">الإجمالي</span>
              <span class="inv-total-val text-accent">{{ inv.totalAmount | number:'1.2-2' }} ج.م</span>
            </div>
          </div>
          <div class="inv-card-foot d-flex gap-2">
            <button class="btn-pos-ghost w-100" (click)="printInvoice(inv.id)"><i class="fas fa-print"></i> الإيصال</button>
          </div>
        </div>
      </div>
    </div>

    <!-- ══════════════════════════════════════════════════════════ -->
        <!-- ────────────────────────────────────────────────────────── -->
    <!-- TAB: Pending Returns                                       -->
    <!-- ────────────────────────────────────────────────────────── -->
    <div *ngIf="activeTab==='pending-returns'">
      <div class="section-head">
        <div class="section-title"><i class="fas fa-undo" style="color:var(--pos-accent)"></i> طلبات المرتجعات الداخلية</div>
        <button class="btn-pos-ghost" (click)="loadPendingReturns()" style="padding:0.45rem 1rem"><i class="fas fa-sync"></i> تحديث</button>
      </div>

      <div *ngIf="isLoading" class="text-center" style="padding:3rem">
        <div class="pos-spinner" style="width:36px;height:36px;border-width:3px;margin:0 auto;border-top-color:var(--pos-accent)"></div>
      </div>
      <div *ngIf="!isLoading && pendingReturns.length === 0" class="pos-card" style="text-align:center;padding:3rem">
        <i class="fas fa-folder-open" style="font-size:2.5rem;color:var(--pos-muted);margin-bottom:0.75rem;display:block;opacity:0.4"></i>
        <div class="text-muted-c">لا توجد طلبات مرتجعة حالياً</div>
      </div>

      <div class="grid-4" *ngIf="!isLoading && pendingReturns.length > 0">
        <div *ngFor="let req of pendingReturns" class="inv-card">
          <div class="inv-card-head">
            <span class="inv-number">{{ req.requestNumber }}</span>
            <span class="inv-status status-pending">بانتظار الموافقة</span>
          </div>
          <div class="inv-card-body">
            <div class="inv-meta"><i class="fas fa-user"></i> {{ req.patientName || 'غير معروف' }}</div>
            <div class="inv-meta"><i class="fas fa-calendar-alt"></i> {{ req.requestDate | date:'yyyy/MM/dd – HH:mm' }}</div>
            <div class="inv-items">
              <div class="text-sm text-muted-c mb-1">الأصناف المرتجعة:</div>
              <div *ngFor="let line of req.lines" class="inv-item-line">
                {{ line.inventoryItemName }} × {{ line.returnQuantity }}
              </div>
            </div>
            <div class="inv-total mt-2">
              <span class="text-sm text-muted-c">ملاحظات:</span>
              <span class="inv-total-val" style="font-size: 0.85rem">{{ req.notes }}</span>
            </div>
          </div>
          <div class="inv-card-foot d-flex gap-2">
            <button class="btn-pos-primary w-100" (click)="approvePendingReturn(req.id)" [disabled]="isBusy"><i class="fas fa-check"></i> موافقة واستلام</button>
          </div>
        </div>
      </div>
    </div>

      <!-- TAB 3: To Dispense                                        -->
    <!-- ══════════════════════════════════════════════════════════ -->
    <div *ngIf="activeTab==='to-dispense'">
      <div class="section-head">
        <div class="section-title"><i class="fas fa-pills" style="color:var(--pos-success)"></i> الفواتير المعتمدة – جاهزة للصرف</div>
        <button class="btn-pos-ghost" (click)="loadToDispense()" style="padding:0.45rem 1rem"><i class="fas fa-sync"></i> تحديث</button>
      </div>

      <div *ngIf="isLoading" class="text-center" style="padding:3rem">
        <div class="pos-spinner" style="width:36px;height:36px;border-width:3px;margin:0 auto;border-top-color:var(--pos-success)"></div>
      </div>
      <div *ngIf="!isLoading && approvedInvoices.length === 0" class="pos-card" style="text-align:center;padding:3rem">
        <i class="fas fa-box-open" style="font-size:2.5rem;color:var(--pos-muted);margin-bottom:0.75rem;display:block;opacity:0.4"></i>
        <div class="text-muted-c">لا توجد فواتير جاهزة للصرف حالياً</div>
      </div>

      <div class="grid-3">
        <div *ngFor="let inv of approvedInvoices" class="inv-card" style="border-color:rgba(52,211,153,0.2)">
          <div class="inv-card-head" [style.background]="inv.status === 9 ? 'rgba(56,189,248,0.06)' : 'rgba(52,211,153,0.06)'">
            <span class="inv-number">{{ inv.invoiceNumber }}</span>
            <span class="inv-status" [ngClass]="{'status-paid': inv.status !== 9, 'status-pending': inv.status === 9}" [style.background]="inv.status === 9 ? 'rgba(56,189,248,0.15)' : ''" [style.color]="inv.status === 9 ? 'var(--pos-accent)' : ''">
              {{ inv.status === 9 ? 'تم الصرف' : 'مدفوعة – جاهزة' }}
            </span>
          </div>
          <div class="inv-card-body">
            <div class="inv-meta"><i class="fas fa-user"></i> {{ inv.patientName || 'بدون اسم' }}</div>
            <div class="inv-meta"><i class="fas fa-calendar-alt"></i> {{ inv.invoiceDate | date:'yyyy/MM/dd – HH:mm' }}</div>
            <div class="inv-items">
              <div class="text-sm fw-700 text-muted-c mb-1">الأصناف للصرف:</div>
              <div *ngFor="let item of inv.items" class="inv-item-line">
                <span class="fw-700" [ngStyle]="{'text-decoration': inv.status === 9 ? 'line-through' : 'none'}">{{ item.description }}</span> × <span class="text-accent fw-800" style="font-family:'Outfit',sans-serif">{{ item.quantity }}</span>
              </div>
            </div>
            <div class="inv-total">
              <span class="text-sm text-muted-c">الإجمالي</span>
              <span class="inv-total-val">{{ inv.totalAmount | number:'1.2-2' }} ج.م</span>
            </div>
          </div>
          <div class="inv-card-foot">
            <div class="d-flex gap-2" *ngIf="inv.status !== 9">
              <button class="btn-pos-success w-100" (click)="confirmDispense(inv)" [disabled]="isBusy">
                <span *ngIf="isBusy" class="pos-spinner"></span>
                <i *ngIf="!isBusy" class="fas fa-check"></i> تأكيد الصرف
              </button>
              <button class="btn-pos-ghost" (click)="printInvoice(inv.id)" title="طباعة" style="padding:0.6rem 0.9rem">
                <i class="fas fa-print"></i>
              </button>
            </div>
            <div class="d-flex gap-2 align-center justify-center w-100" *ngIf="inv.status === 9" style="padding:0.5rem">
              <div class="text-accent fw-800"><i class="fas fa-check-circle"></i> تم صرف الفاتورة</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- ══════════════════════════════════════════════════════════ -->
    <!-- TAB 4: Return / Refund                                    -->
    <!-- ══════════════════════════════════════════════════════════ -->
    <div *ngIf="activeTab==='return'">
      <div class="row-pos">
        <div class="col-pos-4">
          <div class="pos-card">
            <div class="pos-card-header">
              <span class="pos-card-title"><i class="fas fa-undo text-danger"></i> ارتجاع فاتورة</span>
            </div>
            <div class="p-1rem">
              <div class="pos-form-group mb-3">
                <label class="pos-label">رقم الفاتورة الأصلية</label>
                <div class="pos-search-group">
                  <input class="pos-search-input" [(ngModel)]="returnInvoiceNumber"
                    placeholder="مثال: POS-123456789" (keyup.enter)="loadInvoiceForReturn()">
                  <button class="pos-btn-add" (click)="loadInvoiceForReturn()" [disabled]="isBusy">
                    <i class="fas fa-search"></i>
                  </button>
                </div>
              </div>

              <!-- Invoice Info -->
              <div *ngIf="invoiceForReturn">
                <div style="background:rgba(56,189,248,0.08);border:1px solid rgba(56,189,248,0.2);border-radius:10px;padding:0.75rem;margin-bottom:0.75rem">
                  <div class="fw-700 text-accent mb-1">{{ invoiceForReturn.invoiceNumber }}</div>
                  <div class="text-sm text-muted-c">{{ invoiceForReturn.patientName }}</div>
                  <div class="text-sm text-muted-c">{{ invoiceForReturn.invoiceDate | date:'yyyy/MM/dd' }}</div>
                  <div class="fw-800 text-accent mt-1" style="font-family:'Outfit',sans-serif">{{ invoiceForReturn.totalAmount | number:'1.2-2' }} ج.م</div>
                </div>

                <div class="fw-700 mb-2 text-sm">اختر الأصناف المراد إرجاعها:</div>

                <div *ngFor="let sel of returnSelections; let i = index" class="return-item" [class.selected]="sel.selected">
                  <div class="d-flex align-center gap-2">
                    <input type="checkbox" [(ngModel)]="sel.selected" (ngModelChange)="recalcRefundTotal()" style="width:18px;height:18px;cursor:pointer;accent-color:var(--pos-warning)">
                    <div style="flex:1">
                      <div class="fw-700 text-sm">{{ sel.item.description }}</div>
                      <div class="text-sm text-muted-c">الكمية الأصلية: {{ sel.item.quantity }}</div>
                    </div>
                    <div *ngIf="sel.selected" style="width:72px">
                      <input type="number" class="qty-input" [(ngModel)]="sel.returnQty" [min]="1" [max]="sel.item.quantity" (ngModelChange)="recalcRefundTotal()">
                    </div>
                    <div *ngIf="sel.selected" class="fw-800 text-danger text-sm" style="min-width:70px;text-align:left;font-family:'Outfit',sans-serif">
                      -{{ (sel.item.unitPrice || 0) * sel.returnQty | number:'1.2-2' }}
                    </div>
                  </div>
                </div>

                <div class="return-total-box">
                  <span class="return-total-label">إجمالي الارتجاع</span>
                  <span class="return-total-val">{{ refundTotal | number:'1.2-2' }} ج.م</span>
                </div>

                <div class="d-flex gap-2 mt-2">
                  <button class="btn-pos-ghost w-100" (click)="selectAllReturn()">الكل</button>
                  <button class="btn-pos-ghost w-100" (click)="clearReturnSelections()">إلغاء</button>
                </div>
                <button class="btn-pos-danger w-100 mt-2" (click)="processReturn()" [disabled]="isBusy || getSelectedReturnCount() === 0" style="padding:0.75rem">
                  <span *ngIf="isBusy" class="pos-spinner"></span>
                  <i *ngIf="!isBusy" class="fas fa-undo"></i> تأكيد الارتجاع
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Return Result -->
        <div class="col-pos-8" *ngIf="refundResult">
          <div class="pos-card">
            <div class="success-state" style="padding:3.5rem">
              <div class="success-emoji">🧾</div>
              <div class="success-title mb-2">تم إجراء الارتجاع بنجاح!</div>
              <div class="text-muted-c text-sm mb-1">رقم فاتورة الارتجاع</div>
              <div class="fw-800 text-accent mb-1" style="font-family:'Outfit',sans-serif;font-size:1.3rem">{{ refundResult.refundInvoiceNumber }}</div>
              <div class="text-muted-c text-sm mb-1">المبلغ المرتجع</div>
              <div class="fw-800 text-danger mb-3" style="font-family:'Outfit',sans-serif;font-size:1.8rem">{{ refundResult.refundAmount | number:'1.2-2' }} ج.م</div>
              <div class="d-flex align-center gap-2" style="justify-content:center">
                <button class="btn-pos-primary" style="width:auto;padding:0.65rem 1.5rem" (click)="printReturnInvoice(refundResult.refundInvoiceId)" [disabled]="isBusy">
                  <i class="fas fa-print"></i> طباعة (أصل وصورة)
                </button>
                <button class="btn-pos-ghost" (click)="resetReturn()"><i class="fas fa-undo"></i> ارتجاع جديد</button>
              </div>
            </div>
          </div>
        </div>

        <div class="col-pos-8" *ngIf="!refundResult">
          <div class="pos-card" style="text-align:center;padding:3rem;opacity:0.4">
            <i class="fas fa-receipt" style="font-size:3rem;display:block;margin-bottom:1rem"></i>
            <div class="text-muted-c">ابحث عن فاتورة لبدء إجراء الارتجاع</div>
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
  pendingReturns: InternalRequestDto[] = [];
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

  // ── Filters & Refunded List ──────────────────────────────────
  filterText = '';
  filterFromDate = '';
  filterToDate = '';
  refundedInvoices: PosInvoiceListDto[] = [];
  dispensedInvoices: PosInvoiceListDto[] = [];
  // ── Computed Counts ──────────────────────────────────────────
  get pendingApprovalCount() { return this.pendingInvoices.length; }
  // ── Pending Returns ─────────────────────────────────────────────
  get toDispenseCount() { return this.approvedInvoices.length; }
  get pendingReturnsCount() { return this.pendingReturns.length; }

  constructor(
    private posService: PosService,
    private settingsService: PharmacySettingsService,
    private internalRequestService: InternalRequestService,
    private toaster: ToasterService
  ) {}

  ngOnInit() {
    const d = new Date();
    const today = d.getFullYear() + '-' + ('0' + (d.getMonth() + 1)).slice(-2) + '-' + ('0' + d.getDate()).slice(-2);
    this.filterFromDate = today;
    this.filterToDate = today;
    this.settingsService.get().subscribe(s => { this.allowNegativeStock = s.allowNegativeStock; });
    this.loadAllQueues();
  }

  // ── Tab & Navigation ─────────────────────────────────────────

  setTab(tab: ActiveTab) {
    this.activeTab = tab;
    if (tab === 'pending-approval') { this.loadPendingApproval(); }
    if (tab === 'to-dispense') { this.loadToDispense(); }
    if (tab === 'refunded-list') { this.loadRefundedInvoices(); }
    if (tab === 'dispensed-list') { this.loadDispensedInvoices(); }
    if (tab === 'pending-returns') { this.loadPendingReturns(); }
  }

  applyFilters() {
    this.loadAllQueues();
  }

  loadAllQueues() {
    this.loadPendingApproval();
    this.loadToDispense();
    this.loadRefundedInvoices();
    this.loadDispensedInvoices();
    this.loadPendingReturns();
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
          error: () => { this.isBusy = false; this.toaster.error('فشل اعتماد الصرف', 'خطأ'); }
        });
      },
      error: () => { this.isBusy = false; this.toaster.error('فشل إنشاء الفاتورة', 'خطأ'); }
    });
  }

  approvePendingReturn(requestId: string) {
    this.isBusy = true;
    this.internalRequestService.approveReturn(requestId).subscribe({
      next: () => {
        this.toaster.success('تمت الموافقة على المرتجع وعكس القيود بنجاح');
        this.isBusy = false;
        this.loadPendingReturns();
      },
      error: () => {
        this.toaster.error('حدث خطأ أثناء الموافقة');
        this.isBusy = false;
      }
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
    this.posService.getPosInvoices(7 /* PendingApproval */, this.filterText, this.filterFromDate, this.filterToDate).subscribe({
      next: (inv) => { this.pendingInvoices = inv; this.isLoading = false; },
      error: () => this.isLoading = false
    });
    this.posService.getPosInvoices(8 /* Rejected */, this.filterText, this.filterFromDate, this.filterToDate).subscribe({
      next: (inv) => { this.rejectedInvoices = inv; },
      error: () => {}
    });
  }

  loadPendingReturns() {
    this.isLoading = true;
    this.internalRequestService.getPendingReturns({ maxResultCount: 100, skipCount: 0 } as any).subscribe({
      next: (res) => {
        this.pendingReturns = res.items;
        this.isLoading = false;
      },
      error: () => this.isLoading = false
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
        
        // Optimistically update lists (prevents browser caching from restoring the item)
        this.pendingInvoices = this.pendingInvoices.filter(i => i.id !== inv.id);
        inv.status = 3; // Paid
        this.approvedInvoices = [inv, ...this.approvedInvoices];

        // Refresh all queues from server
        this.loadAllQueues();
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
    const reasonToSave = this.rejectReason;
    this.posService.reject(inv.id, { rejectionReason: reasonToSave }).subscribe({
      next: () => {
        this.isBusy = false;
        this.rejectingInvoiceId = null;
        this.rejectReason = '';
        abp.notify.warn('تم رفض الفاتورة وإعادتها للصيدلي', 'رفض');
        
        // Optimistically update lists
        this.pendingInvoices = this.pendingInvoices.filter(i => i.id !== inv.id);
        inv.status = 8; // Rejected
        inv.rejectionReason = reasonToSave;
        this.rejectedInvoices = [inv, ...this.rejectedInvoices];

        // Refresh all queues from server
        this.loadAllQueues();
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
        
        // Optimistically update lists
        this.rejectedInvoices = this.rejectedInvoices.filter(i => i.id !== inv.id);
        inv.status = 7; // PendingApproval
        inv.rejectionReason = null;
        this.pendingInvoices = [inv, ...this.pendingInvoices];
      },
      error: () => { this.isBusy = false; }
    });
  }

  // ── Dispense (Pharmacist) ─────────────────────────────────────

  loadToDispense() {
    this.isLoading = true;
    this.posService.getPosInvoices(3 /* Paid */, this.filterText, this.filterFromDate, this.filterToDate).subscribe({
      next: (inv) => { this.approvedInvoices = inv; this.isLoading = false; },
      error: () => this.isLoading = false
    });
  }

  loadRefundedInvoices() {
    this.isLoading = true;
    this.posService.getPosInvoices(6 /* Refunded */, this.filterText, this.filterFromDate, this.filterToDate).subscribe({
      next: (inv) => { this.refundedInvoices = inv; this.isLoading = false; },
      error: () => this.isLoading = false
    });
  }

  loadDispensedInvoices() {
    this.isLoading = true;
    this.posService.getPosInvoices(9 /* Dispensed */, this.filterText, this.filterFromDate, this.filterToDate).subscribe({
      next: (inv) => { this.dispensedInvoices = inv; this.isLoading = false; },
      error: () => this.isLoading = false
    });
  }

  confirmDispense(inv: PosInvoiceListDto) {
    this.isBusy = true;
    this.posService.dispense(inv.id).subscribe({
      next: () => {
        this.isBusy = false;
        abp.notify.success('تم صرف الأصناف بنجاح', 'صرف');
        
        // Remove from approved queue so it disappears from the current tab
        this.approvedInvoices = this.approvedInvoices.filter(i => i.id !== inv.id);
        
        // Refresh all queues from server in the background
        this.loadAllQueues();
        
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
        
        // Refresh all queues from server
        this.loadAllQueues();

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

