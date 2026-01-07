import { Injector, runInInjectionContext, NgZone } from '@angular/core';
import { adminGuard } from './admin.guard';
import { AuthService } from '../services/auth.service';
import { of, Subject } from 'rxjs';

describe('adminGuard (unit)', () => {
  it('allows navigation when AuthService.isAdmin() returns true', () => {
    const fakeZone = {
      run: (fn: any) => fn(),
      runOutsideAngular: (fn: any) => fn(),
      runTask: (fn: any) => fn(),
      onStable: new Subject<void>(),
      onMicrotaskEmpty: new Subject<void>(),
    } as unknown as NgZone;

    const injector = Injector.create({
      providers: [
        { provide: AuthService, useValue: { isAdmin: () => of(true) } },
        { provide: NgZone, useValue: fakeZone }
      ]
    });

    let result: any = runInInjectionContext(injector, () => adminGuard({} as any, {} as any));
    if (result && typeof result.subscribe === 'function') {
      let val = false as any;
      result.subscribe((v: any) => (val = v)).unsubscribe();
      result = val;
    }
    expect(result).toBeTrue();
  });

  it('blocks navigation when AuthService.isAdmin() returns false', () => {
    const fakeZone = {
      run: (fn: any) => fn(),
      runOutsideAngular: (fn: any) => fn(),
      runTask: (fn: any) => fn(),
      onStable: new Subject<void>(),
      onMicrotaskEmpty: new Subject<void>(),
    } as unknown as NgZone;

    const injector = Injector.create({
      providers: [
        { provide: AuthService, useValue: { isAdmin: () => of(false) } },
        { provide: NgZone, useValue: fakeZone }
      ]
    });

    let result: any = runInInjectionContext(injector, () => adminGuard({} as any, {} as any));
    if (result && typeof result.subscribe === 'function') {
      let val = true as any;
      result.subscribe((v: any) => (val = v)).unsubscribe();
      result = val;
    }
    expect(result).toBeFalse();
  });
});
