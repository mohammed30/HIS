import { Component, inject, OnInit } from '@angular/core';
import { RestService } from '@abp/ng.core';

@Component({
  selector: 'app-footer',
  template: `
    <div class="lpx-footbar-container end-0">
      <div class="lpx-footbar">
        <div class="lpx-footbar-copyright">
          <span>{{ currentYear }}© </span>
          <a href="https://asiahospital.com/" target="_blank"> Asia Hospital </a>
        </div>
        <div class="lpx-footbar-solo-links">
          <span class="text-muted" style="font-size: 0.85em;">v{{ version }}</span>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
    }
    
    /* Replicate default LeptonX styles to ensure layout matches */
    .lpx-footbar-container {
      /* inherit defaults from layout-bundle.css */
    }

    /* Enhanced Dark Mode Support */
    :host-context(html[data-theme="dark"]) .lpx-footbar {
      background-color: #212529 !important;
      color: #fff !important;
      border-top-color: #343a40 !important;
    }

    :host-context(html[data-theme="dark"]) .lpx-footbar-copyright a {
      color: #6ea8fe !important; /* Bootstrap link-light-blue for better contrast */
    }
  `],
  standalone: true
})
export class AppFooterComponent implements OnInit {
  currentYear = new Date().getFullYear();
  version = '...';

  private restService = inject(RestService);

  ngOnInit() {
    this.restService.request<any, string>({
      method: 'GET',
      url: '/api/app/version',
      responseType: 'text'
    }).subscribe(v => {
      this.version = v;
    });
  }
}
