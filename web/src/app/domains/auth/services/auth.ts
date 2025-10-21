import { inject, Injectable } from '@angular/core';
import { LoginDto, RegisterDto, TokenDto } from '../models/auth.model';
import { BehaviorSubject, catchError, Observable, tap, throwError } from 'rxjs';
import { HttpClient } from '@angular/common/http';
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
      catchError(error => {
        this.currentUserSubject.next(null);
        return throwError(() => error);
      })
    );
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
