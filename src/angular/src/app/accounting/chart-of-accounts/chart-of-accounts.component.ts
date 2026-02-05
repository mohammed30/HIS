import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListService, PagedResultDto, CoreModule, LocalizationService, SessionStateService } from '@abp/ng.core';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AccountService } from '../../proxy/accounting/account.service';
import { AccountDto } from '../../proxy/accounting/dtos';
import { AccountType } from '../../proxy/accounting';

declare var abp: any;

interface TreeAccountDto extends AccountDto {
    level: number;
    expanded: boolean;
    children: TreeAccountDto[];
    hasChildren: boolean;
    isVisible: boolean; // For filtering/collapsing
    parentCode?: string; // Helper for display
    nameAr?: string; // Add NameAr
}

@Component({
    selector: 'app-chart-of-accounts',
    templateUrl: './chart-of-accounts.component.html',
    styleUrls: ['./chart-of-accounts.component.scss'],
    providers: [ListService],
    standalone: true,
    imports: [CommonModule, ThemeSharedModule, CoreModule, ReactiveFormsModule, FormsModule, NgbDropdownModule]
})
export class ChartOfAccountsComponent implements OnInit {
    flatAccounts: TreeAccountDto[] = []; // The ordered flattened tree for display
    allAccounts: AccountDto[] = []; // Raw data
    isModalOpen = false;
    form: FormGroup;
    selectedAccount: AccountDto = {} as AccountDto;

    accountTypes = [
        { value: 0, key: '::Enum:AccountType.Asset', color: 'success' },
        { value: 1, key: '::Enum:AccountType.Liability', color: 'danger' },
        { value: 2, key: '::Enum:AccountType.Equity', color: 'info' }, // mapped to purple in css
        { value: 3, key: '::Enum:AccountType.Revenue', color: 'primary' },
        { value: 4, key: '::Enum:AccountType.Expense', color: 'warning' },
    ];

    isAllExpanded = true;
    searchText = '';

    constructor(
        public readonly list: ListService,
        private accountService: AccountService,
        private fb: FormBuilder,
        private confirmation: ConfirmationService,
        private sessionState: SessionStateService
    ) { }

    // ... (omitting intermediate methods for brevity if possible, targeting getLocalizedName)



    ngOnInit() {
        this.loadAccounts();
    }

    loadAccounts() {
        // Fetch ALL accounts (large page size) to build the tree
        this.accountService.getList({ maxResultCount: 1000 }).subscribe((response) => {
            this.allAccounts = response.items;
            this.buildTree();
        });
    }

    buildTree() {
        // 1. Map to Tree DTOs
        const nodes: TreeAccountDto[] = this.allAccounts.map(a => ({
            ...a,
            level: 0,
            expanded: true, // Default expanded
            children: [],
            hasChildren: false,
            isVisible: true
        }));

        // 2. Build Hierarchy
        const idMap = new Map(nodes.map(n => [n.id, n]));
        const roots: TreeAccountDto[] = [];

        nodes.forEach(node => {
            if (node.parentId && idMap.has(node.parentId)) {
                const parent = idMap.get(node.parentId);
                parent.children.push(node);
                parent.hasChildren = true;
                node.parentCode = parent.code;
            } else {
                roots.push(node);
            }
        });

        // 3. Sort Children by Code
        const sortNodes = (n: TreeAccountDto[]) => {
            n.sort((a, b) => a.code.localeCompare(b.code));
            n.forEach(child => sortNodes(child.children));
        };
        sortNodes(roots);

        // 4. Flatten for Display (Depth-First Traversal)
        this.flatAccounts = [];
        const flatten = (nodes: TreeAccountDto[], level: number) => {
            nodes.forEach(node => {
                node.level = level;
                this.flatAccounts.push(node);
                if (node.children.length > 0) {
                    flatten(node.children, level + 1);
                }
            });
        };
        flatten(roots, 0);

        this.updateVisibility();
    }

    toggleExpand(account: TreeAccountDto) {
        account.expanded = !account.expanded;
        this.updateVisibility();
    }

