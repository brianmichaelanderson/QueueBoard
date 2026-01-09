import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';
import { routes } from '../app.routes';
import { AuthService } from '../services/auth.service';
import { NgZone } from '@angular/core';
import { Subject } from 'rxjs';
import { of } from 'rxjs';
import createRouterTestModule from '../../test-helpers/router-test-helpers';

describe('Admin routes (integration - blocking)', () => {
  beforeEach(() => {
    const cfg = createRouterTestModule(routes, { useFakeNgZone: true });

    TestBed.configureTestingModule({
      imports: cfg.imports,
      providers: [
        ...(cfg.providers || []),
        // non-admin AuthService stub — navigation to /admin should be blocked by guard
        { provide: AuthService, useValue: { isAdmin: () => of(false), isAuthenticated: () => of(true) } }
      ]
    });
  });

  it('blocks /admin for non-admin users', async () => {
    const router = TestBed.inject(Router);
    const location = TestBed.inject(Location);

    await router.initialNavigation();
    await router.navigate(['/admin']);

    // Expectation: navigation should NOT succeed for non-admin users
    expect(location.path()).not.toBe('/admin');
  });
});
