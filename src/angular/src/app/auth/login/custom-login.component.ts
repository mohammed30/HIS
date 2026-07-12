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
    errorMessage: string | null = null;

    languages: any[] = [];
    currentLang: string;

    ngOnInit(): void {
        this.form = this.fb.group({
            username: ['', [Validators.required]],
            password: ['', [Validators.required]],
            rememberMe: [false]
        });

        this.languages = this.config.getDeep('localization.languages') || [];
        this.currentLang = this.session.getLanguage() || 'ar';

        // If no language is saved in session, set Arabic as default now
        if (!this.session.getLanguage()) {
            this.session.setLanguage('ar');
        }
    }

    togglePasswordVisibility() {
        this.isPasswordVisible = !this.isPasswordVisible;
    }

    onSubmit() {
        if (this.form.invalid) {
            this.form.markAllAsTouched();
            return;
        }

        this.inProgress = true;
        this.errorMessage = null;

        const redirectUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
        console.log('[Login Flow] Form is valid. redirectUrl:', redirectUrl);
        console.log('[Login Flow] Calling authService.login...');

        this.authService.login({
            username: this.form.value.username,
            password: this.form.value.password,
            rememberMe: this.form.value.rememberMe,
            redirectUrl: redirectUrl
        }).subscribe({
            next: (result) => {
                console.log('[Login Flow] authService.login returned successfully.', result);
                // We're letting ABP handle the redirection here.
                // Let's also listen if the router actually navigates.
                console.log('[Login Flow] Waiting for ABP to refresh app state and redirect...');
            },
            error: (err) => {
                console.error('[Login Flow] Error inside authService.login:', err);
                this.inProgress = false;
                const status = err?.status || err?.error?.status;

                if (status === 400 || status === 401) {
                    this.errorMessage = 'اسم المستخدم أو كلمة المرور غير صحيحة';
                } else if (status === 0 || status === 503) {
                    this.errorMessage = 'تعذر الاتصال بالخادم، يرجى المحاولة لاحقاً';
                } else {
                    this.errorMessage = 'حدث خطأ أثناء تسجيل الدخول، يرجى المحاولة مجدداً';
                }
            }
        });
    }

    changeLang(langCode: string) {
        this.session.setLanguage(langCode);
        window.location.reload();
    }

    get logoUrl() {
        return 'assets/images/logo/logo.svg';
    }
}
