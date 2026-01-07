import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuardCheck = (auth: AuthService, _route: any, _state: any) => {
  const roles: string[] = _route?.data?.roles || [];
  let allowed = true;
  if (roles.includes('admin')) {
    // when admin role is required, prefer calling isAdmin() if available
    const maybeIsAdmin = (auth as any).isAdmin;
    if (typeof maybeIsAdmin === 'function') {
      (maybeIsAdmin.call(auth) as any).subscribe((v: boolean) => (allowed = v)).unsubscribe();
      return allowed;
    }
  }
  // otherwise fall back to authenticated check
  auth.isAuthenticated().subscribe((v: boolean) => (allowed = v)).unsubscribe();
  return allowed;
};

// Simple auth guard — in dev it allows access, replace with real checks later.
export const authGuard: CanActivateFn = (_route, _state) => {
  const auth = inject(AuthService);
  return authGuardCheck(auth, _route, _state);
};
