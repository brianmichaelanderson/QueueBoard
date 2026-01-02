import { Routes } from '@angular/router';
import { AgentService } from '../services/agent.service';

export const agentRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./agent-list.component').then(m => m.AgentListComponent),
    providers: [AgentService]
  },
  {
    path: 'create',
    loadComponent: () => import('./agent-edit.component').then(m => m.AgentEditComponent),
    providers: [AgentService]
  },
  {
    path: ':id',
    loadComponent: () => import('./agent-detail.component').then(m => m.AgentDetailComponent),
    providers: [AgentService]
  }
];
