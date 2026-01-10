import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

// Lightweight auth service stub for dev — replace with real auth integration later.
@Injectable({ providedIn: 'root' })
export class AuthService {
  // internal roles state (default: non-admin to make tests explicit)
  private roles$ = new BehaviorSubject<string[]>([]);

  // Return whether user is authenticated (stubbed true for dev)
  isAuthenticated(): Observable<boolean> {
    return new Observable(sub => {
      sub.next(true);
      sub.complete();
    });
  }

  // Return user roles or claims (observable)
  getRoles(): Observable<string[]> {
    return this.roles$.asObservable();
  }

  // Convenience helper: return whether the current user is an admin
  isAdmin(): Observable<boolean> {
    return this.roles$.pipe(map(roles => roles.includes('admin')));
  }

  // Dev helper: set the current user to admin
  becomeAdmin() {
    this.roles$.next(['admin']);
  }

  // Dev helper: clear admin role
  becomeUser() {
    this.roles$.next([]);
  }
}
