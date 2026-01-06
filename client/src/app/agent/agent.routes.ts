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
    path: 'create',
    loadComponent: () => import('./agent-edit.component').then(m => m.AgentEditComponent),
    
  },
  {
    path: 'view/:id',
    loadComponent: () => import('./agent-detail.component').then(m => m.AgentDetailComponent),
    resolve: { initialData: agentsResolver }
  },
  {
    path: 'edit/:id',
    loadComponent: () => import('./agent-edit.component').then(m => m.AgentEditComponent)
  }
];
