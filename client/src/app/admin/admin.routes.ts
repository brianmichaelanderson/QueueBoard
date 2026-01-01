import { Routes } from '@angular/router';
import { adminGuard } from './admin.guard';

export const adminRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-list.component').then(m => m.AdminListComponent),
    canActivate: [adminGuard]
  },
  {
    path: ':id',
    loadComponent: () => import('./admin-detail.component').then(m => m.AdminDetailComponent),
    canActivate: [adminGuard]
  }
];
