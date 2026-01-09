import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import createRouterTestModule from '../test-helpers/router-test-helpers';
import { routes } from './app.routes';

describe('App routes smoke (integration)', () => {
  beforeEach(() => {
    const cfg = createRouterTestModule(routes, { useFakeNgZone: true });

    TestBed.configureTestingModule({
      imports: cfg.imports,
      providers: cfg.providers || []
    });
  });

  it('boots the Router and contains lazy routes for agents and admin', async () => {
    const router = TestBed.inject(Router);

    await router.initialNavigation();

    const agents = router.config.find(r => r.path === 'agents');
    const admin = router.config.find(r => r.path === 'admin');

    expect(agents).toBeDefined();
    expect(agents && (agents.loadChildren || agents.loadComponent || agents.children)).toBeDefined();

    expect(admin).toBeDefined();
    expect(admin && (admin.loadChildren || admin.loadComponent || admin.children)).toBeDefined();
  });
});
