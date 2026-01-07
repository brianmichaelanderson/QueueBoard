import { routes } from './app.routes';

describe('App routes', () => {
  it('defines lazy routes for agents and admin', () => {
    const agents = routes.find(r => r.path === 'agents');
    const admin = routes.find(r => r.path === 'admin');

    expect(agents).toBeDefined();
    expect(agents && (agents.loadChildren || agents.loadComponent || agents.children)).toBeDefined();

    expect(admin).toBeDefined();
    expect(admin && (admin.loadChildren || admin.loadComponent || admin.children)).toBeDefined();
  });
});
