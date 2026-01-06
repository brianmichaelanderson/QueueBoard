import { ResolveFn } from '@angular/router';
import { AgentDto } from '../shared/models/agent.model';
import { inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { AgentService } from '../services/agent.service';

// Resolver returns list data from the API via AgentService; falls back to empty list on error.
export interface AgentsResolveResult { items: AgentDto[]; total: number; item?: AgentDto }

export const agentsResolver: ResolveFn<AgentsResolveResult> = async (_route, _state) => {
  const agentService = inject(AgentService);
  try {
    const res = await firstValueFrom(agentService.list('', 1, 25));
    return { items: res.items ?? [], total: res.total ?? 0 };
  } catch {
    return { items: [], total: 0 };
  }
};
