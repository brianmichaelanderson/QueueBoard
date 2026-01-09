import { RouterTestingModule } from '@angular/router/testing';
import { NgZone } from '@angular/core';
import { Subject } from 'rxjs';

/**
 * Create a lightweight fake NgZone that provides the lifecycle observables
 * Angular may subscribe to during Router integration tests. Useful when running
 * tests in the repo's zoneless change-detection mode.
 */
export function createFakeNgZone(): NgZone {
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

/**
 * Helper to produce TestBed `imports` and `providers` for router-based specs.
 * Usage:
 *   TestBed.configureTestingModule({
 *     ...createRouterTestModule(routes, { useFakeNgZone: true })
 *   });
 */
export function createRouterTestModule(routes: any[], options?: { useFakeNgZone?: boolean }) {
  const imports = [RouterTestingModule.withRoutes(routes)];
  const providers: any[] = [];

  if (options?.useFakeNgZone) {
    providers.push({ provide: NgZone, useValue: createFakeNgZone() });
  }

  return { imports, providers };
}

export default createRouterTestModule;
