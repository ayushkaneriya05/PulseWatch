import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

export interface AuthResponse {
  token: string;
  email: string;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());

  isLoggedIn$ = this.loggedIn.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  register(name: string, email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, { name, email, password })
      .pipe(tap(res => this.setSession(res)));
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, { email, password })
      .pipe(tap(res => this.setSession(res)));
  }

  logout(): void {
    localStorage.removeItem('pw_token');
    localStorage.removeItem('pw_user');
    this.loggedIn.next(false);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('pw_token');
  }

  getUser(): { name: string; email: string } | null {
    const user = localStorage.getItem('pw_user');
    return user ? JSON.parse(user) : null;
  }

  isLoggedIn(): boolean {
    return this.hasToken();
  }

  private setSession(res: AuthResponse): void {
    localStorage.setItem('pw_token', res.token);
    localStorage.setItem('pw_user', JSON.stringify({ name: res.name, email: res.email }));
    this.loggedIn.next(true);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('pw_token');
  }
}
