import { adminRoutes } from './admin.routes';
import { agentsResolver } from '../agent/agents.resolver';
import { queuesResolver } from '../queues/queues.resolver';

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

  it('attaches agentsResolver as resolve.initialData for admin list and detail routes', () => {
    const listRoute = adminRoutes.find(r => r.path === '');
    const idRoute = adminRoutes.find(r => r.path === ':id');

    expect(listRoute).toBeDefined();
    expect(idRoute).toBeDefined();

    expect((listRoute as any).resolve).toBeDefined();
    expect((listRoute as any).resolve.initialData).toBe(agentsResolver);

    expect((idRoute as any).resolve).toBeDefined();
    expect((idRoute as any).resolve.initialData).toBe(agentsResolver);
  });

  it('attaches queuesResolver and showEditButtons for admin queues routes', () => {
    const queuesRoute = adminRoutes.find(r => r.path === 'queues');
    expect(queuesRoute).toBeDefined();
    expect((queuesRoute as any).resolve).toBeDefined();
    expect((queuesRoute as any).resolve.initialData).toBe(queuesResolver);
    expect((queuesRoute as any).data && (queuesRoute as any).data.showEditButtons).toBeTrue();
  });
});
