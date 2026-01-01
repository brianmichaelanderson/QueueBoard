import { Routes } from '@angular/router';

export const agentRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./agent-list.component').then(m => m.AgentListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./agent-edit.component').then(m => m.AgentEditComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./agent-detail.component').then(m => m.AgentDetailComponent)
  }
];
