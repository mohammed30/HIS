import { Component, OnInit, inject } from '@angular/core';
import {
  DynamicLayoutComponent,
  ReplaceableComponentsService,
  SessionStateService,
  AuthService,
} from '@abp/ng.core';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
import { ThemeToggleComponent } from './shared/theme-toggle/theme-toggle.component';
import { SidebarSearchComponent } from './shared/sidebar-search/sidebar-search.component';
import { eAccountComponents } from '@abp/ng.account';
import { AppLogoComponent } from './shared/logo/app-logo.component';
import { UserManagementComponent } from './identity-extended/users/user-management.component';
import { RoleManagementComponent } from './identity-extended/roles/role-management.component';
import { eIdentityComponents } from '@abp/ng.identity';
import { AppFooterComponent } from './layout/footer/app-footer.component';
import {
  Router,
  NavigationStart,
  NavigationCancel,
  NavigationError,
  NavigationEnd,
} from '@angular/router';
import { NotificationBellComponent } from './notifications/notification-bell/notification-bell.component';
import { LoginNotificationsModalComponent } from './notifications/login-modal/login-notifications-modal.component';
import { NotificationHubService } from './notifications/services/notification-hub.service';

import { NavItemsService } from '@abp/ng.theme.shared';

@Component({
  selector: 'app-root',
  template: `
    <abp-loader-bar />
    <abp-dynamic-layout />
    <app-login-notifications-modal />
    <app-sidebar-search />
  `,
  imports: [
    LoaderBarComponent,
    DynamicLayoutComponent,
    ThemeToggleComponent,
    LoginNotificationsModalComponent,
    SidebarSearchComponent,
  ],
})
export class AppComponent implements OnInit {
  private replaceableComponents = inject(ReplaceableComponentsService);
  private session = inject(SessionStateService);
  private router = inject(Router);
  private authService = inject(AuthService);
  private notifHub = inject(NotificationHubService);
  private navItems = inject(NavItemsService);

  constructor() {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        console.log('[Router] Navigation started to:', event.url);
      } else if (event instanceof NavigationCancel) {
        console.warn('[Router] Navigation cancelled to:', event.url, 'Reason:', event.reason);
      } else if (event instanceof NavigationError) {
        console.error('[Router] Navigation error to:', event.url, 'Error:', event.error);
      } else if (event instanceof NavigationEnd) {
        console.log('[Router] Navigation ended successfully to:', event.url);
      }
    });
  }

  ngOnInit(): void {
    // Set Arabic as the default language if not already set
    if (!this.session.getLanguage()) {
      this.session.setLanguage('ar');
    }

    // Connect SignalR hub when authenticated
    if (this.authService.isAuthenticated) {
      this.notifHub.connect();
    }

    // Add Notification Bell to the top navigation bar
    this.navItems.addItems([
      {
        id: 'Notifications',
        component: NotificationBellComponent,
        order: 1, // Put it on the right side
      },
      {
        id: 'ThemeToggle',
        component: ThemeToggleComponent,
        order: 2, // Put it next to notifications
      },
    ]);

    // Register custom components after app is initialized
    this.replaceableComponents.add({
      component: AppLogoComponent,
      key: 'Logo',
    });
    this.replaceableComponents.add({
      component: AppLogoComponent,
      key: 'Account.Logo',
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
