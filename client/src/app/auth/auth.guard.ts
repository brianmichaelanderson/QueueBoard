import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

// Simple auth guard — in dev it allows access, replace with real checks later.
export const authGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  // For now return true synchronously via snapshot of observable
  let allowed = true;
  auth.isAuthenticated().subscribe(v => (allowed = v)).unsubscribe();
  return allowed;
};
