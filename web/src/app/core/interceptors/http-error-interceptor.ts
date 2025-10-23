import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  // const toastr = inject(ToastrService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // laisser passer pour le jwtInterceptor
        return throwError(() => error);
      }
      let errorMessage = 'An unexpected error occurred.';

      if (error.error instanceof ErrorEvent) {
        errorMessage = `Error: ${error.error.message}`;
      } else if (error.error) {
        if (typeof error.error === 'string') {
          errorMessage = error.error;
        }
        else if (typeof error.error === 'object' && error.error.message) {
          errorMessage = error.error.message;
        }
      } else {
        switch (error.status) {
          case 0:
            errorMessage = 'Cannot reach the server.';
            break;
          case 400:
            errorMessage = 'Invalid request. Please check your data.';
            break;
          case 403:
            errorMessage = 'Access denied.';
            break;
          case 404:
            errorMessage = 'Resource not found.';
            break;
          case 500:
            errorMessage = 'Internal server error.';
            break;
        }
      }

      return throwError(() => new Error(errorMessage));
    })
  );
};
