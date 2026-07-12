import { ErrorHandler, Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

/**
 * Custom Error Handler that clears stale OAuth tokens when ABP OAuth fails
 * This fixes the "Cannot read properties of undefined (reading 'injector')" error
 */
@Injectable()
export class AppErrorHandler implements ErrorHandler {
    private router = inject(Router);

    // Prevent reload loop: only allow one automatic reload per session
    private static readonly RELOAD_KEY = '__abp_reload_ts__';
    private static readonly RELOAD_COOLDOWN_MS = 10_000; // 10 seconds

    handleError(error: any): void {
        const errorMessage = error?.message || error?.toString() || '';

        if (errorMessage.includes("Cannot read properties of undefined (reading 'injector')") ||
            errorMessage.includes('checkAccessToken')) {

            // Don't clear tokens if we are on the login page
            if (this.router.url.includes('/account/login')) {
                console.warn('OAuth error on login page, skipping automatic token clear');
                return;
            }

            // Prevent reload loop: check how long ago we last reloaded
            const lastReload = Number(sessionStorage.getItem(AppErrorHandler.RELOAD_KEY) || '0');
            const now = Date.now();

            if (now - lastReload < AppErrorHandler.RELOAD_COOLDOWN_MS) {
                // We already reloaded very recently — do NOT reload again, just log
                console.warn('OAuth error detected but reload cooldown active, skipping reload. Error:', errorMessage);
                return;
            }

            console.warn('OAuth token error detected, clearing stale tokens and reloading...');
            this.clearOAuthStorage();

            // Record the time of this reload to prevent loops
            sessionStorage.setItem(AppErrorHandler.RELOAD_KEY, String(now));
            window.location.reload();
            return;
        }

        // Log other errors normally
        console.error('Application Error:', error);
    }

    private clearOAuthStorage(): void {
        // Clear localStorage
        const keysToRemove: string[] = [];
        for (let i = 0; i < localStorage.length; i++) {
            const key = localStorage.key(i);
            if (key && (
                key.includes('access_token') ||
                key.includes('refresh_token') ||
                key.includes('id_token') ||
                key.includes('token') ||
                key.includes('oauth') ||
                key.includes('PKCE') ||
                key.includes('nonce') ||
                key.includes('expires_at')
            )) {
                keysToRemove.push(key);
            }
        }
        keysToRemove.forEach(key => localStorage.removeItem(key));

        // Clear sessionStorage (but keep our reload key)
        const sessionKeysToRemove: string[] = [];
        for (let i = 0; i < sessionStorage.length; i++) {
            const key = sessionStorage.key(i);
            if (key && key !== AppErrorHandler.RELOAD_KEY && (
                key.includes('access_token') ||
                key.includes('refresh_token') ||
                key.includes('id_token') ||
                key.includes('token') ||
                key.includes('oauth') ||
                key.includes('PKCE') ||
                key.includes('nonce') ||
                key.includes('expires_at')
            )) {
                sessionKeysToRemove.push(key);
            }
        }
        sessionKeysToRemove.forEach(key => sessionStorage.removeItem(key));
    }
}
