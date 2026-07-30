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

        // Trim whitespace from username
        const username = (this.form.value.username || '').trim();
        if (!username) {
            this.errorMessage = 'اسم المستخدم لا يمكن أن يكون فارغاً';
            return;
        }

        this.inProgress = true;
        this.errorMessage = null;

        // Fix redirect: only use returnUrl if it's a local path (not external)
        const rawReturn = this.route.snapshot.queryParams['returnUrl'] || '';
        const redirectUrl = rawReturn && rawReturn.startsWith('/') ? rawReturn : '/';

        this.authService.login({
            username: username,
            password: this.form.value.password,
            rememberMe: this.form.value.rememberMe,
            redirectUrl: redirectUrl
        }).subscribe({
            next: () => {
                // ABP handles navigation after successful login
            },
            error: (err) => {
                this.inProgress = false;
                const status = err?.status || err?.error?.status;

                if (status === 400 || status === 401) {
                    this.errorMessage = 'اسم المستخدم أو كلمة المرور غير صحيحة. يرجى المحاولة مجدداً.';
                } else if (status === 423) {
                    this.errorMessage = 'تم قفل الحساب مؤقتاً بسبب محاولات دخول متعددة. يرجى الانتظار.';
                } else if (status === 0 || status === 503) {
                    this.errorMessage = 'تعذر الاتصال بالخادم. يرجى التحقق من اتصالك بالإنترنت والمحاولة مجدداً.';
                } else {
                    this.errorMessage = 'حدث خطأ غير متوقع أثناء تسجيل الدخول. يرجى المحاولة مجدداً.';
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
