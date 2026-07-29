import { Component, OnInit, inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';

@Component({
    selector: 'app-theme-toggle',
    standalone: true,
    imports: [CommonModule],
    template: `
    <button 
      class="nav-theme-toggle-btn" 
      (click)="toggleTheme()"
      [title]="isDarkMode ? 'تبديل إلى الوضع الفاتح' : 'تبديل إلى الوضع الداكن'">
      <i [class]="isDarkMode ? 'fas fa-sun' : 'fas fa-moon'"></i>
    </button>
  `,
  styles: [`
    .nav-theme-toggle-btn {
      background: transparent;
      border: none;
      position: relative;
      cursor: pointer;
      padding: 8px 10px;
      border-radius: 10px;
      color: var(--lpx-text-muted, #94a3b8);
      font-size: 1.1rem;
      transition: all 0.2s ease;
      outline: none;
      margin: 0 4px;
    }
    .nav-theme-toggle-btn:hover {
      color: #ffc107;
      background: rgba(255, 193, 7, 0.1);
    }
  `]
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
