import { Component, OnInit, inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';

@Component({
    selector: 'app-theme-toggle',
    standalone: true,
    imports: [CommonModule],
    template: `
    <button 
      class="theme-toggle-btn" 
      [class.light-mode]="!isDarkMode"
      [class.dark-mode]="isDarkMode"
      (click)="toggleTheme()"
      [title]="isDarkMode ? 'تبديل إلى الوضع الفاتح' : 'تبديل إلى الوضع الداكن'">
      <i [class]="isDarkMode ? 'fas fa-sun' : 'fas fa-moon'"></i>
    </button>
  `
})
export class ThemeToggleComponent implements OnInit {
    private platformId = inject(PLATFORM_ID);
    isDarkMode = false;
    private readonly THEME_KEY = 'his-theme-mode';

    ngOnInit() {
        if (isPlatformBrowser(this.platformId)) {
            // Check saved preference or system preference
            const savedTheme = localStorage.getItem(this.THEME_KEY);
            if (savedTheme) {
                this.isDarkMode = savedTheme === 'dark';
            } else {
                // Check system preference
                this.isDarkMode = window.matchMedia('(prefers-color-scheme: dark)').matches;
            }
            this.applyTheme();
        }
    }

    toggleTheme() {
        this.isDarkMode = !this.isDarkMode;
        this.applyTheme();
        if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem(this.THEME_KEY, this.isDarkMode ? 'dark' : 'light');
        }
    }

    private applyTheme() {
        if (isPlatformBrowser(this.platformId)) {
            document.documentElement.setAttribute('data-theme', this.isDarkMode ? 'dark' : 'light');
        }
    }
}
