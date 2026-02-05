import { Component } from '@angular/core';

@Component({
    selector: 'app-logo',
    template: `
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
  `,
    styles: [`
    :host {
      display: block;
      max-width: 120px;
    }
    .logo-dark { display: none; }
    .logo-light { display: block; }

    /* When dark theme is active (assuming 'dark' class on body or html) */
    :host-context([data-theme="dark"]) .logo-light { display: none; }
    :host-context([data-theme="dark"]) .logo-dark { display: block; }
    
    /* Fallback for other dark mode implementations */
    :host-context(.dark) .logo-light { display: none; }
    :host-context(.dark) .logo-dark { display: block; }
  `],
    standalone: true
})
export class AppLogoComponent { }
