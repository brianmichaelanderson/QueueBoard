import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

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
}
