import { adminRoutes } from './admin.routes';

describe('Admin routes wrapper contract (4.2.1 - failing)', () => {
  it('create and :id routes set route.data.showEditButtons = true', () => {
    const createRoute = adminRoutes.find(r => r.path === 'create');
    const idRoute = adminRoutes.find(r => r.path === ':id');

    expect(createRoute).toBeDefined();
    expect(idRoute).toBeDefined();

    // Failing expectation: wrappers should set showEditButtons in route.data
    expect(createRoute!.data && createRoute!.data['showEditButtons']).toBeTrue();
    expect(idRoute!.data && idRoute!.data['showEditButtons']).toBeTrue();
  });
});
