import { Routes } from '@angular/router';
import { AgentService } from '../services/agent.service';
import { agentsResolver } from './agents.resolver';

export const agentRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./agent-list.component').then(m => m.AgentListComponent),
    resolve: { initialData: agentsResolver }
  },
  {
    path: 'view/:id',
    loadComponent: () => import('./agent-detail.component').then(m => m.AgentDetailComponent),
    resolve: { initialData: agentsResolver }
  },
  // Note: edit/create flows are provided via AdminModule per routing policy (AgentModule is read-only)
];
