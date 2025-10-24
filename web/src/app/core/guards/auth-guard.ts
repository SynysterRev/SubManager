import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../domains/auth/services/auth';
import { catchError, map, of } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.ensureTokenValid().pipe(
    map(tokenDto => {
      if (tokenDto) {
        return true;
      } else {
        router.navigate(['/login'], {
          queryParams: { returnUrl: state.url }
        });
        return false;
      }
    }),
    catchError(() => {
      router.navigate(['/login'], {
        queryParams: { returnUrl: state.url }
      });
      return of(false);
    })
  );
};
