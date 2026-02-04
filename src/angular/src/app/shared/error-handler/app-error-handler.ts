import { ErrorHandler, Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

/**
 * Custom Error Handler that clears stale OAuth tokens when ABP OAuth fails
 * This fixes the "Cannot read properties of undefined (reading 'injector')" error
 */
@Injectable()
export class AppErrorHandler implements ErrorHandler {
    private router = inject(Router);

    handleError(error: any): void {
        // Check if this is the ABP OAuth injector error
        const errorMessage = error?.message || error?.toString() || '';

        if (errorMessage.includes("Cannot read properties of undefined (reading 'injector')") ||
            errorMessage.includes('checkAccessToken')) {

            console.warn('OAuth token error detected, clearing stale tokens...');

            // Clear all OAuth-related storage
            this.clearOAuthStorage();

            // Reload to get fresh state
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

        // Clear sessionStorage
        const sessionKeysToRemove: string[] = [];
        for (let i = 0; i < sessionStorage.length; i++) {
            const key = sessionStorage.key(i);
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
                sessionKeysToRemove.push(key);
            }
        }
        sessionKeysToRemove.forEach(key => sessionStorage.removeItem(key));
    }
}
