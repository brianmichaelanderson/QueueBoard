import { Routes } from '@angular/router';
import { queuesResolver } from './queues.resolver';

export const queuesRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./queues.component').then(m => m.QueuesComponent),
    resolve: { initialData: queuesResolver }
  },
  {
    path: 'create',
    loadComponent: () => import('./queue-edit.component').then(m => m.QueueEditComponent)
  },
  {
    path: 'edit/:id',
    loadComponent: () => import('./queue-edit.component').then(m => m.QueueEditComponent)
  }
];
