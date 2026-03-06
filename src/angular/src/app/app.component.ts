import { Component, OnInit, inject } from '@angular/core';
import { DynamicLayoutComponent, ReplaceableComponentsService } from '@abp/ng.core';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
import { ThemeToggleComponent } from './shared/theme-toggle/theme-toggle.component';
import { eAccountComponents } from '@abp/ng.account';
import { CustomLoginComponent } from './auth/login/custom-login.component';
import { AppLogoComponent } from './shared/logo/app-logo.component';
import { UserManagementComponent } from './identity-extended/users/user-management.component';
import { RoleManagementComponent } from './identity-extended/roles/role-management.component';
import { eIdentityComponents } from '@abp/ng.identity';
import { AppFooterComponent } from './layout/footer/app-footer.component';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <abp-dynamic-layout />
    <app-theme-toggle />
  `,
  imports: [LoaderBarComponent, DynamicLayoutComponent, ThemeToggleComponent],
})
export class AppComponent implements OnInit {
  private replaceableComponents = inject(ReplaceableComponentsService);

  ngOnInit(): void {
    // Register custom components after app is initialized
    this.replaceableComponents.add({
      component: AppLogoComponent,
      key: 'Logo',
    });
    this.replaceableComponents.add({
      component: AppLogoComponent,
      key: 'Theme.LogoComponent',
    });

    // Replace Identity Components
    this.replaceableComponents.add({
      component: UserManagementComponent,
      key: eIdentityComponents.Users,
    });
    this.replaceableComponents.add({
      component: RoleManagementComponent,
      key: eIdentityComponents.Roles,
    });

    // Replace Footer Component
    this.replaceableComponents.add({
      component: AppFooterComponent,
      key: 'Theme.FooterComponent', // LeptonX uses this key based on FooterPanelDirective
    });
  }
}
