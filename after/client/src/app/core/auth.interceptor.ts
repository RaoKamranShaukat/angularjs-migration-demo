import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

// Runs for every HttpClient request. A 401 (session missing or expired) sends the user to /login.
// BEFORE: an $httpProvider interceptor factory, using $injector.get() to dodge a circular dependency.
// AFTER:  a plain function. inject() works here, no workaround needed.
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // /api/auth/* handles its own 401s (wrong password, guard check).
      if (error.status === 401 && !req.url.startsWith('/api/auth/')) {
        auth.clear();
        router.navigate(['/login']);
      }
      return throwError(() => error);
    }),
  );
};
