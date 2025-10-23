import { inject, Injectable } from '@angular/core';
import { LoginDto, RegisterDto, TokenDto } from '../models/auth.model';
import { BehaviorSubject, catchError, Observable, of, tap, throwError } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);

  private readonly TOKEN_KEY = 'access_token';

  private currentUserSubject = new BehaviorSubject<TokenDto | null>(
    this.getTokenDto()
  );
  public currentUser$ = this.currentUserSubject.asObservable();

  saveToken(tokenDto: TokenDto): void {
    localStorage.setItem(this.TOKEN_KEY, JSON.stringify(tokenDto));
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
    const now = Math.floor(Date.now() / 1000);
    return payload.exp <= now;
  }

  refreshToken(): Observable<TokenDto> {
    return this.http.post<TokenDto>(
      `${environment.apiUrl}/refresh-token`,
      { withCredentials: true }
    ).pipe(
      tap(tokenDto => {
        this.saveToken(tokenDto);
        this.currentUserSubject.next(tokenDto);
      }),
      catchError(error => {
        this.currentUserSubject.next(null);
        return throwError(() => error);
      })
    );
  }

  register(credentials: RegisterDto): Observable<TokenDto> {
    return this.http.post<TokenDto>(
      `${environment.apiUrl}/register`,
      credentials,
      { withCredentials: true }
    ).pipe(
      tap(tokenDto => {
        this.saveToken(tokenDto);
        this.currentUserSubject.next(tokenDto);
      }),
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred';
    this.currentUserSubject.next(null);
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      if (error.status === 400 && error.error?.errors) {
        errorMessage = error.error.errors.join(', ');
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
      { withCredentials: true }
    ).pipe(
      tap(tokenDto => {
        this.saveToken(tokenDto);
        this.currentUserSubject.next(tokenDto);
      }),
      catchError(error => {
        this.currentUserSubject.next(null);
        return throwError(() => error);
      })
    );
  }

  logout() {
    return this.http.get(`${environment.apiUrl}/logout`,
      { withCredentials: true }
    ).pipe(
      tap(() => {
        localStorage.removeItem(this.TOKEN_KEY);
        this.currentUserSubject.next(null);
      })
    );
  }
}
