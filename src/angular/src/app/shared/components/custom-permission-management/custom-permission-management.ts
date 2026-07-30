import { Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RestService } from '@abp/ng.core';
import { ePermissionManagementComponents } from '@abp/ng.permission-management';
import { ReplaceableComponentsService } from '@abp/ng.core';

@Component({
  selector: 'app-custom-permission-management',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './custom-permission-management.html',
  styleUrl: './custom-permission-management.scss'
})
export class CustomPermissionManagement implements OnChanges {
  @Input() providerName: string = '';
  @Input() providerKey: string = '';
  @Input() visible: boolean = false;
  @Output() visibleChange = new EventEmitter<boolean>();

  permissions: any[] = [];
  matrixPermissions: any[] = [];
  otherPermissions: any[] = [];
  otherPermissionsTree: any[] = [];
  isLoading = false;
  isSaving = false;

  constructor(
    private restService: RestService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && this.visible) {
      this.loadPermissions();
    }
  }

  close() {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }

  loadPermissions() {
    this.isLoading = true;
    this.restService.request<any, any>({
      method: 'GET',
      url: `/api/permission-management/permissions`,
      params: { providerName: this.providerName, providerKey: this.providerKey }
    }).subscribe({
      next: (res) => {
        // Flatten or organize permissions
        this.organizePermissions(res.groups);
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  organizePermissions(groups: any[]) {
    // Collect all permissions
    let allPerms: any[] = [];
    groups.forEach(g => {
      allPerms = [...allPerms, ...g.permissions];
    });

    this.permissions = allPerms;
    this.matrixPermissions = [];
    this.otherPermissions = [];
    this.otherPermissionsTree = [];

    // Group by base name (e.g. HIS.Patients, HIS.Patients.Create, HIS.Patients.Update, HIS.Patients.Delete)
    const baseNames = Array.from(new Set(allPerms.map(p => this.getBaseName(p.name))));

    baseNames.forEach(baseName => {
      const basePerm = allPerms.find(p => p.name === baseName);
      const createPerm = allPerms.find(p => p.name === `${baseName}.Create`);
      const updatePerm = allPerms.find(p => p.name === `${baseName}.Update`);
      const deletePerm = allPerms.find(p => p.name === `${baseName}.Delete`);

      if (createPerm || updatePerm || deletePerm) {
        this.matrixPermissions.push({
          baseName,
          displayName: basePerm ? basePerm.displayName : baseName,
          view: basePerm,
          create: createPerm,
          update: updatePerm,
          delete: deletePerm
        });
      } else {
        if (basePerm) {
           this.otherPermissions.push(basePerm);
        }
      }
    });

    // Build Tree for otherPermissions
    const permMap = new Map<string, any>();
    this.otherPermissions.forEach(p => {
      permMap.set(p.name, { ...p, children: [] });
    });

    this.otherPermissionsTree = [];
    permMap.forEach(node => {
      if (node.parentName && permMap.has(node.parentName)) {
        permMap.get(node.parentName).children.push(node);
      } else {
        this.otherPermissionsTree.push(node);
      }
    });
  }

  getBaseName(name: string): string {
    if (name.endsWith('.Create') || name.endsWith('.Update') || name.endsWith('.Delete')) {
      return name.substring(0, name.lastIndexOf('.'));
    }
    return name;
  }



  toggleView(matrix: any) {
    // If view is unchecked, uncheck children
    if (!matrix.view.isGranted) {
      if (matrix.create) matrix.create.isGranted = false;
      if (matrix.update) matrix.update.isGranted = false;
      if (matrix.delete) matrix.delete.isGranted = false;
    }
  }

  toggleChild(matrix: any) {
    // If any child is checked, view must be checked
    if ((matrix.create && matrix.create.isGranted) ||
        (matrix.update && matrix.update.isGranted) ||
        (matrix.delete && matrix.delete.isGranted)) {
      if (matrix.view) matrix.view.isGranted = true;
    }
  }

  // Sync actual permissions array before saving
  syncTreeToPermissions() {
    const updatePerms = (nodes: any[]) => {
      nodes.forEach(node => {
        const p = this.permissions.find(x => x.name === node.name);
        if (p) p.isGranted = node.isGranted;
        if (node.children) updatePerms(node.children);
      });
    };
    updatePerms(this.otherPermissionsTree);
  }

  save() {
    this.syncTreeToPermissions();
    this.isSaving = true;
    const dto = {
      permissions: this.permissions.map(p => ({ name: p.name, isGranted: p.isGranted }))
    };

    this.restService.request<any, any>({
      method: 'PUT',
      url: `/api/permission-management/permissions`,
      params: { providerName: this.providerName, providerKey: this.providerKey },
      body: dto
    }).subscribe({
      next: () => {
        this.isSaving = false;
        this.close();
      },
      error: () => {
        this.isSaving = false;
      }
    });
  }

  toggleTreeParent(node: any) {
    if (node.children) {
      node.children.forEach((child: any) => {
        child.isGranted = node.isGranted;
        this.toggleTreeParent(child); // recurse
      });
    }
  }

  toggleTreeChild(node: any, parentName: string) {
    if (node.isGranted && parentName) {
      // Find parent and set it to granted
      const checkAndSetParent = (nodes: any[]): boolean => {
        for (let n of nodes) {
          if (n.name === parentName) {
            n.isGranted = true;
            if (n.parentName) checkAndSetParent(this.otherPermissionsTree); // recursively check grandparent
            return true;
          }
          if (n.children && checkAndSetParent(n.children)) return true;
        }
        return false;
      };
      checkAndSetParent(this.otherPermissionsTree);
    }
  }
}