    updateVisibility() {
        // Simple visibility logic: A node is visible if all its ancestors are expanded
        // Since list is ordered depth-first, we can just walk down

        // Easier approach for flat list:
        // 1. Mark all roots visible.
        // 2. If parent is expanded, show children.
        // But we have a flat list `flatAccounts`.

        // Re-flattening might be expensive if purely visual, but simplest for logic:
        // We will just iterate the flat list structure essentially.
        // Actually, let's just use the `isVisible` property logic based on parent status.
        // BUT, `parentId` reference lookup is O(N) in flat list unless we use the map.

        // Let's re-run the visibility flag setting starting from roots
        const setVisible = (nodes: TreeAccountDto[], parentExpanded: boolean) => {
            nodes.forEach(node => {
                node.isVisible = parentExpanded;
                if (node.children.length > 0) {
                    setVisible(node.children, parentExpanded && node.expanded); // Child visible only if parent visible AND parent expanded
                }
            });
        };

        // Get roots from flat list (level 0)
        // Re-using the hierarchy we built in memory (nodes linked by children ref)
        // We can just iterate the roots again? Roots are not stored separately in class property.
        // Let's filter roots from flatAccounts
        const roots = this.flatAccounts.filter(x => x.level === 0);
        setVisible(roots, true);
    }

    toggleExpandAll() {
        this.isAllExpanded = !this.isAllExpanded;
        this.flatAccounts.forEach(node => node.expanded = this.isAllExpanded);
        this.updateVisibility();
    }

    filterAccounts() {
        if (!this.searchText) {
            // Reset to default state
            this.flatAccounts.forEach(x => x.isVisible = true);
            this.updateVisibility();
            return;
        }

        const term = this.searchText.toLowerCase();

        // 1. Reset all to hidden first
        this.flatAccounts.forEach(x => x.isVisible = false);

        // 2. Find matches
        const matches = this.flatAccounts.filter(x =>
            x.name.toLowerCase().includes(term) ||
            x.code.toLowerCase().includes(term) ||
            (x.nameAr && x.nameAr.toLowerCase().includes(term))
        );

        // 3. Reveal matches and their ancestors
        const reveal = (node: TreeAccountDto) => {
            node.isVisible = true;
            // Also expand parents to make sure this path is open
            if (node.parentCode) {
                // Find parent in flat list (inefficient but safe) or Map if we kept it
                const parent = this.flatAccounts.find(p => p.code === node.parentCode);
                if (parent) {
                    parent.expanded = true;
                    reveal(parent);
                }
            }
        };

        matches.forEach(match => reveal(match));
    }

    createAccount() {
        this.selectedAccount = {} as AccountDto;
        this.buildForm();
        this.isModalOpen = true;
    }

    createChildAccount(parentId: string) {
        this.selectedAccount = { parentId: parentId } as AccountDto;
        // Optionally pre-fill Type based on parent
        const parent = this.flatAccounts.find(x => x.id === parentId);
        if (parent) {
            this.selectedAccount.type = parent.type;
        }
        this.buildForm();
        this.isModalOpen = true;
    }

    editAccount(id: string) {
        this.accountService.get(id).subscribe((account) => {
            this.selectedAccount = account;
            this.buildForm();
            this.isModalOpen = true;
        });
    }

    buildForm() {
        this.form = this.fb.group({
            code: [this.selectedAccount.code || '', [Validators.required, Validators.maxLength(32)]],
            name: [this.selectedAccount.name || '', [Validators.required, Validators.maxLength(128)]],
            nameAr: [this.selectedAccount.nameAr || '', [Validators.maxLength(128)]],
            type: [this.selectedAccount.type !== undefined ? this.selectedAccount.type : null, [Validators.required]],
            parentId: [this.selectedAccount.parentId || null],
        });
    }

    getLocalizedName(account: AccountDto): string {
        const currentLang = this.sessionState.getLanguage();
        if (currentLang && currentLang.startsWith('ar') && account.nameAr) {
            return account.nameAr;
        }
        return account.name;
    }

    save() {
        if (this.form.invalid) return;

        const request = this.selectedAccount.id
            ? this.accountService.update(this.selectedAccount.id, this.form.value)
            : this.accountService.create(this.form.value);

        request.subscribe(() => {
            this.isModalOpen = false;
            this.form.reset();
            this.loadAccounts(); // Reload tree
        });
    }

    delete(id: string) {
        this.confirmation.warn('::AreYouSureToDelete', '::AreYouSure').subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.accountService.delete(id).subscribe(() => this.loadAccounts());
            }
        });
    }

    getAccountType(value: number) {
        return this.accountTypes.find(x => x.value === value);
    }
}
