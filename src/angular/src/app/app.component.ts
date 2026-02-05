import { Component, OnInit, inject } from '@angular/core';
import { DynamicLayoutComponent, ReplaceableComponentsService } from '@abp/ng.core';
import { LoaderBarComponent } from '@abp/ng.theme.shared';
import { ThemeToggleComponent } from './shared/theme-toggle/theme-toggle.component';
import { eAccountComponents } from '@abp/ng.account';
import { CustomLoginComponent } from './auth/login/custom-login.component';
import { AppLogoComponent } from './shared/logo/app-logo.component';

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
      component: CustomLoginComponent,
      key: eAccountComponents.Login,
    });
    this.replaceableComponents.add({
      component: AppLogoComponent,
      key: 'Logo',
    });
  }
}
