import { agentRoutes } from './agent.routes';
import { agentsResolver } from './agents.resolver';

describe('AgentModule routes (TDD)', () => {
  it('does not expose a create route', () => {
    const hasCreate = agentRoutes.some(r => r.path === 'create');
    expect(hasCreate).toBeFalse();
  });

  it('does not expose edit routes', () => {
    const hasEdit = agentRoutes.some(r => typeof r.path === 'string' && r.path.startsWith('edit'));
    expect(hasEdit).toBeFalse();
  });
});

describe('agentRoutes wiring', () => {
  it('should attach agentsResolver to the list route as resolve.initialData', () => {
    const listRoute = agentRoutes.find(r => r.path === '');
    expect(listRoute).toBeTruthy();
    // Assert resolve exists and points to agentsResolver
    expect((listRoute as any).resolve).toBeDefined();
    expect((listRoute as any).resolve.initialData).toBe(agentsResolver);
  });
});
