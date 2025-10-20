import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../../domains/auth/services/auth';
import { inject } from '@angular/core';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = null; //authService.getToken();

  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      },
      withCredentials: true
    });
  } else {
    // Même sans token, on veut envoyer les cookies
    req = req.clone({
      withCredentials: true
    });
  }

  return next(req);
};
