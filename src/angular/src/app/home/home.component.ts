import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService, LocalizationModule } from '@abp/ng.core';

@Component({
  standalone: true,
  imports: [CommonModule, LocalizationModule],
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
