import { CanActivateFn } from '@angular/router';

// Simple admin guard stub — replace with real auth checks later
export const adminGuard: CanActivateFn = (route, state) => {
  return true;
};
