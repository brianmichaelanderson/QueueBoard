import { adminRoutes } from './admin.routes';

describe('Admin routes config (4.2.2 - smoke)', () => {
  it('has create and :id routes protected for admin', () => {
    const createRoute = adminRoutes.find(r => r.path === 'create');
    const idRoute = adminRoutes.find(r => r.path === ':id');

    expect(createRoute).toBeDefined();
    expect(idRoute).toBeDefined();

    expect(createRoute!.canActivate).toBeDefined();
    expect(idRoute!.canActivate).toBeDefined();

    expect(createRoute!.data && createRoute!.data['roles']).toContain('admin');
    expect(idRoute!.data && idRoute!.data['roles']).toContain('admin');
  });
});
