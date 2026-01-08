import { Routes } from '@angular/router';
import { adminGuard } from './admin.guard';
import { authGuard } from '../auth/auth.guard';
import { AdminService } from '../services/admin.service';

export const adminRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./admin-agent-list.component').then(m => m.AdminAgentListComponent),
    canActivate: [adminGuard, authGuard],
    data: { roles: ['admin'] },
    providers: [AdminService]
  },
  {
    path: 'create',
    loadComponent: () => import('./admin-agent-edit.component').then(m => m.AdminAgentEditComponent),
    canActivate: [adminGuard, authGuard],
    data: { roles: ['admin'], showEditButtons: true },
    providers: [AdminService]
  },
  {
    path: ':id',
    loadComponent: () => import('./admin-agent-detail.component').then(m => m.AdminAgentDetailComponent),
    canActivate: [adminGuard, authGuard],
    data: { roles: ['admin'], showEditButtons: true },
    providers: [AdminService]
  }
];
