import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService, LocalizationModule } from '@abp/ng.core';
import { AppLogoComponent } from '../shared/logo/app-logo.component';

@Component({
  standalone: true,
  imports: [CommonModule, LocalizationModule, AppLogoComponent],
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent {
  private authService = inject(AuthService);

  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated;
  }

  login() {
    this.authService.navigateToLogin();
  }
}
