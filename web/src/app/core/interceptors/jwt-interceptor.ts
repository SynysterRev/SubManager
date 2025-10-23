import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../../domains/auth/services/auth';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const ensureToken$ = authService.ensureTokenValid();

  return ensureToken$.pipe(
    switchMap(token => {
      // Cloner la requête avec le token valide
      const authReq = req.clone({
        setHeaders: token ? { Authorization: `Bearer ${token.token}` } : {},
        withCredentials: true
      });
      return next(authReq);
    }),
    catchError(err => {
      // if token expired or invalid
      if (err.status === 401) {
        return authService.refreshToken().pipe(
          switchMap(newToken => {
            if (!newToken) return throwError(() => err);
            const newReq = req.clone({
              setHeaders: { Authorization: `Bearer ${newToken}` },
              withCredentials: true
            });
            return next(newReq);
          })
        );
      }
      return throwError(() => err);
    })
  );
};