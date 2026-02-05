import { Injectable } from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor,
    HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {

    intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
        return next.handle(request).pipe(
            catchError((error: HttpErrorResponse) => {
                if (error.status === 405) {
                    // Clone the error with a friendly message that ABP can display
                    const modifiedError = new HttpErrorResponse({
                        error: {
                            error: {
                                code: '405',
                                message: 'Method Not Allowed (405) - The requested action is not supported by the server.',
                                details: 'Please check if the API endpoint supports this HTTP method.'
                            }
                        },
                        headers: error.headers,
                        status: 405,
                        statusText: 'Method Not Allowed',
                        url: error.url || undefined
                    });
                    return throwError(() => modifiedError);
                }

                if (error.status === 400 && !error.error?.error?.message) {
                    // Handle non-ABP 400 errors (e.g. .NET Core ProblemDetails)
                    let message = 'Bad Request';
                    let details = 'The request input was invalid.';

                    if (error.error) {
                        if (error.error.title) {
                            message = error.error.title;
                        }
                        if (error.error.errors) {
                            // Join all error messages from ProblemDetails
                            const validationErrors = [];
                            for (const key in error.error.errors) {
                                if (Object.prototype.hasOwnProperty.call(error.error.errors, key)) {
                                    const errors = error.error.errors[key];
                                    validationErrors.push(`${key}: ${errors.join(', ')}`);
                                }
                            }
                            if (validationErrors.length > 0) {
                                details = validationErrors.join('\n');
                            }
                        } else if (typeof error.error === 'string') {
                            details = error.error;
                        }
                    }

                    const modifiedError = new HttpErrorResponse({
                        error: {
                            error: {
                                code: '400',
                                message: message,
                                details: details
                            }
                        },
                        headers: error.headers,
                        status: 400,
                        statusText: 'Bad Request',
                        url: error.url || undefined
                    });
                    return throwError(() => modifiedError);
                }
                return throwError(() => error);
            })
        );
    }
}
