import { TestBed } from '@angular/core/testing';
import { Injector, runInInjectionContext, NgZone } from '@angular/core';
import { createFakeNgZone } from '../../test-helpers/router-test-helpers';
import { authGuard } from './auth.guard';
import { AuthService } from '../services/auth.service';
import { of } from 'rxjs';
import { Subject } from 'rxjs';

describe('AuthGuard (integration)', () => {
  it('allows admin when AuthService.isAdmin() returns true', () => {
    const fakeZone = createFakeNgZone();

    const injector = Injector.create({
      providers: [
        { provide: AuthService, useValue: { isAdmin: () => of(true), isAuthenticated: () => of(true) } },
        { provide: NgZone, useValue: fakeZone }
      ]
    });
    const result = runInInjectionContext(injector, () =>
      authGuard({ data: { roles: ['admin'] } } as any, {} as any)
    );

    expect(result).toBeTrue();
  });

  it('blocks admin when AuthService.isAdmin() returns false', () => {
    const fakeZone2 = createFakeNgZone();

    const injector = Injector.create({
      providers: [
        { provide: AuthService, useValue: { isAdmin: () => of(false), isAuthenticated: () => of(true) } },
        { provide: NgZone, useValue: fakeZone2 }
      ]
    });
    const result = runInInjectionContext(injector, () =>
      authGuard({ data: { roles: ['admin'] } } as any, {} as any)
    );

    expect(result).toBeFalse();
  });
});
