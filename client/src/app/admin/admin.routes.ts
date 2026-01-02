import { Routes } from '@angular/router';
import { adminGuard } from './admin.guard';
import { authGuard } from '../auth/auth.guard';
import { AdminService } from '../services/admin.service';

export const adminRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-list.component').then(m => m.AdminListComponent),
    canActivate: [adminGuard, authGuard],
    providers: [AdminService]
  },
  {
    path: 'create',
    loadComponent: () => import('./admin-edit.component').then(m => m.AdminEditComponent),
    canActivate: [adminGuard, authGuard],
    providers: [AdminService]
  },
  {
    path: ':id',
    loadComponent: () => import('./admin-detail.component').then(m => m.AdminDetailComponent),
    canActivate: [adminGuard, authGuard],
    providers: [AdminService]
  }
];
