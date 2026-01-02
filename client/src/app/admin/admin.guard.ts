import { CanActivateFn } from '@angular/router';

// Simple admin guard stub — replace with real auth checks later
export const adminGuard: CanActivateFn = (_route, _state) => {
  return true;
};
