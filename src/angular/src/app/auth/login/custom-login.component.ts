import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AuthService, SessionStateService, ConfigStateService } from '@abp/ng.core';
import { Router, ActivatedRoute } from '@angular/router';
import { ToasterService } from '@abp/ng.theme.shared';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-custom-login',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule, FormsModule],
    templateUrl: './custom-login.component.html',
    styleUrls: ['./custom-login.component.scss']
})
export class CustomLoginComponent implements OnInit {
    private fb = inject(FormBuilder);
    private authService = inject(AuthService);
    private router = inject(Router);
    private route = inject(ActivatedRoute);
    private toaster = inject(ToasterService);
    private config = inject(ConfigStateService);
    private session = inject(SessionStateService);

    form: FormGroup;
    inProgress = false;
    isPasswordVisible = false;

    languages: any[] = [];
    currentLang: string;

    ngOnInit(): void {
        this.form = this.fb.group({
            username: ['', [Validators.required]],
            password: ['', [Validators.required]],
            rememberMe: [false]
        });

        this.languages = this.config.getDeep('localization.languages') || [];
        this.currentLang = this.session.getLanguage();
    }

    togglePasswordVisibility() {
        this.isPasswordVisible = !this.isPasswordVisible;
    }

    onSubmit() {
        if (this.form.invalid) return;

        this.inProgress = true;

        // Using standard ABP auth service login
        // Note: This relies on the internal implementation of AuthService in @abp/ng.core
        // Usually it redirects to Identity Server or handles token request directly depending on config
        // For password flow/resource owner password:
        this.authService.login({
            username: this.form.value.username,
            password: this.form.value.password,
            rememberMe: this.form.value.rememberMe
        }).subscribe({
            next: () => {
                // Get returnUrl from query parameters or default to home
                const returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

                // Allow a small delay for token to be fully persisted and state synced
                setTimeout(() => {
                    // Using navigateByUrl vs window.location.href
                    // navigateByUrl is smoother, but sometimes a full refresh is needed to reload ABP config/permissions
                    // If we use navigateByUrl and it still requires refresh, we can switch back to window.location.href
                    this.router.navigateByUrl(returnUrl).then(success => {
                        if (!success) {
                            window.location.href = returnUrl;
                        }
                    });
                }, 100);
            },
            error: (err) => {
                this.inProgress = false;
                this.toaster.error('Invalid username or password', 'Login Failed');
                console.error(err);
            }
        });
    }

    changeLang(langCode: string) {
        this.session.setLanguage(langCode);
        window.location.reload();
    }

    // Helper to maintain branding
    get logoUrl() {
        return 'assets/images/logo/logo.svg';
    }
}
