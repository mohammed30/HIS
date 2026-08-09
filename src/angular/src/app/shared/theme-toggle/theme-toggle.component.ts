import { Component, OnInit, inject, PLATFORM_ID } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ConfigStateService, RestService } from '@abp/ng.core';

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
    private configState = inject(ConfigStateService);
    private restService = inject(RestService);
    
    isDarkMode = false;
    private readonly THEME_KEY = 'his-theme-mode';

    ngOnInit() {
        if (isPlatformBrowser(this.platformId)) {
            // Load theme from database setting if available
            const dbTheme = this.configState.getSetting('HIS.User.Theme');
            if (dbTheme) {
                this.isDarkMode = dbTheme === 'dark';
                localStorage.setItem(this.THEME_KEY, dbTheme);
            } else {
                // Fallback to localStorage or default to White (light)
                const savedTheme = localStorage.getItem(this.THEME_KEY);
                if (savedTheme) {
                    this.isDarkMode = savedTheme === 'dark';
                } else {
                    this.isDarkMode = false; // Default to light mode
                }
            }
            this.applyTheme();
        }
    }

    toggleTheme() {
        this.isDarkMode = !this.isDarkMode;
        this.applyTheme();
        
        const themeString = this.isDarkMode ? 'dark' : 'light';
        
        if (isPlatformBrowser(this.platformId)) {
            localStorage.setItem(this.THEME_KEY, themeString);
        }

        // Save theme choice to database
        this.restService.request<void, void>({
            method: 'POST',
            url: `/api/app/user-settings/set-theme?theme=${themeString}`
        }, { skipHandleError: true }).subscribe({
            error: (err) => console.error('Failed to save theme to database', err)
        });
    }

    private applyTheme() {
        if (isPlatformBrowser(this.platformId)) {
            document.documentElement.setAttribute('data-theme', this.isDarkMode ? 'dark' : 'light');
        }
    }
}
