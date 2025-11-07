import { computed, inject, Injectable, signal } from '@angular/core';
import { LoginDto, RegisterDto, TokenDto } from '../models/auth.model';
import { BehaviorSubject, catchError, map, Observable, of, tap, throwError } from 'rxjs';
import { HttpClient, HttpContext, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { SKIP_AUTH } from '../../../core/tokens/http-context.tokens';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);
  private router = inject(Router);

  private readonly TOKEN_KEY = 'access_token';

  private currentUser = signal<TokenDto | null>(this.getTokenDto());
  public readonly currentUserSignal = this.currentUser.asReadonly();

  public currency = computed(() =>
    this.currentUserSignal()?.currency ?? 'EUR'
  );

  saveToken(tokenDto: TokenDto): void {
    localStorage.setItem(this.TOKEN_KEY, JSON.stringify(tokenDto));
  }

  private setCurrentUser(tokenDto: TokenDto | null) {
    this.currentUser.set(tokenDto);
  }

  getTokenDto(): TokenDto | null {
    const tokenStr = localStorage.getItem(this.TOKEN_KEY);
    if (!tokenStr) return null;
    return JSON.parse(tokenStr) as TokenDto;
  }

  getToken(): string | null {
    const tokenDto = this.getTokenDto();
    return tokenDto?.token ?? null;
  }

  isAuthenticated(): boolean {
    const tokenDto = this.getTokenDto();
    if (!tokenDto) return false;

    const expirationDate = new Date(tokenDto.expiration);
    return expirationDate > new Date();
  }

  ensureTokenValid(): Observable<TokenDto> {
    const token = this.getTokenDto();

    if (!token || this.isTokenExpired(token.token)) {
      // send request to refresh token if invalid or expired
      return this.refreshToken();
    }
    // token ok send it
    return of(token);
  }

  isTokenExpired(token: string): boolean {
    const payload = JSON.parse(atob(token.split('.')[1]));
    // convert milsec to sec
    const now = Math.floor(Date.now() / 1000);
    return payload.exp <= now;
  }

  refreshToken(): Observable<TokenDto> {
    return this.http.post<TokenDto>(
      `${environment.apiUrl}/refresh-token`,
      {},
      {
        withCredentials: true,
        context: new HttpContext().set(SKIP_AUTH, true)
      }
    ).pipe(
      tap(tokenDto => {
        this.saveToken(tokenDto);
        this.setCurrentUser(tokenDto);
      }),
      catchError(error => {
        this.setCurrentUser(null);
        return throwError(() => error);
      })
    );
  }

  register(credentials: RegisterDto): Observable<TokenDto> {
    return this.http.post<TokenDto>(
      `${environment.apiUrl}/register`,
      credentials,
      {
        withCredentials: true,
        context: new HttpContext().set(SKIP_AUTH, true)
      }
    ).pipe(
      tap(tokenDto => {
        this.saveToken(tokenDto);
        this.setCurrentUser(tokenDto);
      }),
      catchError((error) => this.handleError(error))
    );
  }

  private handleError = (error: HttpErrorResponse): Observable<never> => {
    let errorMessage = 'An unknown error occurred';

    this.setCurrentUser(null);

    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      if (error.status === 400 && error.error?.errors?.length > 0) {
        // Prendre la première erreur du tableau
        errorMessage = error.error.errors[0];
      } else if (error.error?.message) {
        errorMessage = error.error.message;
      } else {
        errorMessage = `Server error: ${error.status}`;
      }
    }

    return throwError(() => errorMessage);
  }

  login(credentials: LoginDto): Observable<TokenDto> {
    return this.http.post<TokenDto>(
      `${environment.apiUrl}/login`,
      credentials,
      {
        withCredentials: true,
        context: new HttpContext().set(SKIP_AUTH, true)
      }
    ).pipe(
      tap(tokenDto => {
        this.saveToken(tokenDto);
        this.setCurrentUser(tokenDto);
      }),
      catchError(error => {
        this.setCurrentUser(null);
        return throwError(() => error);
      })
    );
  }

  logout() {
    return this.http.post(`${environment.apiUrl}/logout`,
      {},
      { withCredentials: true }
    ).subscribe({
      next: () => this.cleanUp(),
      error: () => this.cleanUp()
    });
  }

  private cleanUp() {
    localStorage.removeItem(this.TOKEN_KEY);
    this.setCurrentUser(null);
    this.router.navigate(['/login']);
  }
}
