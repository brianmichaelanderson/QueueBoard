import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

// Lightweight auth service stub for dev — replace with real auth integration later.
@Injectable({ providedIn: 'root' })
export class AuthService {
  // Return whether user is authenticated (stubbed true for dev)
  isAuthenticated(): Observable<boolean> {
    return of(true);
  }

  // Return user roles or claims (stub)
  getRoles(): Observable<string[]> {
    return of(['admin']);
  }

  // Convenience helper: return whether the current user is an admin
  isAdmin(): Observable<boolean> {
    return this.getRoles().pipe(map(roles => roles.includes('admin')));
  }
}
