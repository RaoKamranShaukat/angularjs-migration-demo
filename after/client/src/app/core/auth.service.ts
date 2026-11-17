import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, finalize, of, tap } from 'rxjs';
import { UserInfo } from './models';

// Login state for the whole app. The real "session" is the HttpOnly cookie set by the server;
// this service only remembers who is logged in so the UI can show the name.
// BEFORE: auth.service.js with a plain "currentUser" property.
// AFTER:  currentUser is a signal, so every template that reads it updates automatically.
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  readonly currentUser = signal<UserInfo | null>(null);

  login(username: string, password: string): Observable<UserInfo> {
    return this.http
      .post<UserInfo>('/api/auth/login', { username, password })
      .pipe(tap((user) => this.currentUser.set(user)));
  }

  logout(): Observable<void> {
    return this.http
      .post<void>('/api/auth/logout', {})
      .pipe(finalize(() => this.currentUser.set(null)));
  }

  // Returns the logged-in user. After a page refresh we don't know yet, so we ask the server.
  requireUser(): Observable<UserInfo> {
    const user = this.currentUser();
    if (user) {
      return of(user);
    }
    return this.http.get<UserInfo>('/api/auth/me').pipe(tap((u) => this.currentUser.set(u)));
  }

  clear(): void {
    this.currentUser.set(null);
  }
}
