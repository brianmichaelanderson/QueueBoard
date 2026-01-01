import { Routes } from '@angular/router';

export const adminRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-list.component').then(m => m.AdminListComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./admin-detail.component').then(m => m.AdminDetailComponent)
  }
];
