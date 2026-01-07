import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';
import { routes } from '../app.routes';
import { AuthService } from '../services/auth.service';
import { NgZone } from '@angular/core';
import { Subject } from 'rxjs';
import { of } from 'rxjs';

describe('Admin routes (integration - edit flows & guard)', () => {
  function makeFakeZone(): NgZone {
    return {
      run: (fn: any) => fn(),
      runOutsideAngular: (fn: any) => fn(),
      runTask: (fn: any) => fn(),
      onUnstable: new Subject<void>(),
      onMicrotaskEmpty: new Subject<void>(),
      onStable: new Subject<void>(),
      isStable: true,
      hasPendingMicrotasks: false,
    } as unknown as NgZone;
  }

  it('blocks /admin/create for non-admin users', async () => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule.withRoutes(routes)],
      providers: [
        { provide: AuthService, useValue: { isAdmin: () => of(false), isAuthenticated: () => of(true) } },
        { provide: NgZone, useValue: makeFakeZone() }
      ]
    });

    const router = TestBed.inject(Router);
    const location = TestBed.inject(Location);

    await router.initialNavigation();
    await router.navigate(['/admin/create']);

    expect(location.path()).not.toBe('/admin/create');
  });

  it('allows /admin/create for admin users', async () => {
    TestBed.configureTestingModule({
      imports: [RouterTestingModule.withRoutes(routes)],
      providers: [
        { provide: AuthService, useValue: { isAdmin: () => of(true), isAuthenticated: () => of(true) } },
        { provide: NgZone, useValue: makeFakeZone() }
      ]
    });

    const router = TestBed.inject(Router);
    const location = TestBed.inject(Location);

    await router.initialNavigation();
    await router.navigate(['/admin/create']);

    expect(location.path()).toBe('/admin/create');
  });
});
