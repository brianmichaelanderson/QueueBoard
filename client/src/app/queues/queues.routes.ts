import { Routes } from '@angular/router';

export const queuesRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./queues.component').then(m => m.QueuesComponent)
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
