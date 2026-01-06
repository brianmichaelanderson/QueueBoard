import { agentRoutes } from './agent.routes';
import { agentsResolver } from './agents.resolver';

describe('agentRoutes wiring', () => {
  it('should attach agentsResolver to the list route as resolve.initialData', () => {
    const listRoute = agentRoutes.find(r => r.path === '');
    expect(listRoute).toBeTruthy();
    // Assert resolve exists and points to agentsResolver
    expect((listRoute as any).resolve).toBeDefined();
    expect((listRoute as any).resolve.initialData).toBe(agentsResolver);
  });
});
