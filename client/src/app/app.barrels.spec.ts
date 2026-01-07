describe('Module barrels (TDD)', () => {
  it('agent barrel should export agentRoutes', async () => {
    const mod = await import('./agent');
    expect((mod as any).agentRoutes).toBeDefined();
  });

  it('admin barrel should export adminRoutes', async () => {
    const mod = await import('./admin');
    expect((mod as any).adminRoutes).toBeDefined();
  });
});
