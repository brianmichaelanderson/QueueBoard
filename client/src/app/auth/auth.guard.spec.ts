import { authGuardCheck } from './auth.guard';
import { AuthService } from '../services/auth.service';
import { of } from 'rxjs';

describe('AuthGuard (TDD)', () => {
  it('allows admin when AuthService.isAdmin() returns true', () => {
    const mockAuth = { isAdmin: () => of(true), isAuthenticated: () => of(true) } as unknown as AuthService;
    const can = authGuardCheck(mockAuth, { data: { roles: ['admin'] } } as any, {} as any);
    expect(can).toBeTrue();
  });

  it('blocks admin when AuthService.isAdmin() returns false', () => {
    const mockAuth = { isAdmin: () => of(false), isAuthenticated: () => of(true) } as unknown as AuthService;
    const can = authGuardCheck(mockAuth, { data: { roles: ['admin'] } } as any, {} as any);
    expect(can).toBeFalse();
  });
});
