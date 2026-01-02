import { Routes } from '@angular/router';
import { queuesResolver } from './queues.resolver';

export const queuesRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./queues-list.component').then(m => m.QueuesListComponent),
    resolve: { initialData: queuesResolver }
  },
  {
    path: 'create',
    loadComponent: () => import('./queue-edit.component').then(m => m.QueueEditComponent)
  },
  {
    path: 'view/:id',
    loadComponent: () => import('./queue-detail.component').then(m => m.QueueDetailComponent),
    resolve: { initialData: queuesResolver }
  },
  {
    path: 'edit/:id',
    loadComponent: () => import('./queue-edit.component').then(m => m.QueueEditComponent)
  }
];
