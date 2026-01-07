import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

// Admin guard: allow navigation only for users where `AuthService.isAdmin()` emits true.
export const adminGuard: CanActivateFn = (_route, _state) => {
  const auth = inject(AuthService);
  return auth.isAdmin();
};
