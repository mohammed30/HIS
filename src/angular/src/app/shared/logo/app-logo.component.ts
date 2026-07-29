import { Component, inject, Input } from '@angular/core';
import { RoutesService, LocalizationService, CoreModule } from '@abp/ng.core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-logo',
  template: `
    <div class="logo-container">
      <a class="navbar-brand" routerLink="/">
        <!-- Light Theme Logo (shown by default or when light theme is active) -->
        <img
          src="./assets/images/logo/Dark.png"
          alt="Logo"
          width="100%"
          height="auto"
          class="logo-light"
        />
        <!-- Dark Theme Logo (shown when dark theme is active) -->
        <img
          src="./assets/images/logo/Light.png"
          alt="Logo"
          width="100%"
          height="auto"
          class="logo-dark"
        />
      </a>

      <!-- Menu Search Bar removed by user request -->
    </div>
  `,
  styles: [
    `
      :host {
        display: block;
        width: 100%;
      }
      .logo-container {
        display: flex;
        flex-direction: column;
        padding: 5px;
      }
      .navbar-brand {
        display: block;
        max-width: 60px;
        margin: 0 auto;
      }
      .logo-dark {
        display: none;
      }
      .logo-light {
        display: block;
      }

      /* When dark theme is active */
      :host-context([data-theme='dark']) .logo-light {
        display: none;
      }
      :host-context([data-theme='dark']) .logo-dark {
        display: block;
      }

      :host-context(.dark) .logo-light {
        display: none;
      }
      :host-context(.dark) .logo-dark {
        display: block;
      }

      .menu-search-wrapper {
        margin-bottom: 5px;
      }

      .form-control {
        font-size: 0.85rem;
        color: inherit !important;
        border-color: rgba(128, 128, 128, 0.3) !important;
      }

      .input-group-text {
        border-color: rgba(128, 128, 128, 0.3) !important;
      }

      /* Hide search when sidebar is collapsed (LeptonX often uses .lpx-sidebar-collapsed class) */
      :host-context(.lpx-sidebar-collapsed) .menu-search-wrapper {
        display: none;
      }
    `,
  ],
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, CoreModule],
})
export class AppLogoComponent {
  private routesService = inject(RoutesService);
  private localizationService = inject(LocalizationService);

  @Input() showSearch = true;

  searchText = '';

  onSearch() {
    const term = this.searchText.trim().toLowerCase();
    const normalizedTerm = this.normalizeArabic(term);
    const allRoutes = this.routesService.flat;

    if (!term) {
      allRoutes.forEach(route => {
        this.routesService.patch(route.name, { invisible: false });
      });
      return;
    }

    // 1. Find matches in localized names and raw names
    const matches = allRoutes.filter(route => {
      let localizedName = this.localizationService.instant(route.name);

      // ABP instant sometimes returns the key if it can't find it, or we need to handle "::"
      if (localizedName === route.name && route.name.startsWith('::')) {
        localizedName = this.localizationService.instant(route.name.replace('::', ''));
      }

      localizedName = (localizedName || '').toLowerCase();
      const normalizedLocalized = this.normalizeArabic(localizedName);
      const rawName = (route.name || '').toLowerCase();

      return (
        localizedName.includes(term) ||
        normalizedLocalized.includes(normalizedTerm) ||
        rawName.includes(term)
      );
    });

    // 2. Build set of names to show (including chain of parents)
    const showNames = new Set<string>();
    matches.forEach(match => {
      let current = match;
      while (current) {
        showNames.add(current.name);
        const parent = allRoutes.find(r => r.name === current.parentName);
        current = parent;
      }
    });

    // 3. Apply visibility via patch
    allRoutes.forEach(route => {
      const shouldBeVisible = showNames.has(route.name);
      if (route.invisible !== !shouldBeVisible) {
        this.routesService.patch(route.name, { invisible: !shouldBeVisible });
      }
    });
  }

  private normalizeArabic(text: string): string {
    if (!text) return '';
    return text
      .replace(/[أإآ]/g, 'ا')
      .replace(/ة/g, 'ه')
      .replace(/ى/g, 'ي')
      .replace(/[\u064B-\u0652]/g, ''); // Remove Tashkeel
  }

  clearSearch() {
    this.searchText = '';
    this.onSearch();
  }
}
