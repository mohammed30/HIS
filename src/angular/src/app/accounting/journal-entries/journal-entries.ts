import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, PagedResultDto, CoreModule, SessionStateService } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, Confirmation, ToasterService } from '@abp/ng.theme.shared';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { FormGroup, FormBuilder, FormArray, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { JournalEntryService } from '../../proxy/accounting/journal-entry.service';
import { JournalEntryDto, AccountLookupDto, CreateUpdateJournalEntryDto } from '../../proxy/accounting/dtos';

interface TreeAccountNode extends AccountLookupDto {
  level: number;
  expanded: boolean;
  children: TreeAccountNode[];
  isVisible: boolean;
}

@Component({
  selector: 'app-journal-entries',
  standalone: true,
  imports: [CommonModule, NgbModule, NgxDatatableModule, CoreModule, ThemeSharedModule, ReactiveFormsModule, FormsModule],
  templateUrl: './journal-entries.html',
  styleUrls: ['./journal-entries.scss'],
  providers: [ListService],
})
export class JournalEntriesComponent implements OnInit {
  // List View
  journalEntries = { items: [], totalCount: 0 } as PagedResultDto<JournalEntryDto>;
  dateFrom: string | null = null;
  dateTo: string | null = null;

  // Editor State
  isEditorOpen = false;
  isEditing = false;
  isViewOnly = false;
  selectedEntryId: string | null = null;
  form: FormGroup;

  // Account Picker
  isAccountPickerOpen = false;
  accountPickerLineIndex = -1;
  allAccounts: AccountLookupDto[] = [];
  flatAccountTree: TreeAccountNode[] = [];
  accountSearchText = '';

  constructor(
    public readonly list: ListService,
    private journalEntryService: JournalEntryService,
    private fb: FormBuilder,
    private confirmation: ConfirmationService,
    private toaster: ToasterService,
    private sessionState: SessionStateService
  ) { }

  ngOnInit() {
    const today = new Date();
    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    // Format dates to YYYY-MM-DD
    this.dateFrom = firstDay.toISOString().substring(0, 10);
    this.dateTo = lastDay.toISOString().substring(0, 10);

    const hookFn = (query: any) => this.journalEntryService.getList({ ...query, dateFrom: this.dateFrom, dateTo: this.dateTo });
    this.list.hookToQuery(hookFn).subscribe((response) => {
      this.journalEntries = response;
    });
  }

  onDateFilterChange() {
    if (this.dateFrom && this.dateTo && new Date(this.dateFrom) > new Date(this.dateTo)) {
      this.toaster.warn('::JournalEntry:InvalidDateRange', 'تنبيه');
      return;
    }
    this.list.get();
  }

  // ─── List Actions ──────────────────────────────────────────

  createEntry() {
    this.selectedEntryId = null;
    this.isEditing = false;
    this.isViewOnly = false;
    this.buildForm();
    this.loadAccounts();
    this.isEditorOpen = true;
  }

  editEntry(entry: JournalEntryDto) {
    if (entry.isPosted) {
      this.toaster.warn('::JournalEntry:CannotEditPosted');
      return;
    }
    this.selectedEntryId = entry.id;
    this.isEditing = true;
    this.isViewOnly = false;
    this.loadEntryForEdit(entry.id);
  }

  viewEntry(entry: JournalEntryDto) {
    this.selectedEntryId = entry.id;
    this.isEditing = false;
    this.isViewOnly = true;
    this.loadEntryForEdit(entry.id);
  }

  deleteEntry(entry: JournalEntryDto) {
    if (entry.isPosted) {
      this.toaster.warn('::JournalEntry:CannotDeletePosted');
      return;
    }
    this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.journalEntryService.delete(entry.id).subscribe(() => {
          this.toaster.success('::SuccessfullyDeleted');
          this.list.get();
        });
      }
    });
  }

  postEntry(entry: JournalEntryDto) {
    if (entry.isPosted) return;
    this.confirmation.warn('::JournalEntry:PostConfirm', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        this.journalEntryService.post(entry.id).subscribe({
          next: () => {
            this.toaster.success('::SuccessfullySaved');
            this.list.get();
          },
          error: (err: any) => {
            // Error is handled by ABP global error handler
          }
        });
      }
    });
  }

  // ─── Editor ────────────────────────────────────────────────

  private loadEntryForEdit(id: string) {
    this.journalEntryService.get(id).subscribe((entry) => {
      this.buildForm(entry);
      this.loadAccounts();
      this.isEditorOpen = true;
    });
  }

  private buildForm(entry?: JournalEntryDto) {
    const today = new Date().toISOString().substring(0, 10);
    this.form = this.fb.group({
      date: [entry?.date ? entry.date.substring(0, 10) : today, [Validators.required]],
      referenceNumber: [entry?.referenceNumber || ''],
      description: [entry?.description || '', [Validators.required, Validators.maxLength(512)]],
      lines: this.fb.array([])
    });

    if (entry?.lines?.length) {
      entry.lines.forEach(line => this.addLine(line.accountId, line.accountName, line.accountNameAr, line.accountCode, line.debit, line.credit));
    } else {
      this.addLine();
      this.addLine();
    }
  }

  get lines(): FormArray {
    return this.form.get('lines') as FormArray;
  }

  addLine(accountId = '', accountName = '', accountNameAr = '', accountCode = '', debit = 0, credit = 0) {
    this.lines.push(this.fb.group({
      accountId: [accountId, [Validators.required]],
      accountName: [accountName],
      accountNameAr: [accountNameAr],
      accountCode: [accountCode],
      debit: [debit, [Validators.min(0)]],
      credit: [credit, [Validators.min(0)]],
    }));
  }

  removeLine(index: number) {
    if (this.lines.length > 2) {
      this.lines.removeAt(index);
    }
  }

  getLocalizedAccountName(line: any): string {
    const lang = this.sessionState.getLanguage();
    if (lang?.startsWith('ar') && line.get('accountNameAr')?.value) {
      return line.get('accountNameAr').value;
    }
    return line.get('accountName')?.value || '';
  }

  // ─── Balance Calculation ───────────────────────────────────

  get totalDebit(): number {
    return this.lines.controls.reduce((sum, line) => sum + (+(line.get('debit')?.value) || 0), 0);
  }

  get totalCredit(): number {
    return this.lines.controls.reduce((sum, line) => sum + (+(line.get('credit')?.value) || 0), 0);
  }

  get difference(): number {
    return Math.abs(this.totalDebit - this.totalCredit);
  }

  get isBalanced(): boolean {
    return this.totalDebit > 0 && this.totalDebit === this.totalCredit;
  }

  // ─── Account Picker ────────────────────────────────────────

  openAccountPicker(lineIndex: number) {
    this.accountPickerLineIndex = lineIndex;
    this.accountSearchText = '';
    this.buildAccountTree();
    this.isAccountPickerOpen = true;
  }

  private loadAccounts() {
    this.journalEntryService.getAccountLookup().subscribe((accounts) => {
      this.allAccounts = accounts;
    });
  }

  private buildAccountTree() {
    const nodes: TreeAccountNode[] = this.allAccounts.map(a => ({
      ...a,
      level: 0,
      expanded: true,
      children: [],
      isVisible: true
    }));

    const idMap = new Map(nodes.map(n => [n.id, n]));
    const roots: TreeAccountNode[] = [];

    nodes.forEach(node => {
      if (node.parentId && idMap.has(node.parentId)) {
        idMap.get(node.parentId)!.children.push(node);
      } else {
        roots.push(node);
      }
    });

    const sortNodes = (list: TreeAccountNode[]) => {
      list.sort((a, b) => (a.code || '').localeCompare(b.code || ''));
      list.forEach(child => sortNodes(child.children));
    };
    sortNodes(roots);

    this.flatAccountTree = [];
    const flatten = (list: TreeAccountNode[], level: number) => {
      list.forEach(node => {
        node.level = level;
        this.flatAccountTree.push(node);
        if (node.children.length > 0) {
          flatten(node.children, level + 1);
        }
      });
    };
    flatten(roots, 0);

    this.filterAccountTree();
  }

  toggleAccountExpand(node: TreeAccountNode) {
    node.expanded = !node.expanded;
    this.updateAccountVisibility();
  }

  filterAccountTree() {
    if (!this.accountSearchText) {
      this.flatAccountTree.forEach(x => {
        x.isVisible = true;
        x.expanded = true;
      });
      this.updateAccountVisibility();
      return;
    }

    const term = this.accountSearchText.toLowerCase();
    this.flatAccountTree.forEach(x => x.isVisible = false);

    const matches = this.flatAccountTree.filter(x =>
      (x.name || '').toLowerCase().includes(term) ||
      (x.code || '').toLowerCase().includes(term) ||
      (x.nameAr || '').toLowerCase().includes(term)
    );

    const reveal = (node: TreeAccountNode) => {
      node.isVisible = true;
      if (node.parentId) {
        const parent = this.flatAccountTree.find(p => p.id === node.parentId);
        if (parent) {
          parent.expanded = true;
          reveal(parent);
        }
      }
    };
    matches.forEach(m => reveal(m));
  }

  private updateAccountVisibility() {
    const setVisible = (nodes: TreeAccountNode[], parentVisible: boolean) => {
      nodes.forEach(node => {
        node.isVisible = parentVisible;
        if (node.children.length > 0) {
          setVisible(node.children, parentVisible && node.expanded);
        }
      });
    };
    const roots = this.flatAccountTree.filter(x => x.level === 0);
    setVisible(roots, true);
  }

  selectAccount(account: TreeAccountNode) {
    if (account.hasChildren) return; // Only leaf accounts

    const line = this.lines.at(this.accountPickerLineIndex);
    line.patchValue({
      accountId: account.id,
      accountName: account.name,
      accountNameAr: account.nameAr,
      accountCode: account.code
    });
    this.isAccountPickerOpen = false;
  }

  getLocalizedTreeName(node: TreeAccountNode): string {
    const lang = this.sessionState.getLanguage();
    if (lang?.startsWith('ar') && node.nameAr) {
      return node.nameAr;
    }
    return node.name || '';
  }

  // ─── Save & Close ─────────────────────────────────────────

  save() {
    if (this.form.invalid || !this.isBalanced) return;

    const dto: CreateUpdateJournalEntryDto = {
      date: this.form.value.date,
      referenceNumber: this.form.value.referenceNumber,
      description: this.form.value.description,
      lines: this.lines.controls.map(l => ({
        accountId: l.get('accountId')!.value,
        debit: +(l.get('debit')!.value) || 0,
        credit: +(l.get('credit')!.value) || 0
      }))
    };

    const request$ = this.isEditing
      ? this.journalEntryService.update(this.selectedEntryId!, dto)
      : this.journalEntryService.create(dto);

    request$.subscribe({
      next: () => {
        this.toaster.success('::SuccessfullySaved');
        this.closeEditor();
        this.list.get();
      },
      error: () => { }
    });
  }

  saveAndPost() {
    if (this.form.invalid || !this.isBalanced) return;

    const dto: CreateUpdateJournalEntryDto = {
      date: this.form.value.date,
      referenceNumber: this.form.value.referenceNumber,
      description: this.form.value.description,
      lines: this.lines.controls.map(l => ({
        accountId: l.get('accountId')!.value,
        debit: +(l.get('debit')!.value) || 0,
        credit: +(l.get('credit')!.value) || 0
      }))
    };

    this.confirmation.warn('::JournalEntry:PostConfirm', '::AreYouSure').subscribe((status) => {
      if (status === Confirmation.Status.confirm) {
        const create$ = this.isEditing
          ? this.journalEntryService.update(this.selectedEntryId!, dto)
          : this.journalEntryService.create(dto);

        create$.subscribe({
          next: (entry) => {
            this.journalEntryService.post(entry.id).subscribe({
              next: () => {
                this.toaster.success('::SuccessfullySaved');
                this.closeEditor();
                this.list.get();
              },
              error: () => { }
            });
          },
          error: () => { }
        });
      }
    });
  }

  closeEditor() {
    this.isEditorOpen = false;
    this.isEditing = false;
    this.isViewOnly = false;
    this.selectedEntryId = null;
  }

  getEntryTotal(entry: JournalEntryDto, type: 'debit' | 'credit'): number {
    return (entry.lines || []).reduce((sum, l) => sum + (type === 'debit' ? (l.debit || 0) : (l.credit || 0)), 0);
  }
}
