import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ThemeSharedModule, ConfirmationService, Confirmation } from '@abp/ng.theme.shared';

interface AccountDto {
    id: string;
    code: string;
    nameAr: string;
    nameEn: string;
    parentId?: string;
    level: number;
    type: number;
    isLeaf: boolean;
    children?: AccountDto[];
    expanded?: boolean;
}

@Component({
    selector: 'app-chart-of-accounts',
    standalone: true,
    imports: [CommonModule, FormsModule, ThemeSharedModule],
    templateUrl: './chart-of-accounts.component.html',
    styles: [`
    .tree-node { cursor: pointer; padding: 5px; border-radius: 4px; }
    .tree-node:hover { background-color: #f8f9fa; }
    .tree-indent { margin-right: 20px; border-right: 1px dashed #ccc; }
    .node-content { display: flex; align-items: center; justify-content: space-between; }
  `]
})
export class ChartOfAccountsComponent implements OnInit {
    private http = inject(HttpClient);
    private apiUrl = environment.apis.default.url + '/api/app/account';
    private confirmation = inject(ConfirmationService);

    accounts: AccountDto[] = [];
    tree: AccountDto[] = [];
    loading = false;

    showForm = false;
    editingItem: AccountDto | null = null;
    formData: any = { nameAr: '', nameEn: '', type: 1, parentId: null };
    parentAccount: AccountDto | null = null;

    accountTypes = [
        { value: 1, label: 'أصول (Assets)' },
        { value: 2, label: 'خصوم (Liabilities)' },
        { value: 3, label: 'حقوق ملكية (Equity)' },
        { value: 4, label: 'إيرادات (Revenue)' },
        { value: 5, label: 'مصروفات (Expenses)' }
    ];

    ngOnInit() {
        this.loadData();
    }

    loadData() {
        this.loading = true;
        this.http.get<AccountDto[]>(this.apiUrl).subscribe({
            next: (res) => {
                this.accounts = res;
                this.buildTree();
                this.loading = false;
            },
            error: (err) => {
                console.error(err);
                this.loading = false;
            }
        });
    }

    buildTree() {
        const map = new Map<string, AccountDto>();
        const roots: AccountDto[] = [];

        // Initialize map and children array
        this.accounts.forEach(acc => {
            acc.children = [];
            acc.expanded = true; // Default expanded
            map.set(acc.id, acc);
        });

        this.accounts.forEach(acc => {
            if (acc.parentId && map.has(acc.parentId)) {
                map.get(acc.parentId)?.children?.push(acc);
            } else {
                roots.push(acc);
            }
        });

        // Sort by code
        const sortRecursive = (nodes: AccountDto[]) => {
            nodes.sort((a, b) => a.code.localeCompare(b.code));
            nodes.forEach(node => {
                if (node.children) sortRecursive(node.children);
            });
        };

        sortRecursive(roots);
        this.tree = roots;
    }

    toggleNode(node: AccountDto) {
        node.expanded = !node.expanded;
    }

    startAdd(parent: AccountDto | null = null) {
        this.editingItem = null;
        this.parentAccount = parent;
        this.formData = {
            nameAr: '',
            nameEn: '',
            type: parent ? parent.type : 1, // Inherit type from parent
            parentId: parent ? parent.id : null
        };
        this.showForm = true;
    }

    startEdit(item: AccountDto) {
        this.editingItem = item;
        // Find parent object for display
        this.parentAccount = item.parentId ? this.accounts.find(x => x.id === item.parentId) || null : null;

        this.formData = {
            nameAr: item.nameAr,
            nameEn: item.nameEn,
            type: item.type,
            parentId: item.parentId
        };
        this.showForm = true;
    }

    save() {
        if (this.editingItem) {
            this.http.put(`${this.apiUrl}/${this.editingItem.id}`, this.formData).subscribe({
                next: () => {
                    this.showForm = false;
                    this.loadData();
                },
                error: (err) => alert('Error updating account')
            });
        } else {
            this.http.post(this.apiUrl, this.formData).subscribe({
                next: () => {
                    this.showForm = false;
                    this.loadData();
                },
                error: (err) => alert('Error creating account')
            });
        }
    }

    delete(item: AccountDto) {
        this.confirmation.warn(`::AreYouSureToDeleteAccount`, '::AreYouSure', {
            messageLocalizationParams: [item.nameAr]
        }).subscribe((status) => {
            if (status === Confirmation.Status.confirm) {
                this.http.delete(`${this.apiUrl}/${item.id}`).subscribe({
                    next: () => this.loadData(),
                    error: (err) => alert('Cannot delete account. Note: Parent accounts cannot be deleted if they have children.')
                });
            }
        });
    }

    getTypeLabel(type: number) {
        return this.accountTypes.find(x => x.value === type)?.label;
    }
}
