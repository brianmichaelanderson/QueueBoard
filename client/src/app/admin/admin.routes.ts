import { Routes } from '@angular/router';
import { adminGuard } from './admin.guard';
import { authGuard } from '../auth/auth.guard';
import { AdminService } from '../services/admin.service';

export const adminRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-list.component').then(m => m.AdminListComponent),
    canActivate: [adminGuard, authGuard],
    data: { roles: ['admin'] },
    providers: [AdminService]
  },
  {
    path: 'create',
    loadComponent: () => import('./admin-edit-wrapper.component').then(m => m.AdminEditWrapperComponent),
    canActivate: [adminGuard, authGuard],
    data: { roles: ['admin'], showEditButtons: true },
    providers: [AdminService]
  },
  {
    path: ':id',
    loadComponent: () => import('./admin-detail-wrapper.component').then(m => m.AdminDetailWrapperComponent),
    canActivate: [adminGuard, authGuard],
    data: { roles: ['admin'], showEditButtons: true },
    providers: [AdminService]
  }
];
