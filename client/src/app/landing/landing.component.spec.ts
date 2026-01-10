import { routes } from '../app.routes';

describe('Landing route', () => {
  it('registers root landing route', () => {
    const root = routes.find(r => r.path === '');
    expect(root).toBeDefined();
    expect(root && (root.loadComponent || root.component)).toBeDefined();
  });
});
